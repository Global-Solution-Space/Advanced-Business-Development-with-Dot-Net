# 🌍 Terra Nova API

> **Global Solution FIAP - Advanced Business Development with .NET**
> 
> API REST em .NET 10 para **monitoramento agrícola inteligente**, focada em centralizar o gerenciamento de propriedades rurais e talhões, coletar e armazenar dados de inteligência climática (NASA POWER) e índices de vegetação (Embrapa SATVeg) por meio de integrações externas, gerando insights e séries temporais georreferenciadas.

---

## 🛠️ Tecnologias & Badges

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white&style=for-the-badge)
![Entity Framework](https://img.shields.io/badge/EF%20Core-10.0-512BD4?logo=nuget&logoColor=white&style=for-the-badge)
![NetTopologySuite](https://img.shields.io/badge/Spatial-NetTopologySuite-1E8C3A?style=for-the-badge)
![Oracle Database](https://img.shields.io/badge/Oracle-23c%20%2F%2019c-F80000?logo=oracle&logoColor=white&style=for-the-badge)

---

## Repositório Github | Apresentação em Vídeo

[Repositório Github](https://github.com/Challenge-Terra-Nova/Advanced-Business-Development-with-Dot-Net) | [Vídeo de Demonstração (YouTube)](#)

---

## 👥 Integrantes

<table>
<tr>
<th>Nome</th>
<th>RM</th>
<th>Turma</th>
<th>GitHub</th>
<th>LinkedIn</th>
</tr>

<tr>
<td>Enzo Okuizumi</td>
<td>561432</td>
<td>2TDSPG</td>
<td><a href="https://github.com/EnzoOkuizumiFiap">EnzoOkuizumiFiap</a></td>
<td><a href="https://www.linkedin.com/in/enzo-okuizumi-b60292256/">Enzo Okuizumi</a></td>
</tr>

<tr>
<td>Lucas Barros Gouveia</td>
<td>566422</td>
<td>2TDSPG</td>
<td><a href="https://github.com/LuzBGouveia">LuzBGouveia</a></td>
<td><a href="https://www.linkedin.com/in/lucas-barros-gouveia-09b147355/">Lucas Barros Gouveia</a></td>
</tr>

<tr>
<td>Milton Marcelino</td>
<td>564836</td>
<td>2TDSPG</td>
<td><a href="https://github.com/MiltonMarcelino">MiltonMarcelino</a></td>
<td><a href="http://linkedin.com/in/milton-marcelino-250298142">Milton Marcelino</a></td>
</tr>

<tr>
<td>Luna de Carvalho Guimarães</td>
<td>562290</td>
<td>2TDSPG</td>
<td><a href="https://github.com/lunaguima">lunaguima</a></td>
<td><a href="https://www.linkedin.com/in/luna-m-guimar%C3%A3es-1850ab173/">Luna M. Guimarães</a></td>
</tr>

<tr>
<td>Gustavo Okada</td>
<td>563428</td>
<td>2TDSPG</td>
<td><a href="https://github.com/Gdev3356">GustavoOkada7268</a></td>
<td><a href="https://www.linkedin.com/in/gustavo-okada-53a3b8359/">Gustavo Okada</a></td>
</tr>

</table>

---

## 🏛️ Arquitetura e Boas Práticas

### Como o projeto está organizado?
A aplicação segue os princípios da **Clean Architecture** (Arquitetura Limpa) e **Domain-Driven Design (DDD)**, dividida em quatro camadas principais:
1. **TerraNova.Domain:** Contém as entidades centrais (`Produtor`, `Talhao`, `Localizacao`), validações de domínio (`DomainException`) e lógicas puras, sem dependência de tecnologia.
2. **TerraNova.Application:** Orquestra os casos de uso. Contém DTOs (Data Transfer Objects), Contratos (Interfaces) de Serviços e Repositórios, blindando o domínio contra alterações na API.
3. **TerraNova.Infrastructure:** Gerencia o acesso a dados (EF Core + Oracle). Implementa o padrão *Repository* e contém as Migrations do banco.
4. **TerraNova.Integration:** Isola os clientes HTTP (`HttpClient`) responsáveis por buscar dados externos — NASA POWER (precipitação) e Embrapa SATVeg (NDVI). Garante que detalhes de integração nunca vazem para camadas superiores.
5. **TerraNova.API:** Camada de apresentação e roteamento (Controllers), injeção de dependências e configuração do Swagger.

### Por que escolheram essa arquitetura?
A Clean Architecture permite o princípio da **Inversão de Dependência**. Se futuramente precisarmos trocar o banco de dados Oracle por PostgreSQL ou substituir a API do SATVeg por outro provedor geoespacial, só precisaremos mexer na camada de **Infrastructure**. O núcleo da regra de negócio (Domain e Application) permanece intacto e perfeitamente isolado.

### Como adicionar uma nova funcionalidade?
O fluxo de desenvolvimento de uma nova funcionalidade seria:
1. **Domain:** Criar/alterar a classe de domínio, garantindo o encapsulamento.
2. **Infrastructure:** Atualizar ou criar o `Configuration.cs` para mapear a tabela no EF Core e gerar uma *Migration*. Implementar o repositório se necessário.
3. **Application:** Criar os DTOs (`Request` e `Response`), a Interface de Serviço e a sua respectiva implementação (caso de uso).
4. **API:** Criar o novo *Controller*, injetar o serviço correspondente e expor as rotas via HTTP, configurando *status codes* (200, 201, 400).

---

## 🗄️ Banco de Dados e Relacionamentos

### Modelagem do Banco de Dados
A modelagem foi concebida utilizando o **Entity Framework Core**, com a abordagem *Code-First*. A grande inovação de viabilidade desta modelagem é o suporte oficial aos **Tipos Geoespaciais (SDO_GEOMETRY)** do Oracle Database usando `NetTopologySuite`.

### Modelo Lógico
![Modelo Lógico](./docs/Logical.png)

### Modelo Relacional
![Modelo Relacional](./docs/Relational.png)

### Relacionamentos Implementados
- **Produtor ↔ Propriedade (1:N):** Um produtor pode possuir várias propriedades, o que nos permite segmentar a visão de gestão agrícola.
- **Propriedade ↔ Localizacao / Talhao ↔ Localizacao (1:1):** O uso do vínculo `UNIQUE INDEX` na chave estrangeira de `Localizacao` possibilita que cada pedaço de terra e cada talhão de cultura tenham suas coordenadas espaciais georreferenciadas armazenadas isoladamente.
- **Talhao ↔ DadoTemporal (1:N):** As séries e dados vitais puxados de APIs (como precipitação e índice NDVI) são atrelados fortemente aos talhões para possibilitar gráficos de rendimento.

### O que acontece ao deletar um registro?
Foi implementada a diretiva de deleção em cascata (`DeleteBehavior.Cascade`) para dados dependentes diretos. Por exemplo, ao deletar um `Talhao`, todos os seus respectivos `AlertaAgricola` e `DadoTemporal` são varridos automaticamente, garantindo que não haja registros órfãos no banco. Associações mais estruturais (como `Propriedade` e `Localizacao`) usam `Restrict` para impedir deleções acidentais na integridade do território.

---

## 🗂️ Migrations e Evolução

### O que são migrations e qual o papel delas?
*Migrations* são scripts evolutivos gerados pelo EF Core que versionam o banco de dados via código C#. Em vez de escrevermos `CREATE TABLE` manualmente em SQL, nós estruturamos o estado do sistema na camada de infraestrutura e o EF calcula as diferenças.

### Como gerenciar mudanças ao longo do tempo?
Toda alteração de banco deve passar pelo fluxo:
1. Edição da Entidade ou de sua classe `Configuration.cs`.
2. Execução de `dotnet ef migrations add NomeDaAlteracao`.
3. Execução de `dotnet ef database update` para aplicar no Oracle.

*Nota de destaque:* O EF Core gerencia a tabela `__EFMigrationsHistory`. A criação de metadados espaciais (`USER_SDO_GEOM_METADATA`) e do índice espacial (`MDSYS.SPATIAL_INDEX_V2`) exige privilégios de DBA no Oracle e deve ser executada manualmente pelo administrador do banco. O mapeamento da coluna `SDO_GEOMETRY` é feito via `NetTopologySuite`, com SRID 4326 (WGS 84).

---

## 🚦 Tratamento de Entradas Inválidas e Testes

### Como a aplicação trata entradas inválidas?
O projeto utiliza um pipeline duplo de validação:
1. **Validação de Sintaxe e Limites (DTOs):** Os Requests (como `LocalizacaoRequest.cs`) utilizam anotações do tipo `[Range(-90.0, 90.0)]` e `[Required]`. Se violado, o ASP.NET Core automaticamente interrompe o fluxo e retorna um `400 Bad Request` sem nem encostar no serviço.
2. **Validação de Negócio (Domain):** Entidades como `Localizacao` não têm *setters* abertos. Eles são inicializados por construtores blindados que jogam uma `DomainException` caso alguém tente cadastrar uma coordenada geograficamente impossível.

### Testes da API (Insomnia / Swagger)
As rotas da API foram intensamente testadas. Graças à blindagem no DTO (`LocalizacaoRequest`), a nossa API recebe os campos JSON normais `latitude` e `longitude` enviados por clientes externos, e o nosso Mapper converte isso para um Ponto Geoespacial (`NetTopologySuite.Point`) transparente para o banco de dados, sem o consumidor (Frontend/Mobile) precisar saber WKT.
O teste final pode ser realizado rodando a API e acessando o `/swagger` gerado para realizar inserts nos endpoints.

---

## 🚀 Como Executar o Projeto

1. **Configuração de Secrets (Banco Oracle FIAP):**
   No terminal, na raiz do projeto `TerraNova\TerraNova.API`, rode para armazenar suas credenciais com segurança:
   ```powershell
   cd TerraNova\TerraNova.API
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:TerraNovaOracle" "User Id=RMxxxxxx;Password=xxxxxx;Data Source=oracle.fiap.com.br:1521/orcl;"
   ```

2. **Atualização do Banco de Dados (Gerar tabelas e schemas espaciais):**
   ```powershell
   dotnet ef database update --project .\TerraNova.Infrastructure --startup-project .\TerraNova.API
   ```

3. **Execução Local:**
   ```powershell
   dotnet run --project .\TerraNova\TerraNova.API
   ```
   Acesse a URL (ex: `http://localhost:5160/swagger`) fornecida no terminal para visualizar os endpoints ativos e testar a aplicação de ponta a ponta.

---

## 📌 Documentação de Rotas (Endpoints Atuais)

> As rotas podem ser integralmente visualizadas e operadas na UI do **Swagger**. Segue estrutura base de rotas operacionais (CRUD e integrações):

### Produtores
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/produtor` | Listar todos os produtores |
| GET | `/api/produtor/{id}` | Buscar produtor por ID |
| GET | `/api/produtor/by-email` | Buscar produtor pelo seu e-mail |
| POST | `/api/produtor` | Cadastrar novo produtor |
| DELETE | `/api/produtor/{id}` | Remover um produtor |

### Propriedades
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/propriedade` | Listar propriedades gerenciadas |
| GET | `/api/propriedade/{id}` | Buscar propriedade por ID |
| GET | `/api/propriedade/by-produtor/{produtorId}` | Listar propriedades vinculadas a um produtor |
| POST | `/api/propriedade` | Cadastrar propriedade vinculando localidade espacial |
| DELETE | `/api/propriedade/{id}` | Remover uma propriedade |

### Gestão Agrícola (Talhões & Plantações)
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/talhao` | Listar talhões cadastrados e suas métricas |
| GET | `/api/talhao/{id}` | Buscar talhão por ID |
| GET | `/api/talhao/by-propriedade/{propriedadeId}` | Listar talhões vinculados a uma propriedade |
| GET | `/api/talhao/by-tipo-plantacao/{tipoPlantacaoId}` | Listar talhões filtrados por tipo de cultura agrícola |
| POST | `/api/talhao` | Cadastrar novo talhão e cultura (`TipoPlantacao`) |
| DELETE | `/api/talhao/{id}` | Remover um talhão |
| GET | `/api/tipoplantacao` | Listar tipos de culturas agrícolas disponíveis |
| GET | `/api/tipoplantacao/{id}` | Buscar cultura por ID |
| POST | `/api/tipoplantacao` | Cadastrar nova cultura agrícola |
| DELETE | `/api/tipoplantacao/{id}` | Remover uma cultura |

### Dados Espaciais & Localização
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/localizacao` | Listar entidades espaciais e coordenadas georreferenciadas |
| GET | `/api/localizacao/{id}` | Buscar uma localização espacial por ID |
| POST | `/api/localizacao` | Cadastrar nova coordenada (WGS 84 Point) |
| DELETE | `/api/localizacao/{id}` | Remover um registro espacial |

### Comunicação (Telefones)
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/telefone` | Listar todos os telefones de produtores |
| GET | `/api/telefone/{id}` | Buscar telefone por ID |
| GET | `/api/telefone/by-produtor/{produtorId}` | Buscar o telefone associado a um produtor específico |
| POST | `/api/telefone` | Cadastrar novo contato telefônico |
| DELETE | `/api/telefone/{id}` | Remover registro de telefone |

### Requisições de API Externa (Integrações)

A integração com APIs externas (NASA POWER e Embrapa SATVeg) é realizada de forma **assíncrona** via `async/await` (Controller → Service → HttpClient), garantindo alta performance sem bloquear threads do Kestrel. Ao chamar o endpoint de criação, o sistema:

1. Valida o TipoApi e Talhão
2. Cria o cabeçalho `ReqApi`
3. Chama a API externa (NASA ou SATVeg) usando coordenadas do Talhão
4. Filtra dados inválidos (sensor com defeito NASA `<= -900`, datas SATVEG `>= 2020-01-01`)
5. Persiste `DadoTemporal` em lote
6. **Gera alertas automáticos** baseados em thresholds de negócio

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/reqapi` | Lista requisições (com contagem de dados otimizada via `COUNT` no banco) |
| GET | `/api/reqapi/{id}` | Busca requisição por ID (com contagem de dados) |
| GET | `/api/reqapi/talhao/{idTalhao}` | Lista requisições de um talhão via `EXISTS` otimizado (`.Any()`) |
| POST | `/api/reqapi` | **Async** - Executa integração e persiste dados + alertas |
| DELETE | `/api/reqapi/{id}` | Remove requisição |

**Compatibilidade de parâmetros de integração:**
| API Externa | `tipoParam` (enum) | Valor JSON |
| --- | --- | --- |
| `NASAPOWER` | `PRECTOTCORR` (chuva) | `1` |
| `SATVEG` | `NDVI` (vegetação) | `0` |

**Thresholds de Alerta Automático:**
| API | Condição | Nível | Título |
| --- | --- | --- | --- |
| NASA | Chuva 3 dias > 80mm | Alto | Risco de Alagamento |
| NASA | Chuva 15 dias < 10mm | Crítico | Seca Severa |
| NASA | Chuva 15 dias < 25mm | Médio | Estresse Hídrico |
| SATVEG | NDVI < 0.2 | Crítico | Anomalia Vegetativa Severa |
| SATVEG | NDVI < 0.4 | Médio | Baixo Vigor Vegetativo |

### Dados Temporais
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/dadostemporal` | Lista todos os dados temporais armazenados |
| GET | `/api/dadostemporal/{id}` | Busca dado temporal por ID |
| GET | `/api/dadostemporal/talhao/{talhaoId}` | Lista dados temporais filtrados por talhão |
| GET | `/api/dadostemporal/req-api/{reqApiId}` | Lista dados temporais gerados por uma requisição específica |

### Alertas Agrícolas

Sistema de alertas reativos com **deduplicação automática**: não cria alertas duplicados para o mesmo talhão + mesmo título enquanto não resolvido.

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/alertaagricola` | Lista todos os alertas |
| GET | `/api/alertaagricola/{id}` | Busca alerta por ID |
| GET | `/api/alertaagricola/talhao/{talhaoId}` | Lista alertas de um talhão |
| POST | `/api/alertaagricola` | Cria alerta manual |
| PATCH | `/api/alertaagricola/{id}/resolver` | Marca alerta como resolvido |
| PATCH | `/api/alertaagricola/{id}/reabrir` | Reabre alerta resolvido |
| DELETE | `/api/alertaagricola/{id}` | Remove alerta |

**Regras de Deduplicação:** Não permite alerta ativo (`resolvido = false`) com mesmo `titulo` para o mesmo `talhaoId`.
