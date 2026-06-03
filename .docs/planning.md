# FillGaps.SpeedOrders - Cronograma de Execução e Planejamento

Este documento serve como o mapa de etapas do projeto **FillGaps.SpeedOrders**. Ele foi estruturado para permitir pausas e retomadas rápidas sem perda de contexto arquitetural.

---

## 📋 Status Atual Geral: 🟨 Fase 2 Concluída (Parcialmente) / Iniciando Injeções e Mensageria

- **[X] Estrutura de Pastas e Solução .NET 10 via CLI** (Concluído)
- **[X] Fase 1: Infraestrutura Local com Docker Compose** (Concluído)
- **[X] Nova Fase: Camada de Aplicação (Use Cases, DTOs e Contratos)** (Concluído)
- **[X] Fase 2: Camada de Domínio e Infraestrutura de Dados** (Concluído)
- **[X] Fase 3: API Principal (Producer & Endpoints)** (Concluído)
- **[X] Fase 4: Background Worker (Consumer & Resiliência com Polly)** (Concluído)
- **[ ] Fase 5: Dockerização Avançada e Orquestração Local no Kubernetes** (Pendente)

---

## 🛠️ Detalhamento das Fases

### Fase 1: Infraestrutura Local (Docker Compose)
*Objetivo: Disponibilizar as dependências externas de infraestrutura para desenvolvimento local.*
- [X] Subir o ambiente via `docker-compose up -d` e validar se as portas (`1433` para SQL e `9092` para Kafka) estão acessíveis.

### Nova Fase: Camada de Aplicação (Application)
*Objetivo: Orquestrar regras de negócio, separando o Core das tecnologias de entrega.*
- [X] Criar a estrutura do projeto `Application` e referenciar `Domain`.
- [X] Criar os DTOs (Data Transfer Objects) usando `records` do C#.
- [X] Configurar o *AppService* orquestrando a criação do Pedido e disparo de evento (`OrderCreatedEvent`).
- [X] Criar contratos de abstração para mensageria (`IMessagePublisher`) e queries (`IOrderQueries`).

### Fase 2: Domínio e Infraestrutura de Dados (Domain & Infrastructure)
*Objetivo: Modelar o coração do sistema e configurar os mecanismos de persistência.*
- [X] Criar a entidade base (`Entity`) e entidade `Order` com *Rich Domain Model*.
- [X] Configurar o **Entity Framework Core** (`SpeedOrdersDbContext`, *Mappings* e *Generic Repository/UnitOfWork*).
- [X] Implementar a interface de leitura utilizando **Dapper** (`OrderQueries`) aplicando *CQRS Tático*.
- [X] Criar as *Migrations* iniciais..

### Fase 3: API Principal (Presentation & Production)
*Objetivo: Disponibilizar os endpoints HTTP e realizar a publicação de eventos.*
- [X] Configurar a Injeção de Dependência (IoC) do EF Core, Dapper e Kafka na `Api` e `Worker`.
- [X] Implementar a publicação real do Kafka (`IMessagePublisher`).
- [X] Criar o endpoint de comandos: `POST /api/orders` (Chama o `AppService`).
- [X] Criar o endpoint de consultas: `GET /api/orders` (Chama o `IOrderQueries`).
- [X] Configurar o pacote nativo de *Health Checks* do .NET.

### Fase 4: Background Worker (Consumer & Resiliência)
*Objetivo: Processar os pedidos de forma assíncrona tolerando falhas de terceiros.*
- [X] Instalar pacotes do Kafka e Polly no projeto Worker.
- [X] Criar um serviço de simulação (*Mock*) de gateway de pagamento que falhe propositalmente.
- [X] Implementar a política de *Retry* com *Exponential Backoff*.
- [X] Implementar o *Circuit Breaker* para isolamento do gateway.
- [X] Implementar o *Fallback* e envelopar (*Wrap*) todas as políticas.
- [X] Configurar o `BackgroundService` para escutar o tópico `order-created-topic`.

### Fase 5: Dockerização e Kubernetes
*Objetivo: Migrar a arquitetura local para um modelo orquestrado.*
- [ ] Criar *Multi-stage Dockerfiles* para a `Api` e para o `Worker`.
- [ ] Escrever os manifestos YAML do Kubernetes (`Deployment`, `Service`).
- [ ] Configurar os mapeamentos de **Liveness Probe** e **Readiness Probe**.