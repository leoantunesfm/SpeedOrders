# FillGaps.SpeedOrders - Documentação Arquitetural

## Visão Geral
O **FillGaps.SpeedOrders** é uma Prova de Conceito (PoC) focada em alta performance, resiliência e comunicação assíncrona. O objetivo principal é receber intenções de criação de pedidos de forma rápida e delegar o processamento pesado para *Background Workers*, garantindo que a API principal permaneça responsiva.

## Decisões Técnicas (*Tech Stack*)
* **Plataforma:** .NET 10 (C#)
* **Orquestração & Containers:** Docker e Kubernetes (K3s/Minikube)
* **Mensageria:** Apache Kafka
* **Banco de Dados:** SQL Server 2022
* **ORM e Acesso a Dados:** Entity Framework Core (Escrita/Domínio) e Dapper (Leitura/Alta Performance)
* **Resiliência:** Polly (Retry, Circuit Breaker, Fallback)

## Desenho da Solução

### 1. API Principal (`FillGaps.SpeedOrders.Api`)
Responsável por receber o tráfego HTTP dos clientes.
* **Escrita (`POST /api/orders`):** Recebe o *payload*, valida via *Domain Driven Design* (DDD), salva o estado inicial como `Pending` usando **Entity Framework Core** e publica um evento `OrderCreatedEvent` no **Kafka**. Retorna `HTTP 202 Accepted`.
* **Leitura (`GET /api/orders`):** *Endpoints* de consulta massiva. Ignora o EF Core e utiliza **Dapper** com *queries* otimizadas para leitura rápida direto do banco de dados.

### 2. Processador em Background (`FillGaps.SpeedOrders.Worker`)
Serviço *headless* (sem interface web) responsável por processar a fila.
* **Consumo:** Assina o tópico do **Kafka** para processar os eventos `OrderCreatedEvent`.
* **Integração Externa & Resiliência:** Simula uma chamada a um serviço de pagamento/ERP. Utiliza **Polly** para aplicar:
    * *Retry Policy* com *Exponential Backoff* para falhas transitórias de rede.
    * *Circuit Breaker* para abortar processamentos se a API externa cair.
    * *Fallback* para marcar pedidos com necessidade de intervenção manual se todas as tentativas falharem.

## Padrões Adotados
* **Separação de Responsabilidades (SoC):** Divisão clara entre *Domain*, *Infrastructure* e *Presentation/API*.
* **Fail Fast:** Uso de *Health Checks* (Liveness/Readiness) para integração nativa com Kubernetes.
* **CQRS (Command Query Responsibility Segregation) Simplificado:** O uso do EF Core para comandos e Dapper para consultas reflete um isolamento tático das responsabilidades de banco.