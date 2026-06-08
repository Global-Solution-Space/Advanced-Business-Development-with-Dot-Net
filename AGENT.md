# AGENT.md - Terra Nova API (.NET)

> Fonte da verdade para agentes trabalhando neste checkout. Antes de alterar codigo ou README, leia este arquivo e confirme a rota/DTO real no projeto.

---

## 1. Contexto do Projeto

- Projeto: **Terra Nova API** para a **Global Solution FIAP**.
- Stack: .NET, ASP.NET Core, EF Core, Oracle, NetTopologySuite, Swagger.
- Solucao: `TerraNova/TerraNova.sln`.
- Arquitetura: Clean Architecture com camadas `Domain`, `Application`, `Infrastructure`, `Integration` e `API`.
- Regra de documentacao: preservar o termo **Global Solution** e o vocabulario Terra Nova (`Produtor`, `Propriedade`, `Talhao`, `ReqApi`, `DadoTemporal`, `AlertaAgricola`).

Fluxo base:

```text
Controller -> Service -> Repository -> Entity
```

---

## 2. Estrutura das Camadas

| Camada | Responsabilidade | Regra |
|---|---|---|
| `TerraNova.Domain` | Entidades, enums e `DomainException` | Nao depende de EF, HTTP ou banco |
| `TerraNova.Application` | DTOs, interfaces e services | Orquestra casos de uso |
| `TerraNova.Infrastructure` | `TerraNovaContext`, repositories, configurations e migrations | Acesso Oracle/EF Core |
| `TerraNova.Integration` | Clientes NASA POWER e SATVeg | HTTP externo, sem acesso ao banco |
| `TerraNova.API` | Controllers, Swagger, DI e exception handler | Expor DTOs, nunca entities |

---

## 3. Modelo de Dados

Todas as entidades herdam de `BaseEntity` e usam `Guid Id`.

| Entidade | Dependencias / relacoes | Observacoes |
|---|---|---|
| `TipoPlantacao` | Nenhuma | Tabela raiz para culturas |
| `Localizacao` | Nenhuma | Usa `Point`/SDO_GEOMETRY com SRID 4326 |
| `Produtor` | Pode criar/atualizar `Telefone` principal via `telefoneContato` | Email unico; senha minima |
| `Telefone` | Depende de `Produtor` | Um telefone por produtor; `DDD + Numero` unico |
| `Propriedade` | Depende de `Produtor` + `Localizacao` | `Localizacao` exclusiva |
| `Talhao` | Depende de `TipoPlantacao` + `Propriedade` + `Localizacao` | Area total dos talhoes nao pode passar da propriedade |
| `TipoApi` | Nenhuma | Tabela raiz para APIs externas, ex.: `NASAPOWER`, `SATVEG` |
| `ReqApi` | Depende de `TipoApi`; ligado ao talhao pelos dados gerados | Cabecalho da integracao externa |
| `DadoTemporal` | Depende de `Talhao` + `ReqApi` | Read-only pela API; criado pelo fluxo de integracao |
| `AlertaAgricola` | Depende de `Talhao` | Manual ou automatico; possui resolver/reabrir |

Cascade importante: remover `Talhao` remove `DadoTemporal` e `AlertaAgricola` associados; remover `ReqApi` tambem remove os `DadoTemporal` associados a essa requisicao.

---

## 4. Endpoints Reais

Os controllers usam `[Route("api/[controller]")]`. O ASP.NET aceita variacao de caixa, mas o README deve preferir rotas em minusculo.

| Entidade | Endpoint base no README | Escrita? | Extras |
|---|---|---|---|
| `TipoPlantacao` | `/api/tipoplantacao` | CRUD completo | - |
| `Localizacao` | `/api/localizacao` | CRUD completo | - |
| `Produtor` | `/api/produtor` | CRUD completo | `GET /by-email?email=` |
| `Telefone` | `/api/telefone` | CRUD completo | `GET /by-produtor/{produtorId}` |
| `Propriedade` | `/api/propriedade` | CRUD completo | `GET /by-produtor/{produtorId}` |
| `Talhao` | `/api/talhao` | CRUD completo | `GET /by-propriedade/{propriedadeId}`, `GET /by-tipo-plantacao/{tipoPlantacaoId}` |
| `TipoApi` | `/api/tipoapi` | CRUD completo | - |
| `ReqApi` | `/api/reqapi` | `GET`, `POST`, `DELETE`; sem `PUT` | `GET /talhao/{talhaoId}` |
| `DadoTemporal` | `/api/dadotemporal` | Somente leitura | `GET /talhao/{talhaoId}`, `GET /req-api/{reqApiId}` |
| `AlertaAgricola` | `/api/alertaagricola` | CRUD + ciclo de vida | `GET /talhao/{talhaoId}`, `PATCH /{id}/resolver`, `PATCH /{id}/reabrir` |

Atencao: se o README tiver `/api/dadostemporal`, conferir o controller antes de manter. O controller atual e `DadoTemporalController`, portanto a rota base natural e `/api/dadotemporal`.

---

## 5. Verificacao Contra o README Atual

Quando o usuario pedir para conferir o projeto "de acordo com o README", validar estes pontos antes de afirmar que esta consistente:

1. Ler `README.md`, principalmente "Documentacao de Rotas" e "Comandos CRUD".
2. Comparar rotas com `TerraNova/TerraNova.API/Controllers/*Controller.cs`.
3. Comparar payloads com `TerraNova/TerraNova.Application/DTOs/*Request.cs`.
4. Comparar valores numericos de enum com `TerraNova/TerraNova.Domain/Enums`.
5. Conferir se exemplos que usam `jq` capturam `.id // .Id`.
6. Conferir a ordem de criacao por FK e a ordem inversa nos deletes.

Resultado da verificacao do README atual:

- Os blocos novos de `TipoApi`, `ReqApi`, `AlertaAgricola` e `DadoTemporal` batem com os controllers quanto a superficie principal.
- `ReqApi` esta correto sem `PUT`.
- `DadoTemporal` esta correto como somente leitura nos cURLs.
- O README atual nao tem bloco cURL dedicado para `Telefone`; isso e aceitavel porque o fluxo de `Produtor` ja cria/atualiza o telefone principal via `telefoneContato`, e a tabela de endpoints documenta `/api/telefone`.
- A numeracao dos cURLs pula do bloco 5 para o 7 por causa da ausencia do bloco dedicado de `Telefone`; nao tratar isso como bug funcional se o usuario pediu somente as quatro entidades finais.
- A tabela de endpoints do README ainda pode mostrar `/api/dadostemporal`, mas o controller e os cURLs atuais usam `/api/dadotemporal`. Se o pedido for corrigir README, trocar a tabela para `/api/dadotemporal`.
- O exemplo de `TipoApiRequest` deve respeitar `[StringLength(10)]`; `NASA POWER` tem 10 caracteres incluindo o espaco e cabe no limite.
- O comentario do README pode chamar `tipoParam: 0 = NVDI/SATVEG`; o enum real se chama `Nvdi = 0`. Preserve o valor numerico e, se ajustar texto, prefira `NDVI/SATVEG` para clareza de dominio.

---

## 6. Ordem dos cURLs no README

Quando atualizar a secao de comandos CRUD do `README.md`, manter a ordem por dependencias:

1. `TipoPlantacao` - sem dependencia.
2. `Localizacao` - sem dependencia.
3. `Produtor` - cria produtor e telefone principal pelo `telefoneContato`.
4. `Propriedade` - depende de `Produtor` + `Localizacao`.
5. `Talhao` - depende de `TipoPlantacao` + `Propriedade` + `Localizacao`.
6. `Telefone` - depende de `Produtor`; endpoint existe, mas no README atual nao ha bloco cURL dedicado porque o produtor cobre o telefone principal.
7. `TipoApi` - sem dependencia.
8. `ReqApi` - depende de `TipoApi` + `Talhao`; dispara integracao externa.
9. `AlertaAgricola` - depende de `Talhao`.
10. `DadoTemporal` - depende de `Talhao` + `ReqApi`; somente leitura, validar dados gerados pela integracao.
11. Deletes - sempre apagar dependentes antes das tabelas raiz.

Para o README, os quatro blocos que costumam faltar sao:

- `TipoApi`: `POST`, `GET all`, `GET by id`, `PUT`, `DELETE`.
- `ReqApi`: `POST`, `GET all`, `GET by id`, `GET talhao`, `DELETE`; nao documentar `PUT`.
- `AlertaAgricola`: `POST`, `GET all`, `GET by id`, `GET talhao`, `PUT`, `PATCH resolver`, `PATCH reabrir`, `DELETE`.
- `DadoTemporal`: `GET all`, `GET by id`, `GET talhao`, `GET req-api`; nao documentar `POST`, `PUT` ou `DELETE`.

Use `jq` para capturar IDs em Bash:

```bash
ID=$(curl -fsS -X POST "$API_URL/api/recurso" \
  -H "Content-Type: application/json" \
  -d '{ "...": "..." }' | jq -r '.id // .Id')
```

---

## 7. DTOs e Payloads Importantes

| DTO | Campos |
|---|---|
| `TipoApiRequest` | `nomeTipoApi` (`string`, 2 a 10 chars) |
| `ReqApiRequest` | `tipoParam`, `tipoApiId`, `talhaoId` |
| `AlertaAgricolaRequest` | `titulo`, `descricao`, `nivelAlerta`, `talhaoId` |
| `DadoTemporalResponse` | `id`, `dataLeitura`, `valor`, `talhaoId`, `reqApiId` |

Enums relevantes:

| Valor JSON | `TipoParamReqApi` | Uso |
|---|---|---|
| `0` | `Nvdi` | SATVeg / NDVI |
| `1` | `Prectotcorr` | NASA POWER / chuva |

| Valor JSON | `NivelAlerta` esperado | Uso comum |
|---|---|---|
| `0` | Baixo | Baixa severidade |
| `1` | Medio | Monitoramento |
| `2` | Alto | Risco elevado |
| `3` | Critico | Risco critico |

Sempre confirmar os nomes exatos nos enums antes de alterar payloads, porque os exemplos do README dependem do binding do ASP.NET.

---

## 8. Regras de Implementacao

- Nao criar DTO de update separado se o projeto ja reutiliza o request atual.
- Reusar os metodos ricos das entidades, como `Atualizar(...)`, `Resolver()` e `Reabrir()`, quando existirem.
- Em GETs, preferir `AsNoTracking()`.
- Filtros devem ir para repository especializado. Nao usar `GetAll().Where(...)` em memoria.
- Para existencia no Oracle, preferir `Count(...) > 0` quando o padrao local ja usa isso para evitar problemas de boolean literal.
- Integracoes externas devem continuar `async/await` ponta a ponta.
- Coordenadas para NASA POWER devem usar `CultureInfo.InvariantCulture`.
- Controllers nao devem ter `try/catch` de regra de negocio; o projeto usa `GlobalExceptionHandler`.

---

## 9. Fluxo de Integracao `ReqApi`

`POST /api/reqapi`:

1. Valida `TipoApiId`.
2. Busca `Talhao` com `Localizacao`.
3. Cria `ReqApi`.
4. Dispara SATVeg quando `tipoParam = 0`.
5. Dispara NASA POWER quando `tipoParam = 1`.
6. Persiste `DadoTemporal` em lote.
7. Gera alertas automaticos sem reconsultar o banco.
8. Retorna `ReqApiResponse` com contagem de dados salvos.

SATVeg usa `SatVegApiToken` em `appsettings.json`, com fallback no service. NASA POWER usa latitude/longitude do talhao.

---

## 10. Validacao e Excecoes

Validadores customizados ficam em `TerraNova.Application/DTOs/Validators`.

| Validador | Uso |
|---|---|
| `BrasilCoordenadasAttribute` | `LocalizacaoRequest` |
| `UniqueEmailAttribute` | `ProdutorRequest.Email` |
| `UniqueTelefoneAttribute` | `ProdutorRequest.TelefoneContato` e `TelefoneRequest` |

`GlobalExceptionHandler` converte:

| Excecao | HTTP |
|---|---|
| `DomainException` | 400 |
| `InvalidOperationException` | 400 |
| `ArgumentException` / `ArgumentNullException` | 400 |
| `KeyNotFoundException` | 404 |
| `UnauthorizedAccessException` | 401 |
| `OracleException` | 502 |
| Demais excecoes | 500 |

---

## 11. Auditoria Oracle de Alta Prioridade

Use esta secao quando revisar o projeto para producao Oracle. Nem todo item abaixo e bug de codigo; alguns sao conferencias obrigatorias entre EF Core, migration gerada e DDL realmente aplicado no banco.

### 11.1 Mitigacoes Confirmadas no Codigo

- `AlertaAgricola.Resolvido` esta protegido contra truncamento em `CHAR(1)`: `AlertaAgricolaConfiguration` converte `true/false` para `"1"`/`"0"`.
- `AlertaAgricola.NivelAlerta` persiste como string em caixa alta com `ToUpperInvariant()` e parse case-insensitive.
- `ReqApi.TipoParam` persiste como string em caixa alta com `ToUpperInvariant()`, respeitando o CHECK esperado (`NVDI`, `PRECTOTCORR`).
- `ReqApi -> DadoTemporal` esta mapeado com `DeleteBehavior.Cascade` em `ReqApiConfiguration`.
- `Talhao -> DadoTemporal` e `Talhao -> AlertaAgricola` tambem estao com cascade em `TalhaoConfiguration`.
- `ProdutorRequest.TelefoneContato` ja valida 10 a 11 digitos com `[RegularExpression]` antes do service extrair DDD e numero.
- `TelefoneRequest` e a entidade `Telefone` exigem DDD e numero numericos, bloqueando padding silencioso em `NCHAR(2)`.
- `ProdutorService.ExtrairTelefone` valida o tamanho normalizado antes de fatiar DDD e numero.
- `AlertaAgricola.Atualizar(...)` centraliza validacao de titulo, descricao, nivel e talhao; o update preserva `Resolvido` e `DataAlerta`.

### 11.2 Itens que Devem Ser Auditados no Banco

1. Gerar e revisar o script EF:

```powershell
dotnet ef migrations script --idempotent --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API
```

2. Comparar o script com o DDL aplicado em producao, principalmente FKs com `ON DELETE CASCADE`.
3. Confirmar que a FK `dado_temporal -> req_api` possui cascade no banco real. Se nao possuir, `DELETE /api/reqapi/{id}` pode falhar com `ORA-02292`.
4. Confirmar se os scripts espaciais foram executados manualmente:
   - `INSERT INTO USER_SDO_GEOM_METADATA` para a coluna espacial de `localizacao`.
   - Criacao do indice espacial `MDSYS.SPATIAL_INDEX` / `MDSYS.SPATIAL_INDEX_V2`.
5. Conferir se as constraints de enum no Oracle estao coerentes com as conversoes EF.

### 11.3 Dividas Tecnicas Conhecidas

- O enum `TipoParamReqApi` usa `Nvdi`, e o banco espera `NVDI`. Corrigir para `NDVI` exige roteiro completo: alterar codigo, criar migration, executar `UPDATE` dos registros legados, e recriar a CHECK constraint no Oracle.
- O fluxo de produtor ainda aceita `telefoneContato` como string unica por compatibilidade com o README/API atual. Uma melhoria opcional seria receber `ddd` e `numero` separados tambem nesse payload.

---

## 12. Banco, Secrets e Execucao

Rodar na raiz `TerraNova/` quando usar EF:

```powershell
dotnet ef database update --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API
dotnet ef database update 0 --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API
dotnet run --project .\TerraNova.API
```

Secrets locais:

```powershell
cd TerraNova\TerraNova.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:TerraNovaOracle" "User Id=RMxxxxxx;Password=xxxxxx;Data Source=oracle.fiap.com.br:1521/orcl;"
```

Docker/Azure:

- O README atual usa `docker compose up -d --build` e API em `http://localhost:8080`.
- Substituir por `$PUBLIC_IP:8080` quando rodar na VM Azure.
- Confirmar `azure-cli-script.sh` antes de documentar qualquer passo de deploy.

---

## 13. Checklist Antes de Finalizar

1. Conferir `git status --short` e nao reverter mudancas do usuario.
2. Conferir controller + DTO antes de escrever cURL.
3. Manter rotas do README em minusculo.
4. Nao documentar escrita para `DadoTemporal`.
5. Nao documentar `PUT` para `ReqApi`.
6. Se o README estiver sendo auditado, registrar divergencias entre README e projeto real, principalmente `/api/dadotemporal` versus `/api/dadostemporal`.
7. Se for auditoria Oracle, verificar a secao "Auditoria Oracle de Alta Prioridade".
8. Se mexer em codigo, tentar `dotnet build TerraNova/TerraNova.sln` quando o ambiente permitir.
9. Se mexer so em docs, validar por leitura contra controllers/DTOs.
