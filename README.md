# FixAtg

O FixAtg é um projeto que contém duas soluções: OrderGenerator e OrderAccumulator.



https://github.com/isaacmklein/FixAtg/assets/165307702/bd00c69f-4469-4b81-8d84-506b54b8d02b



## OrderGenerator

O OrderGenerator é responsável por gerar ordens fictícias que serão enviadas para o OrderAccumulator para processamento. Ele simula o comportamento de um sistema de geração de ordens em um ambiente de negociação financeira.

### Funcionalidades

- Geração contínua de ordens fictícias com símbolos, quantidades e preços aleatórios.
- Envio das ordens geradas para o OrderAccumulator para processamento.

## OrderAccumulator

O OrderAccumulator é responsável por receber as ordens geradas pelo OrderGenerator, aplicar validações e processá-las de acordo com as regras de negócio definidas. Ele mantém um registro da exposição financeira e gerencia as ordens de compra e venda para garantir que não excedam os limites estabelecidos.

### Funcionalidades

- Recebimento das ordens geradas pelo OrderGenerator.
- Validação das ordens recebidas, incluindo verificação de exposição financeira.
- Processamento das ordens aceitas, incluindo o envio de relatórios de execução.
- Rejeição das ordens que excedem os limites de exposição financeira.
- Gerenciamento da exposição financeira total por símbolo.
- Integração com o sistema de mensagens FIX para comunicação com o OrderGenerator.

## Pré-requisitos

- Plataforma compatível com .NET Core 8.0.
- Configuração correta das duas soluções para execução simultânea.
