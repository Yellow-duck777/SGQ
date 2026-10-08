# Matriz de testes

Esta matriz acompanha a [matriz de rastreabilidade](../produto/matriz-rastreabilidade.md). Estados: `Planejado`, `Automatizado`, `Manual`, `Verificado` ou `Bloqueado`. A contagem de testes não é fixada aqui; execute `dotnet test SGQ.slnx` para o total atual.

## Testes automatizados existentes

### `SGQ.ArchitectureTests`

`ProjectDependencyTests`: `Domain_NaoDeveDependerDeCamadasExternas`, `Application_NaoDeveDependerDeInfrastructureOuWeb`, `Infrastructure_NaoDeveDependerDeWeb`.

### `SGQ.Web.Tests` (xUnit, EF Core InMemory)

| Classe | Cobre | Testes |
| --- | --- | --- |
| `PrazoServiceTests` | Dias úteis, calendário, prazo de NC | `AdicionarDiasUteisAsync_PulaFimDeSemana`, `AdicionarDiasUteisAsync_PulaDiaNaoUtilAtivo`, `CalcularPrazoNaoConformidadeAsync_NaoDefinePrazoParaClassificacaoCritica` |
| `AnexosControllerTests` | Envio e vínculo de anexos | `Enviar_EvidenciaCriticaComum_NaoExigeFluxoDeLaboratorio`, `Vincular_CriaVinculoParaProcessoDeDestino_EImpedeDuplicidade` |
| `FluxosProcessosControllerTests` | Validação da RC e regras de encerramento | `Validar_ReclamacaoElegivel_CriaNaoConformidadeVinculadaEIniciaInvestigacao`, `Encerrar_NaoConformidadeSemAprovacoes_ConservaStatus`, `Encerrar_RecallSemComunicacaoRegulatoria_ConservaStatus` |
| `ReclamacoesControllerTests` | Laboratório externo e reabertura da RC | `RegistrarResultadoLaboratorio_*` (2), `Reabrir_ReclamacaoEncerrada_RetornaParaInvestigacaoEPreservaHistorico` |
| `NaoConformidadesControllerTests` | Aprovação, divergência e reabertura da NC | `Reprovar_QuandoGqAprovouErtReprova_EncaminhaParaDecisaoDoCq`, `DecidirDivergencia_*` (2), `Reabrir_NcEncerrada_ResetaAprovacoesERetornaParaInvestigacao` |
| `RecallsControllerTests` | Aprovação, operação e reabertura do Recall | `Aprovar_QuandoRtEGqAprovam_EncaminhaParaRecolhimento`, `RegistrarOperacao_ComRegistrosObrigatorios_AguardaRetornos`, `DecidirDivergencia_SemJustificativaSuficiente_MantemAguardandoDecisaoDoCq`, `Reabrir_RecallEncerrado_ResetaAprovacoesERetornaParaAvaliacao` |
| `CorrecoesSegurancaFluxosTests` | Correções da DEM-2026-008 | Segregação RT/GQ (`NcAprovar_*`, `RecallAprovar_*`, `AprovarEReprovar_NaoPermitemAdministrador`), CQ e reabertura sem estado residual (`NcReabrir_*`, `NcAvaliarEficacia_*`, `NcAprovar_ComDecisaoFavoravelDoCq_*`, `Recall*`), classificação da RC (`Classificar_ExigeGestaoDaQualidade`, `RcClassificar_*`, `RcValidar_SemClassificacao_*`, `ReclamacoesController_NaoExpoeMaisAvancarStatus`), `RcReabrir_LimpaCarimboDeEncerramento`, `Csv_NeutralizaInjecaoDeFormula`, `Auditoria_NaoGravaHashDeSenhaNemCarimbosDeSeguranca`, `Baixar_Anexo*` |

### `SGQ.IntegrationTests` (xUnit, `WebApplicationFactory`, PostgreSQL real)

Rodam contra um PostgreSQL real indicado pela variável de ambiente `SGQ_TEST_PG` (conexão sem nome de banco); cada execução cria e descarta um banco próprio. Sem a variável, os testes aparecem como ignorados. A autenticação é simulada por um esquema de teste com cabeçalhos: o fluxo de login por cookie não é exercitado.

| Classe | Cobre | Testes |
| --- | --- | --- |
| `AcessoPorPerfilTests` | Política de acesso por perfil | `Anonimo_RotasDeNegocio_ExigemAutenticacao`, `AutenticadoSemPerfil_RecebeAcessoNegado`, `QualquerPerfilReconhecido_AcessaListagensEDashboard`, `Usuarios_SoAdministrador`, `PaginasPublicas_ContinuamAcessiveisSemLogin`, `ContaSemPerfil_AindaVePaginaDeErro`, `AvancarStatus_NaoExisteMais` |
| `BancoPostgresTests` | Banco | `Migrations_AplicadasEmBancoVazio_SemPendenciasDeModelo`, `CodigoAnualDeNc_NaoPodeSerDuplicado`, `VinculoDeAnexo_ExigeExatamenteUmProcesso`, `CriarUsuario_NaoGravaHashNemCarimbosNaAuditoria`, `AtribuirPerfil_FicaRegistradoNaAuditoria` |

## Cobertura por tema

| ID | Área | Nível | Resultado esperado | Estado |
| --- | --- | --- | --- | --- |
| AUTH-001 | Login válido e inválido | Manual | Autentica somente conta apta; bloqueio após cinco falhas | Manual (roteiro); login por cookie sem automação |
| AUTH-002 | Identidade do autor e dos pareceres | Manual e unidade | Grava o usuário autenticado | Manual (roteiro) e testes de segregação |
| SEC-001 | Acesso sem perfil e por perfil | Integração e manual | Anônimo é bloqueado, conta sem perfil recebe 403, `/Usuarios` só Administrador | Automatizado (`AcessoPorPerfilTests`); autorização por ação de escrita planejada (DEM-2026-101) |
| SEC-002 | Auditoria | Integração | Evento contém ator e mudança e não contém dados do Identity | Automatizado (`BancoPostgresTests`) |
| SEC-003 | Segregação RT/GQ | Unidade | Mesma conta e Administrador não emitem os dois pareceres | Automatizado (`CorrecoesSegurancaFluxosTests`) |
| SEC-004 | Segredos e dependências | CI | Nenhum segredo ou vulnerabilidade conhecida | Manual; varredura automática planejada |
| DB-001 | Migrations em banco vazio | Integração | Aplicam sem pendência de modelo | Automatizado |
| DB-002 | Restrições (código anual único, `CHECK` de vínculo) | Integração | PostgreSQL recusa dados inválidos | Automatizado |
| CAD-001 | Cadastros e abertura de RC | Manual | Cadastros listados; RC com código anual; lote de outro produto rejeitado | Manual (roteiro CAD-001) |
| CAD-004 | Inativação | Integração | Registro referenciado não é apagado | Planejado |
| RC | Fluxo de RC | Unidade e integração | Validação, NC automática, laboratório, reabertura | Automatizado em parte; conclusão, encerramento e prorrogação planejados |
| NC | Fluxo de NC | Unidade e integração | Aprovação, divergência, reabertura | Automatizado em parte; ações, eficácia e laboratório planejados |
| REC | Fluxo de Recall | Unidade e integração | Aprovação, operação, reabertura | Automatizado em parte; retornos e destinação planejados |
| ANX | Anexos | Integração e segurança | Valida acesso, formato, malware e limites | Automatizado em parte (envio, vínculo, download); malware, assinatura e limite de 20 planejados |
| PRZ | Prazos e calendário | Unidade | Dias úteis e prazos por classificação | Automatizado (`PrazoServiceTests`) |
| NOT | Notificações e alertas | Unidade e integração | Destinatários e deduplicação | Planejado |
| REL | Relatórios e exportação | Integração | Totais conciliam com os filtros; CSV sem injeção de fórmula | Automatizado em parte (CSV); totais planejados |
| E2E | Fluxos no navegador, concorrência e carga | E2E | Fluxos completos; concorrência de numeração | Planejado (DEM-2026-117) |
