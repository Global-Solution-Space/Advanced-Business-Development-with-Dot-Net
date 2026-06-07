# Dossiê Arquitetural: Terra Nova API (.NET)

**Guia Definitivo para Agentes de Inteligência Artificial**

Este documento é a fonte da verdade para o projeto Terra Nova API. Qualquer futura implementação, refatoração ou adição de features deve respeitar ESTRITAMENTE as regras, arquiteturas e otimizações descritas aqui. Este projeto foi portado de um backend Java (Spring Boot) e mantém paridade comportamental total, traduzida para os padrões idiomáticos e de alta performance do .NET 10.

---

## 1. Padrão Arquitetural Base (Clean Architecture)

O projeto segue uma estrutura de camadas estrita (`Controller -> Service -> Repository -> Entity`), garantindo o princípio da Inversão de Dependência (SOLID):

*   **`TerraNova.Domain`**: O coração do sistema. Contém apenas Entidades (ricas), Enums e Exceções de Domínio. **Não possui dependências de infraestrutura**.
*   **`TerraNova.Application`**: Contém as regras de negócio puras (Services), Contratos (Interfaces), e os Data Transfer Objects (DTOs/Records). É a camada orquestradora.
*   **`TerraNova.Infrastructure`**: Implementa os contratos de acesso a dados (`DbContext`, Repositórios) e interage fisicamente com o banco Oracle.
*   **`TerraNova.Integration`**: Isola clientes HTTP (`Refit` ou `HttpClient` padrão) responsáveis por buscar dados externos (NASA, Embrapa).
*   **`TerraNova.API`**: A porta de entrada (Controllers, Program.cs, Middlewares, Swagger). **Nunca expõe as Entidades do EF Core diretamente.** Tudo entra e sai como DTO (blindagem de fronteira).

---

## 2. Entidades de Domínio e Relacionamentos

Todas as entidades herdam de `BaseEntity` (que provê o `Id` como `Guid`). O banco de dados alvo é o **Oracle**.

*   **`Produtor` e `Telefone` (1:N)**: O Produtor é a raiz do usuário. O `Telefone` armazena `DDD` e `Numero`.
*   **`Localizacao` (Espacial)**: Usa a biblioteca `NetTopologySuite`. A propriedade `Coordenadas` é do tipo `Point` e é mapeada no Oracle como **`MDSYS.SDO_GEOMETRY`**. É a "ponta fraca" da relação (relacionamento 1:1 direcional partindo de outras entidades). Não possui chaves estrangeiras.
*   **`Propriedade`**: Pertence a um `Produtor` (N:1). Tem uma `Localizacao` exclusiva (1:1). Possui `TamanhoTotal` (decimal).
*   **`Talhao`**: Subdivisão da `Propriedade` (N:1). Possui sua própria `Localizacao` (1:1, centro do polígono). Possui `VolumArea`. Possui relação N:1 com `TipoPlantacao` (LookUp table: soja, milho, etc.).
    *   **Regra de Negócio Crucial**: A soma das áreas (`VolumArea`) de todos os talhões de uma propriedade **nunca pode exceder** a área total (`TamanhoTotal`) da `Propriedade` pai. (Validado no `TalhaoService`).
*   **`AlertaAgricola`**: Entidade reativa ligada a um `Talhao` (N:1). Possui `NivelAlerta`, `Titulo`, `Descricao`, `Data` e `Resolvido`. Possui métodos ricos de domínio (`Resolver()` e `Reabrir()`) que alteram seu estado.

---

## 3. O Coração do Monitoramento: Integrações (Facade Pattern)

A coleta de dados é separada em Cabeçalho e Linhas para não inchar o banco:

*   **`ReqApi` (Cabeçalho)**: Histórico de "quem pediu o quê" (`TipoParamReqApi`: NDVI ou PRECTOTCORR). **Não tem FK direta para Talhão**.
*   **`DadoTemporal` (Linhas)**: Possui `Valor` (decimal), `DataLeitura`, e FKs para o `Talhao` e para a `ReqApi` (tabela de interseção N:N resolvida).

**Orquestração (`ReqApiService.cs`):**
O Serviço age como um Maestro. Recebe a solicitação da Controller, identifica o tipo de dado pedido, e repassa para a API correta de forma **assíncrona** (`Task` / `await`):
*   **NASA POWER (Chuva)**: Busca de 2020 até "hoje" usando a latitude/longitude do Talhão. Ignora retornos com falha de sensor (`-900.0`).
*   **Embrapa SatVeg (NDVI)**: Faz o parser de um JSON complexo para uma API nacional, tratando eventuais falhas (ex: coordenadas caindo no oceano repassam erros de negócio controlados).

**Endpoint de Consulta Otimizada (`GET /api/reqapi/talhao/{id}`):**
Como `ReqApi` não possui FK direta para `Talhao`, a busca de "Quais integrações foram feitas para o Talhão X?" usa um **EXISTS implícito via LINQ `.Any()`** no `ReqApiRepository`:
```csharp
Context.ReqApis.AsNoTracking()
    .Where(r => r.DadosTemporais.Any(d => d.TalhaoId == talhaoId))
    .ToList();
```
Isso gera um `EXISTS` ou `INNER JOIN` super otimizado no Oracle, **sem trazer duplicatas para a memória**.

---

## 4. Otimizações de Performance (MANDATÓRIO PARA NOVOS CÓDIGOS)

Se for escrever novos métodos nos repositórios ou serviços, SIGA ESTAS LEIS:

1.  **Read-Only Sempre com `.AsNoTracking()`**: Todos os métodos de leitura (GET) no `IRepository` e seus derivados **DEVEM** usar `.AsNoTracking()`. Exemplo:
    `_set.AsNoTracking().OrderBy(e => e.Id).ToList();`
    *Motivo:* Retirar os objetos do *Change Tracker* economiza memória e processamento.

2.  **Agregações no Banco (Zero N+1)**: Para obter "Quantidade de Dados Temporais" na tela de ReqApi, **NÃO** use `.Include()`, pois isso carrega milhões de linhas na memória RAM apenas para contá-las. Em vez disso, use `CountDadosByReqApiIds()` em batch (uma única query SQL):
    ```csharp
    var counts = reqApiRepository.CountDadosByReqApiIds(reqApis.Select(r => r.Id));
    return reqApis.Select(r => ReqApiResponse.FromDomain(r, counts.GetValueOrDefault(r.Id, 0))).ToList();
    ```
    *Motivo:* Faz o Oracle gerar um `GROUP BY` com `COUNT()` em uma única query, evitando N+1 queries.

3.  **Proibições de Sync-over-Async**: Chamadas HTTP para integrações externas **NUNCA** podem ser bloqueantes (ex: `.GetAwaiter().GetResult()`). Isso causa esgotamento de *Thread Pool* no Kestrel. Use `async/await` ponta a ponta (Controller -> Service -> Integração). O `ReqApiService.CreateAsync` e `ReqApiController.Create` são `async Task<>`.

4.  **ExistsById / ExistsByEmail com `Count()` no Oracle**: O Oracle não aceita `TRUE`/`FALSE` como literais em `CASE WHEN EXISTS ... THEN True ELSE False END`. Substitua `_set.Any(...)` por `_set.Count(...) > 0` nos repositórios base (`Repository.cs`, `ProdutorRepository.cs`) para evitar **ORA-00904: "FALSE": identificador inválido**. **ATENÇÃO:** Isto vale para `ExistsByNome()` e qualquer outro método que use `.Any()` em query LINQ.

5.  **Soma/Contagem Direto no Banco**: Ao validar regras de negócio como "área total dos talhões não excede a propriedade", use `SUM()` e `COUNT()` direto no repositório (ex: `SomarAreaPorPropriedade()`). **NÃO** carregue todos os registros na memória para somar/contar com `.Sum()` ou `.Count()` do LINQ em memória.

6.  **Verificação de Existência por Campo**: Para verificar se uma Localização já está em uso, use `Count() > 0` no repositório (ex: `ExistsByLocalizacaoId()`) em vez de `GetAll().Any()`.

---

## 5. Camada de Validação (`DataAnnotations` Híbridas)

Para validar DTOs (Request records), usamos `ValidationAttribute` customizados dentro do C# para evitar o disparo de "ConstraintViolations" brutos no banco. Os validadores injetam instâncias de Repositório ou HttpClient via `ValidationContext.GetService`:

*   **`[BrasilCoordenadas]`**: Aplicado no `LocalizacaoRequest`. Faz uma chamada HTTP assíncrona para a API `bigdatacloud.net`. Se o país retornado (`countryCode`) não for `BR`, o request é barrado na API. Possui arquitetura "fail-safe" (se a API cair, a validação aprova, evitando travar o sistema).
*   **`[UniqueEmail]`**: Injeta o `IProdutorRepository` e checa se o e-mail já existe, bloqueando na borda.
*   **`[UniqueTelefone]`**: Pode ser aplicado globalmente em propriedades concatenadas (`Ddd` + `Numero`).

---

## 6. Tratamento de Exceções Global

Não há `try/catch` espalhados pelas Controllers para lidar com regras de negócio.
O projeto utiliza um `IExceptionHandler` global registrado no pipeline do `Program.cs` (`GlobalExceptionHandler.cs`).

*   Quando um Serviço lança uma **`InvalidOperationException`** (ex: "Soma de áreas inválida", "Talhão não encontrado"), o Handler a intercepta.
*   A exceção é mapeada de forma limpa para um `HTTP 400 Bad Request` com o formato JSON oficial (`ProblemDetails`), garantindo clareza para o Frontend/Mobile. Exceções de banco (Not Found) são mapeadas para `404`, e exceções do Oracle para `502 Bad Gateway`.

---

## 7. Migrações e Banco de Dados

*   As migrações foram **recriadas do zero** (`Initial`) após limpeza total da pasta `Migrations`.
*   A criação de metadados espaciais (`USER_SDO_GEOM_METADATA`) e índice espacial (`MDSYS.SPATIAL_INDEX_V2`) foi **removida da migration** pois exige privilégios de DBA no Oracle (ORA-00942/ORA-13223). Esses objetos devem ser criados manualmente pelo DBA se necessário.
*   O banco Oracle da FIAP já possui as tabelas criadas via DDL; o EF Core apenas gerencia a tabela `__EFMigrationsHistory`.

---

## 8. Controllers Implementadas

Todas as Controllers possuem CRUD completo (`GET`, `GET by ID`, `POST`, `DELETE`) e endpoints extras de negócio:
*   `ProdutorController` (+ `by-email`)
*   `PropriedadeController` (+ `by-produtor`)
*   `TalhaoController` (+ `by-propriedade`, `by-tipo-plantacao`)
*   `TelefoneController` (+ `by-produtor`)
*   `LocalizacaoController`
*   `TipoPlantacaoController`
*   `TipoApiController` (criado durante esta sessão)
*   `ReqApiController` (+ `talhao/{id}`, `CreateAsync` com integração NASA/SATVeg)
*   `DadoTemporalController` (+ `talhao/{id}`, `req-api/{id}`)
*   `AlertaAgricolaController` (+ `talhao/{id}`, `PATCH /{id}/resolver`, `PATCH /{id}/reabrir`)

---

## 9. Comandos de Teste (Endpoints CRUD) via Terminal

Para testar cada endpoint do projeto via PowerShell, use os comandos abaixo. **IMPORTANTE:** Execute um endpoint por vez e anote o `id` retornado para usar nos próximos passos.

### 9.1. Setup Inicial (criar dados base antes de testar)

```powershell
# 1. Criar Produtor
Invoke-RestMethod -Uri "http://localhost:5160/api/Produtor" -Method Post -ContentType "application/json" -Body '{"nome":"Joao Silva","email":"joao@terranova.com","senha":"123456","telefoneContato":"11999999999"}'
# Salve o id retornado como $produtorId

# 2. Criar Localizacao (latitude/longitude de SP)
Invoke-RestMethod -Uri "http://localhost:5160/api/Localizacao" -Method Post -ContentType "application/json" -Body '{"latitude":-23.5505,"longitude":-46.6333}'
# Salve o id retornado como $localizacaoId

# 3. Criar TipoPlantacao
Invoke-RestMethod -Uri "http://localhost:5160/api/TipoPlantacao" -Method Post -ContentType "application/json" -Body '{"tipoPlant":"Soja"}'
# Salve o id retornado como $tipoPlantacaoId

# 4. Criar TipoApi (NASA POWER)
Invoke-RestMethod -Uri "http://localhost:5160/api/TipoApi" -Method Post -ContentType "application/json" -Body '{"nomeTipoApi":"NASAPOWER"}'
# Salve o id retornado como $tipoApiId

# 5. Criar Propriedade (precisa de produtorId e localizacaoId)
Invoke-RestMethod -Uri "http://localhost:5160/api/Propriedade" -Method Post -ContentType "application/json" -Body '{"nome":"Fazenda Boa Vista","tamanhoTotal":100,"produtorId":"<produtorId>","localizacaoId":"<localizacaoId>"}'
# Salve o id retornado como $propriedadeId

# 6. Criar Talhao (precisa de propriedadeId, tipoPlantacaoId, localizacaoId)
Invoke-RestMethod -Uri "http://localhost:5160/api/Talhao" -Method Post -ContentType "application/json" -Body '{"nomeTalhao":"Talhao 1","volumArea":50,"tipoPlantacaoId":"<tipoPlantacaoId>","propriedadeId":"<propriedadeId>","localizacaoId":"<localizacaoId>"}'
# Salve o id retornado como $talhaoId
```

### 9.2. Testes de Leitura (GET)

```powershell
# Listar todos
Invoke-RestMethod -Uri "http://localhost:5160/api/Produtor" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Propriedade" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Talhao" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Telefone" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Localizacao" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/TipoPlantacao" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/TipoApi" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/ReqApi" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/DadoTemporal" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/AlertaAgricola" -Method Get

# Buscar por ID
Invoke-RestMethod -Uri "http://localhost:5160/api/Produtor/<id>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Talhao/<id>" -Method Get

# Buscar por filtro
Invoke-RestMethod -Uri "http://localhost:5160/api/Produtor/by-email?email=joao@terranova.com" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Propriedade/by-produtor/<produtorId>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Talhao/by-propriedade/<propriedadeId>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Talhao/by-tipo-plantacao/<tipoPlantacaoId>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/Telefone/by-produtor/<produtorId>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/DadoTemporal/talhao/<talhaoId>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/DadoTemporal/req-api/<reqApiId>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/ReqApi/talhao/<talhaoId>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/AlertaAgricola/talhao/<talhaoId>" -Method Get
```

### 9.3. Testes de Integração (POST ReqApi - NASA/SATVEG)

```powershell
# Criar TipoApi NASAPOWER
Invoke-RestMethod -Uri "http://localhost:5160/api/TipoApi" -Method Post -ContentType "application/json" -Body '{"nomeTipoApi":"NASAPOWER"}'

# Criar TipoApi SATVEG
Invoke-RestMethod -Uri "http://localhost:5160/api/TipoApi" -Method Post -ContentType "application/json" -Body '{"nomeTipoApi":"SATVEG"}'

# Integrar dados da NASA (tipoParam 0=NDVI, 1=PRECTOTCORR)
# Precisa de um TipoApiId válido e um TalhaoId válido
Invoke-RestMethod -Uri "http://localhost:5160/api/ReqApi" -Method Post -ContentType "application/json" -Body '{"tipoParam":1,"tipoApiId":"<tipoApiId>","talhaoId":"<talhaoId>"}'
# Retorna: id, tipoParam, dataAnalise, tipoApiId, totalDadosSalvos (quantidade de registros baixados)

# Verificar dados integrados
Invoke-RestMethod -Uri "http://localhost:5160/api/DadoTemporal/talhao/<talhaoId>" -Method Get
Invoke-RestMethod -Uri "http://localhost:5160/api/ReqApi/talhao/<talhaoId>" -Method Get
```

### 9.4. Testes de Alerta (PATCH resolver/reabrir)

```powershell
# Criar alerta
Invoke-RestMethod -Uri "http://localhost:5160/api/AlertaAgricola" -Method Post -ContentType "application/json" -Body '{"titulo":"Alerta Seca","descricao":"Alerta de baixa umidade","nivelAlerta":2,"talhaoId":"<talhaoId>"}'

# Resolver alerta
Invoke-RestMethod -Uri "http://localhost:5160/api/AlertaAgricola/<alertaId>/resolver" -Method Patch
# Retorna: resolvido: true

# Reabrir alerta
Invoke-RestMethod -Uri "http://localhost:5160/api/AlertaAgricola/<alertaId>/reabrir" -Method Patch
# Retorna: resolvido: false
```

### 9.5. Testes de Validação (deve retornar 400 Bad Request)

```powershell
# Email duplicado (UniqueEmail) — deve retornar 400
Invoke-RestMethod -Uri "http://localhost:5160/api/Produtor" -Method Post -ContentType "application/json" -Body '{"nome":"Duplo","email":"joao@terranova.com","senha":"123456","telefoneContato":"11888888888"}'

# Telefone duplicado (UniqueTelefone) — deve retornar 400
Invoke-RestMethod -Uri "http://localhost:5160/api/Produtor" -Method Post -ContentType "application/json" -Body '{"nome":"Duplo2","email":"outro@email.com","senha":"123456","telefoneContato":"11999999999"}'

# Talhao com area que excede propriedade — deve retornar 400
Invoke-RestMethod -Uri "http://localhost:5160/api/Talhao" -Method Post -ContentType "application/json" -Body '{"nomeTalhao":"Grande Demais","volumArea":200,"tipoPlantacaoId":"<tipoPlantacaoId>","propriedadeId":"<propriedadeId>","localizacaoId":"<localizacaoId>"}'
```

### 9.6. Comandos de Migração e Ambiente

```powershell
# Configurar connection string via User Secrets
cd TerraNova\TerraNova.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:TerraNovaOracle" "User Id=RMxxxxxx;Password=xxxxxx;Data Source=oracle.fiap.com.br:1521/orcl;"

# Voltar para a raiz do solution
cd ..

# Limpar e recriar migrations (se necessário)
Remove-Item -Recurse -Force TerraNova\TerraNova.Infrastructure\Migrations
cd TerraNova
dotnet ef migrations add Initial --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API

# Rodar migrations no banco
dotnet ef database update --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API

# Drop completo do banco (se necessário)
dotnet ef database drop --force --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API

# Rodar a API
dotnet run --project .\TerraNova.API
```

---

### TL;DR para o Próximo Agente:
Se você foi chamado para dar manutenção, siga este fluxo:
1. Precisa de uma nova tabela? Crie a `Entity`, a configure no `TerraNovaContext` e crie seu respectivo DTO na `Application`.
2. O DTO de Request tem regras exóticas? Crie um `ValidationAttribute` na pasta `Validators`.
3. A regra de negócio envolve validar limites ou estados? Coloque o bloco lógico no `Service` e dispare `InvalidOperationException` se algo ferir o negócio.
4. Vai fazer queries? Use `.AsNoTracking()` e filtre com LINQ diretamente. Não carregue coleções enormes se só precisa contar ou verificar existência (`.Any()`). Use `Count()` no repositório para agregações.
5. Precisa consultar pai por condição no filho? Use `.Where(p => p.Filhos.Any(f => f.Condicao))` para gerar `EXISTS` no SQL.
6. Oracle não aceita boolean literals em `CASE WHEN` — use `Count() > 0` em vez de `Any()`.
7. Integrações externas: **sempre `async/await`**, nunca `.Result` ou `.GetAwaiter().GetResult()`.

---

## 10. Histórico de Mudanças e Correções Críticas (Sessão Recente)

Esta seção documenta **TODAS** as correções e ajustes feitos durante a sessão de integração com APIs externas (NASA POWER e Embrapa SATVeg). **LEIA ANTES DE TOCAR NO CÓDIGO RELACIONADO A INTEGRAÇÃO.**

### 10.1. Bug de Cultura pt-BR (Coordenadas com Vírgula)

**Problema:** No Windows/BR, `decimal`/`double` em interpolação de string usa **vírgula** como separador decimal (`-23,3849`). A NASA POWER **rejeita** com HTTP 422 (Unprocessable Entity) porque a API espera **ponto** (`-23.3849`).

**Sintoma:** Logs mostravam `422` e `EnsureSuccessStatusCode` estourava `HttpRequestException`.

**Solução em `ReqApiService.BuscarDadosNasaPower()`:** Formatar coordenadas com `CultureInfo.InvariantCulture` **antes** de montar a URL. **NÃO** passar `decimal`/`double` direto na interpolação.

```csharp
// ERRADO (gera vírgula em pt-BR):
var url = $"?latitude={latitude}&longitude={longitude}...";

// CERTO (sempre ponto decimal):
var lat = talhao.Localizacao!.Coordenadas.Y.ToString("0.0000", CultureInfo.InvariantCulture);
var lon = talhao.Localizacao.Coordenadas.X.ToString("0.0000", CultureInfo.InvariantCulture);
var url = $"?latitude={lat}&longitude={lon}...";
```

**Convenção do projeto:** O usuário **sempre** envia coordenadas com **PONTO**. Se vier vírgula, é problema na origem (mobile/frontend) — **NÃO** aceitamos vírgula no backend.

### 10.2. Bug do Alerta Automático (SELECT não commitado)

**Problema:** `AnalisarEGerarAlertas()` fazia `dadoTemporalRepository.GetByTalhaoId(talhaoId)` para buscar os dados recém-inseridos. **MAS** os dados haviam sido adicionados via `AddRange()` (sem `SaveChanges`), ou seja, ainda estavam no Change Tracker. O SELECT retornava **0 registros** e a função saía sem criar alertas.

**Sintoma:** Dados eram salvos corretamente (`totalDadosSalvos: 2348`) mas **nenhum alerta era gerado**.

**Solução:** Passar a lista `dados` **em memória** (já populada) diretamente para a função de análise. Não re-consultar o banco.

```csharp
// ERRADO (busca no banco, dados não foram commitados):
AnalisarEGerarAlertas(talhao.Id, "NASA POWER");

// CERTO (usa lista já em memória):
AnalisarEGerarAlertas(talhao.Id, dados, "NASA POWER");
```

A função agora recebe `List<DadoTemporal> dadosRecentes` como parâmetro e filtra diretamente da lista.

### 10.3. Thresholds de Alerta (Regras de Negócio)

**NASA POWER (`AnalisarEGerarAlertas` quando `tipoApiNome == "NASA POWER"`):**
- Chuva acumulada 3 dias > 80mm → **ALERTA ALTO** (Risco de Alagamento)
- Chuva acumulada 15 dias < 10mm → **ALERTA CRÍTICO** (Seca Severa)
- Chuva acumulada 15 dias < 25mm → **ALERTA MÉDIO** (Estresse Hídrico)

**SATVEG (`AnalisarEGerarAlertas` quando `tipoApiNome == "SATVEG"`):**
- Último NDVI < 0.2 → **ALERTA CRÍTICO** (Anomalia Vegetativa Severa)
- Último NDVI < 0.4 → **ALERTA MÉDIO** (Baixo Vigor Vegetativo)

**Importante:** Se o valor estiver saudável (acima dos thresholds), **NÃO** gera alerta. É comportamento correto. Para forçar teste de alerta, use um talhão com coordenadas em região de baixa precipitação/baixa vegetação.

### 10.4. Filtros de Validação de Dados Externos

**NASA POWER:**
- Datas hardcoded: `dataInicio = "20200101"`, `dataFim = DateTime.UtcNow.ToString("yyyyMMdd")`
- Filtro: Ignorar valores `<= -900.0` (sensor com defeito)
- Normalização: `"YYYYMMDD"` (da NASA) → `"YYYY-MM-DD"` (para `DateTime.TryParse`)

**SATVEG:**
- Filtro temporal: Apenas datas `>= "2020-01-01"`
- Tratamento de erro: HTTP 400 (coordenada no oceano) → `InvalidOperationException` com mensagem amigável

### 10.5. SatVeg Token Configuration

Token da Embrapa configurado em `appsettings.json` na chave `SatVegApiToken` (valor default hardcoded em `ReqApiService` se não existir):
```json
{
  "SatVegApiToken": "Bearer e97dab05-eedc-39b9-a3fd-fa83cb5fef5e"
}
```

### 10.6. Testes e Comportamento Esperado

1. **NASA POWER** com talhão em região **seca** (ex: interior BA) → gera alerta de Seca/Estresse Hídrico
2. **SATVEG** com talhão em região com **baixa vegetação** (ex: semi-árido) → gera alerta de NDVI baixo

Talentos em regiões saudáveis (SP capital, por ex.) **NÃO** geram alertas mesmo após integração bem-sucedida. Isso é o comportamento esperado.

### 10.7. Coordenadas Geográficas (Importante)

No Java, o `latitude` e `longitude` vêm como `BigDecimal` (numérico). No .NET, o DTO `LocalizacaoRequest` recebe como `decimal` e o backend faz a conversão para `Point` do NetTopologySuite (SRID 4326) usando `NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326).CreatePoint(new Coordinate(longitude, latitude))`.

**Cuidado com a ordem:** WKT/Point usa `(X=longitude, Y=latitude)`. O DTO do `LocalizacaoRequest` já está esperando `{latitude, longitude}` e o mapper faz a troca internamente.
