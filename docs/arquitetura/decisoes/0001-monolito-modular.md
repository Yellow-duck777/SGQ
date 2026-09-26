# ADR 0001 — Monólito modular

- Estado: aceito
- Data: 2026-09-22

## Contexto

O SGQ crescerá em regras e módulos, mas é desenvolvido por uma equipe pequena e possui um único ciclo de implantação.

## Decisão

Separar Domain, Application, Infrastructure e Web na mesma solução e no mesmo processo de implantação. RC, NC e Recall serão módulos internos, não serviços independentes.

## Consequências

Regras tornam-se testáveis sem banco ou MVC, enquanto implantação e transações permanecem simples. Testes arquiteturais serão necessários para preservar as fronteiras.
