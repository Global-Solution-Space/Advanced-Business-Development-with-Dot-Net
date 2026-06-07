# AGENT.md — Terra Nova API (.NET 10)

> **Fonte da verdade.** Leia antes de tocar em qualquer arquivo. Portado do Java (Spring Boot) com paridade comportamental total.

---

## 1. Arquitetura (Clean Architecture — 5 camadas)

```
Controller → Service → Repository → Entity
```

| Camada | Responsabilidade | Regra |
|---|---|---|
| `TerraNova.Domain` | Entidades ricas, Enums, DomainException | Sem dependências externas |
| `TerraNova.Application` | Services, Interfaces, DTOs/Records | Orquestra; não conhece EF/HTTP |
| `TerraNova.Infrastructure` | DbContext, Repositórios, Migrations | Só Oracle/EF Core |
| `TerraNova.Integration` | HttpClients NASA POWER e SATVeg | Sem acesso ao banco |
| `TerraNova.API` | Controllers, Program.cs, Swagger | Nunca expõe Entity — só DTO |

---

## 2. Modelo de Dados

Todas as entidades herdam de `BaseEntity` (→ `Id: Guid`). Banco: **Oracle**.

| Entidade | Relacionamentos | Regra crítica |
|---|---|---|
| `Produtor` | 1:N `Propriedade`, 1:1 `Telefone` | Email único; senha ≥ 6 chars |
| `Telefone` | 1:1 `Produtor` | FK única por produtor; `DDD + Numero` bloqueado pela aplicação |
| `Localizacao` | 1:1 com `Propriedade` ou `Talhao` | `Point` (NTS) → `SDO_GEOMETRY` SRID 4326; sem FK própria |
| `Propriedade` | N:1 `Produtor`, 1:1 `Localizacao`, 1:N `Talhao` | Localização exclusiva (UNIQUE INDEX) |
| `Talhao` | N:1 `Propriedade`, 1:1 `Localizacao`, N:1 `TipoPlantacao` | `SUM(VolumArea) ≤ Propriedade.TamanhoTotal` |
| `TipoPlantacao` | 1:N `Talhao` | LookUp (soja, milho…) |
| `TipoApi` | 1:N `ReqApi` | LookUp (NASAPOWER, SATVEG) |
| `ReqApi` | **Sem FK para Talhao** — ligado via DadoTemporal | Cabeçalho da requisição |
| `DadoTemporal` | N:1 `Talhao`, N:1 `ReqApi` | Série temporal; tabela de interseção |
| `AlertaAgricola` | N:1 `Talhao` | `Resolver()` / `Reabrir()` — métodos ricos |

**Cascade Delete:** `Talhao` deleta → `DadoTemporal` + `AlertaAgricola` cascadeiam.
**Restrict Delete:** `Localizacao`, `TipoPlantacao` bloqueiam deleção se em uso.

---

## 3. Repositórios Especializados

Além do genérico `IRepository<T>` (CRUD + `ExistsById` + `ExistsByNome`):

| Interface | Métodos extras |
|---|---|
| `IProdutorRepository` | `GetByEmail`, `ExistsByEmail` |
| `IPropriedadeRepository` | `GetByProdutorId` (SQL WHERE), `ExistsByLocalizacaoId` (COUNT > 0) |
| `ITalhaoRepository` | `GetByPropriedadeId`, `GetByTipoPlantacaoId`, `GetByIdWithLocalizacao`, `SomarAreaPorPropriedade`, `ExistsByLocalizacaoId` |
| `IAlertaAgricolaRepository` | `GetByTalhaoId`, `GetByNivelAlerta`, `GetNaoResolvidos`, `ExisteAlertaAtivo` |
| `IReqApiRepository` | `GetByTalhaoId` (EXISTS via `.Any()`), `CountDadosByReqApiId`, `CountDadosByReqApiIds` (batch GROUP BY) |
| `IDadoTemporalRepository` | `GetByTalhaoId`, `GetByReqApiId`, `GetByTalhaoAndReqApi`, `AddRange` |

> **`GetByTalhaoId` em ReqApi:** `ReqApi` não tem FK para `Talhao`. A query usa EXISTS implícito:
> ```csharp
> Context.ReqApis.AsNoTracking()
>     .Where(r => r.DadosTemporais.Any(d => d.TalhaoId == talhaoId))
>     .ToList();
> ```

---

## 4. Leis de Performance — OBRIGATÓRIO

> Violações serão detectadas em code review. Não negocie.

### 4.1 AsNoTracking em todo GET
```csharp
_set.AsNoTracking().OrderBy(e => e.Id).ToList(); // ✅
_set.OrderBy(e => e.Id).ToList();                // ❌
```

### 4.2 Zero GetAll() em filtros — use SQL WHERE
```csharp
// ✅ SQL WHERE no Oracle:
Context.Propriedades.AsNoTracking().Where(p => p.ProdutorId == id).ToList();
// ❌ Carrega tudo em RAM:
GetAll().Where(p => p.ProdutorId == id).ToList();
```

### 4.3 Count() > 0 em vez de Any() — Oracle não aceita boolean literal
```csharp
_set.Count(e => e.Id == id) > 0  // ✅ — evita ORA-00904
_set.Any(e => e.Id == id)        // ❌ — ORA-00904: "FALSE": identificador inválido
```
> Aplica-se a: `ExistsById`, `ExistsByEmail`, `ExistsByLocalizacaoId`, `ExistsByNome`, `ExisteAlertaAtivo`.

### 4.4 Agregações no banco — não em memória
```csharp
// ✅ GROUP BY + COUNT no Oracle (batch):
var counts = reqApiRepository.CountDadosByReqApiIds(ids);
// ✅ SUM no Oracle:
Context.Talhoes.AsNoTracking().Where(t => t.PropriedadeId == id).Sum(t => t.VolumArea);
// ❌ Include() + Count em memória:
propriedade.Talhoes.Count
```

### 4.5 Async/Await ponta a ponta — proibido Sync-over-Async
```csharp
var dados = await nasaPowerClient.GetDailyDataAsync(...); // ✅
nasaPowerClient.GetDailyDataAsync(...).GetAwaiter().GetResult(); // ❌ thread starvation
```

### 4.6 CultureInfo.InvariantCulture em coordenadas
```csharp
// ✅ Sempre ponto decimal (NASA POWER rejeita vírgula com HTTP 422):
var lat = coordenadas.Y.ToString("0.0000", CultureInfo.InvariantCulture);
// ❌ pt-BR gera "-23,3849":
var lat = $"{coordenadas.Y}";
```

---

## 5. Fluxo de Integração (ReqApiService.CreateAsync)

```
POST /api/reqapi
  → valida TipoApi e Talhao
  → cria ReqApi (SaveChanges)
  → switch(TipoParam):
      0 (NDVI)        → BuscarDadosSatVeg()
      1 (PRECTOTCORR) → BuscarDadosNasaPower()
  → dadoTemporalRepository.AddRange(dados)  ← SaveChanges aqui
  → AnalisarEGerarAlertas(talhaoId, dados, "NASA POWER"|"SATVEG")
      ↳ dados passados EM MEMÓRIA — não re-consulta o banco
  → retorna ReqApiResponse com totalDadosSalvos
```

**Filtros de dados externos:**

| API | Filtro | Normalização |
|---|---|---|
| NASA POWER | Ignorar `valor <= -900.0` | `YYYYMMDD` → `YYYY-MM-DD` |
| SATVeg | Ignorar datas `< "2020-01-01"` | HTTP 400 (oceano) → `InvalidOperationException` |

**Thresholds de Alerta Automático:**

| API | Condição | Nível | Título no banco |
|---|---|---|---|
| NASA POWER | Chuva 3 dias > 80mm | Alto | Risco de Alagamento (NASA) |
| NASA POWER | Chuva 15 dias < 10mm | Crítico | Seca Severa (NASA) |
| NASA POWER | Chuva 15 dias < 25mm | Médio | Estresse Hídrico (NASA) |
| SATVeg | NDVI < 0.2 | Crítico | Anomalia Vegetativa Severa (SATVEG) |
| SATVeg | NDVI < 0.4 | Médio | Baixo Vigor Vegetativo (SATVEG) |

> **Deduplicação:** `ExisteAlertaAtivo(talhaoId, titulo)` — não cria alerta duplicado se já existir ativo com o mesmo título.
> **Sem alerta ≠ bug:** Regiões saudáveis não geram alertas. Use coordenadas de região seca/semi-árida para forçar.

**SATVeg token** (`appsettings.json`, com fallback hardcoded em `ReqApiService`):
```json
{ "SatVegApiToken": "Bearer e97dab05-eedc-39b9-a3fd-fa83cb5fef5e" }
```

---

## 6. Camada de Validação

Validadores customizados em `Application/DTOs/Validators/`, injetam serviços via `ValidationContext.GetService`:

| Validador | Aplicado em | Comportamento |
|---|---|---|
| `[BrasilCoordenadas]` | `LocalizacaoRequest` | Chama `bigdatacloud.net`; bloqueia se `countryCode != "BR"`; fail-safe (aprova se API cair) |
| `[UniqueEmail]` | `ProdutorRequest` | Injeta `IProdutorRepository` e checa duplicata |
| `[UniqueTelefone]` | `ProdutorRequest`, `TelefoneRequest` | Checa `DDD + Numero`; no cadastro de produtor, separa `TelefoneContato` em DDD + número |

> `LocalizacaoRequest.ToDomain()` → `Coordinate(longitude, latitude)` — ordem X=lon, Y=lat (WKT padrão).

---

## 7. Tratamento de Exceções Global

`GlobalExceptionHandler.cs` implementa `IExceptionHandler` (registrado no `Program.cs`). Controllers **não** devem ter `try/catch` para regras de negócio.

| Exceção | HTTP |
|---|---|
| `DomainException` | 400 |
| `InvalidOperationException` | 400 |
| `ArgumentException` / `ArgumentNullException` | 400 |
| `KeyNotFoundException` | 404 |
| `UnauthorizedAccessException` | 401 |
| `OracleException` | 502 |
| Qualquer outra | 500 (stack trace em Development) |

---

## 8. Controllers (CRUD completo em todas)

| Controller | Endpoints extras de negócio |
|---|---|
| `ProdutorController` | `GET by-email` |
| `PropriedadeController` | `GET by-produtor/{id}` |
| `TalhaoController` | `GET by-propriedade/{id}`, `GET by-tipo-plantacao/{id}` |
| `TelefoneController` | `GET by-produtor/{id}` |
| `LocalizacaoController` | — |
| `TipoPlantacaoController` | — |
| `TipoApiController` | — |
| `ReqApiController` | `GET talhao/{id}`, `POST` async (integração) |
| `DadoTemporalController` | `GET talhao/{id}`, `GET req-api/{id}` |
| `AlertaAgricolaController` | `GET talhao/{id}`, `PATCH /{id}/resolver`, `PATCH /{id}/reabrir` |

---

## 9. Banco de Dados e Migrations

- Banco Oracle FIAP — tabelas criadas e versionadas pelo EF Core Migrations.
- Metadados espaciais (`USER_SDO_GEOM_METADATA`) e índice (`MDSYS.SPATIAL_INDEX_V2`) requerem privilégios DBA — criar manualmente.
- `NetTopologySuite` com SRID 4326 (WGS 84) — configurado via `UseNetTopologySuite()` em `ServiceCollectionExtensions`.

```powershell
# Secrets (rodar dentro de TerraNova\TerraNova.API)
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:TerraNovaOracle" "User Id=RMxxxxxx;Password=xxxxxx;Data Source=oracle.fiap.com.br:1521/orcl;"

# Migrations (rodar dentro de TerraNova\)
dotnet ef migrations add Initial --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API
dotnet ef database update        --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API
dotnet ef database update 0      --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API
dotnet ef database drop --force  --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API

# Executar
dotnet run --project .\TerraNova.API
```

---

## 10. TL;DR — Checklist para Novos Códigos

1. **Nova tabela?** → `Entity` + `Configuration.cs` + DTO + Controller.
2. **Novo filtro/query?** → Método no repositório especializado. **Nunca** `GetAll().Where(...)`.
3. **Verificar existência?** → `Count(...) > 0` — jamais `.Any()` em query LINQ (ORA-00904).
4. **Regra de negócio?** → `InvalidOperationException` no Service → GlobalExceptionHandler → 400.
5. **Integração HTTP?** → `async/await` ponta a ponta + `CultureInfo.InvariantCulture` em coordenadas.
6. **DTO com validação especial?** → `ValidationAttribute` em `Application/DTOs/Validators/`.
7. **Existência por campo único?** → Repositório especializado com `Count(x => x.Campo == valor) > 0`.

---

## 11. Histórico de Bugs Corrigidos

### 11.1 — Cultura pt-BR: Coordenadas com Vírgula (NASA POWER)
**Sintoma:** HTTP 422 da NASA. **Causa:** `$"{latitude}"` em pt-BR gera `"-23,3849"`.
**Fix:** `ToString("0.0000", CultureInfo.InvariantCulture)` antes de montar a URL.

### 11.2 — Alertas não Gerados (dados fora do Change Tracker)
**Sintoma:** `totalDadosSalvos: 2348` mas 0 alertas criados. **Causa:** `AnalisarEGerarAlertas` re-consultava o banco antes do `SaveChanges` → retornava 0 registros.
**Fix:** Passar `List<DadoTemporal> dados` diretamente como parâmetro; não re-consultar o banco.

### 11.3 — PropriedadeService: N+1 em GetByProdutorId e Create (2026-06-07)
**Causa A:** `GetByProdutorId` usava `GetAll().Where(...)` — carregava tudo em RAM.
**Causa B:** `Create` usava `GetAll().Any(p => p.LocalizacaoId == ...)` — mesma violação.

**Fix:**
```csharp
// GetByProdutorId — SQL WHERE direto no Oracle
Context.Propriedades.AsNoTracking()
    .Where(p => p.ProdutorId == id).OrderBy(p => p.Nome).ToList();

// ExistsByLocalizacaoId — COUNT no banco
Context.Propriedades.AsNoTracking().Count(p => p.LocalizacaoId == id) > 0;
```

**Arquivos criados/modificados:**
- `Application/Repositories/IPropriedadeRepository.cs` ← **NOVO**
- `Infrastructure/Persistence/Repositories/PropriedadeRepository.cs` ← **NOVO**
- `Application/Services/Implementations/PropriedadeService.cs` ← atualizado
- `API/Extensions/ServiceCollectionExtensions.cs` ← `IPropriedadeRepository` registrado

---

## 12. Testes via PowerShell (legado)

> **Não usar este bloco como fonte atual.** Ele foi mantido apenas como histórico; use o script automático da seção 12.1.

```powershell
$base = "http://localhost:5160/api"

# ── Setup (execute em ordem, guarde os IDs) ──────────────────────────────────
Invoke-RestMethod "$base/Produtor"      -Method Post -ContentType "application/json" `
  -Body '{"nome":"Joao Silva","email":"joao@terranova.com","senha":"123456","telefoneContato":"11999999999"}'

Invoke-RestMethod "$base/Localizacao"   -Method Post -ContentType "application/json" `
  -Body '{"latitude":-12.9714,"longitude":-38.5014}'   # Salvador-BA — região seca (força alertas)

Invoke-RestMethod "$base/TipoPlantacao" -Method Post -ContentType "application/json" -Body '{"tipoPlant":"Soja"}'
Invoke-RestMethod "$base/TipoApi"       -Method Post -ContentType "application/json" -Body '{"nomeTipoApi":"NASAPOWER"}'
Invoke-RestMethod "$base/TipoApi"       -Method Post -ContentType "application/json" -Body '{"nomeTipoApi":"SATVEG"}'

Invoke-RestMethod "$base/Propriedade"   -Method Post -ContentType "application/json" `
  -Body '{"nome":"Fazenda","tamanhoTotal":100,"produtorId":"<id>","localizacaoId":"<id>"}'

Invoke-RestMethod "$base/Talhao"        -Method Post -ContentType "application/json" `
  -Body '{"nomeTalhao":"Talhao 1","volumArea":50,"tipoPlantacaoId":"<id>","propriedadeId":"<id>","localizacaoId":"<id>"}'

# ── Integração ────────────────────────────────────────────────────────────────
# tipoParam: 0 = NDVI (SATVeg) | 1 = PRECTOTCORR (NASA POWER)
Invoke-RestMethod "$base/ReqApi" -Method Post -ContentType "application/json" `
  -Body '{"tipoParam":1,"tipoApiId":"<tipoApiNASAId>","talhaoId":"<talhaoId>"}'
Invoke-RestMethod "$base/ReqApi" -Method Post -ContentType "application/json" `
  -Body '{"tipoParam":0,"tipoApiId":"<tipoApiSATVEGId>","talhaoId":"<talhaoId>"}'

# ── Verificar resultados ──────────────────────────────────────────────────────
Invoke-RestMethod "$base/AlertaAgricola/talhao/<talhaoId>" -Method Get
Invoke-RestMethod "$base/DadoTemporal/talhao/<talhaoId>"   -Method Get
Invoke-RestMethod "$base/ReqApi/talhao/<talhaoId>"         -Method Get

# ── Ciclo de vida do Alerta ───────────────────────────────────────────────────
Invoke-RestMethod "$base/AlertaAgricola/<alertaId>/resolver" -Method Patch
Invoke-RestMethod "$base/AlertaAgricola/<alertaId>/reabrir"  -Method Patch

# ── Validações (devem retornar 400) ──────────────────────────────────────────
Invoke-RestMethod "$base/Produtor" -Method Post -ContentType "application/json" `
  -Body '{"nome":"Dup","email":"joao@terranova.com","senha":"123456","telefoneContato":"11888888888"}'

Invoke-RestMethod "$base/Talhao"   -Method Post -ContentType "application/json" `
  -Body '{"nomeTalhao":"Grande","volumArea":200,"tipoPlantacaoId":"<id>","propriedadeId":"<id>","localizacaoId":"<id>"}'
```

### 12.1 Script automático atual

> Execute a API antes: `dotnet run --project .\TerraNova.API`

```powershell
$base = "http://localhost:5160/api"
$runId = (Get-Date -Format "HHmmss")

function Invoke-ApiJson {
  param(
    [Parameter(Mandatory=$true)][string]$Uri,
    [Parameter(Mandatory=$true)][string]$Method,
    [object]$Body = $null
  )

  $json = if ($null -ne $Body) { $Body | ConvertTo-Json -Depth 8 } else { $null }
  Invoke-RestMethod $Uri -Method $Method -ContentType "application/json" -Body $json
}

function Expect-BadRequest {
  param(
    [Parameter(Mandatory=$true)][string]$Label,
    [Parameter(Mandatory=$true)][scriptblock]$Action
  )

  try {
    & $Action | Out-Null
    throw "$Label deveria retornar 400, mas passou."
  }
  catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -ne 400) { throw }
    Write-Host "OK 400 - $Label"
  }
}

$produtor = Invoke-ApiJson "$base/Produtor" "Post" @{
  nome = "Joao $runId"
  email = "joao$runId@tn.com"
  senha = "123456"
  telefoneContato = "1199999$runId".Substring(0, 11)
}

$localizacaoPropriedade = Invoke-ApiJson "$base/Localizacao" "Post" @{
  latitude = -12.9714
  longitude = -38.5014
}

$localizacaoTalhao = Invoke-ApiJson "$base/Localizacao" "Post" @{
  latitude = -12.9814
  longitude = -38.5114
}

$localizacaoTalhaoInvalido = Invoke-ApiJson "$base/Localizacao" "Post" @{
  latitude = -12.9914
  longitude = -38.5214
}

$tipoPlantacao = Invoke-ApiJson "$base/TipoPlantacao" "Post" @{ tipoPlant = "Soja $runId" }
$tipoApiNasa = Invoke-ApiJson "$base/TipoApi" "Post" @{ nomeTipoApi = "NASA$runId".Substring(0, 10) }
$tipoApiSatveg = Invoke-ApiJson "$base/TipoApi" "Post" @{ nomeTipoApi = "SAT$runId" }

$propriedade = Invoke-ApiJson "$base/Propriedade" "Post" @{
  nome = "Fazenda $runId"
  tamanhoTotal = 100
  produtorId = $produtor.id
  localizacaoId = $localizacaoPropriedade.id
}

$talhao = Invoke-ApiJson "$base/Talhao" "Post" @{
  nomeTalhao = "Talhao $runId"
  volumArea = 50
  tipoPlantacaoId = $tipoPlantacao.id
  propriedadeId = $propriedade.id
  localizacaoId = $localizacaoTalhao.id
}

Invoke-RestMethod "$base/Produtor/$($produtor.id)" -Method Get
Invoke-RestMethod "$base/Produtor/by-email?email=$($produtor.email)" -Method Get
Invoke-RestMethod "$base/Telefone/by-produtor/$($produtor.id)" -Method Get
Invoke-RestMethod "$base/Propriedade/by-produtor/$($produtor.id)" -Method Get
Invoke-RestMethod "$base/Talhao/by-propriedade/$($propriedade.id)" -Method Get
Invoke-RestMethod "$base/Talhao/by-tipo-plantacao/$($tipoPlantacao.id)" -Method Get

try {
  $reqNasa = Invoke-ApiJson "$base/ReqApi" "Post" @{
    tipoParam = 1
    tipoApiId = $tipoApiNasa.id
    talhaoId = $talhao.id
  }
  Invoke-RestMethod "$base/DadoTemporal/req-api/$($reqNasa.id)" -Method Get
}
catch {
  Write-Warning "Integração NASA POWER não validada: $($_.Exception.Message)"
}

try {
  $reqSatveg = Invoke-ApiJson "$base/ReqApi" "Post" @{
    tipoParam = 0
    tipoApiId = $tipoApiSatveg.id
    talhaoId = $talhao.id
  }
  Invoke-RestMethod "$base/DadoTemporal/req-api/$($reqSatveg.id)" -Method Get
}
catch {
  Write-Warning "Integração SATVeg não validada: $($_.Exception.Message)"
}

Invoke-RestMethod "$base/DadoTemporal/talhao/$($talhao.id)" -Method Get
Invoke-RestMethod "$base/ReqApi/talhao/$($talhao.id)" -Method Get

$alerta = Invoke-ApiJson "$base/AlertaAgricola" "Post" @{
  titulo = "Alerta $runId"
  descricao = "Teste manual do ciclo de vida"
  nivelAlerta = 2
  talhaoId = $talhao.id
}
Invoke-RestMethod "$base/AlertaAgricola/talhao/$($talhao.id)" -Method Get
Invoke-RestMethod "$base/AlertaAgricola/$($alerta.id)/resolver" -Method Patch
Invoke-RestMethod "$base/AlertaAgricola/$($alerta.id)/reabrir" -Method Patch

Expect-BadRequest "e-mail duplicado" {
  Invoke-ApiJson "$base/Produtor" "Post" @{
    nome = "Dup Email"
    email = $produtor.email
    senha = "123456"
    telefoneContato = "1188888$runId".Substring(0, 11)
  }
}

Expect-BadRequest "telefone duplicado" {
  Invoke-ApiJson "$base/Produtor" "Post" @{
    nome = "Dup Tel"
    email = "duptel$runId@tn.com"
    senha = "123456"
    telefoneContato = $produtor.telefoneContato
  }
}

Expect-BadRequest "área de talhão maior que a propriedade" {
  Invoke-ApiJson "$base/Talhao" "Post" @{
    nomeTalhao = "Grande $runId"
    volumArea = 200
    tipoPlantacaoId = $tipoPlantacao.id
    propriedadeId = $propriedade.id
    localizacaoId = $localizacaoTalhaoInvalido.id
  }
}
```
