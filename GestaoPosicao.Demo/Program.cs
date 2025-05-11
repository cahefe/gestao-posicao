using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Custodia;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Custodia.Extensoes;
using GestaoPosicao.Custodia.Modelos;
using GestaoPosicao.Operacoes;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Extensoes;
using GestaoPosicao.Operacoes.Modelos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var serviceProvider = PrepararServiceProvider();
foreach (var dtoOperacao in PrepararOperacoes())
{
    using var scope = serviceProvider.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
    var pipelineRegistrar = ObterPipeline<DtoOperacao, Operacao>("Registrar", scope);
    var resultadoRegistrar = await ExecutarPipeline(pipelineRegistrar, dtoOperacao);
    if (TratarResultadoFalha(resultadoRegistrar, logger))
        continue;

    var operacao = resultadoRegistrar.Response;
    if (operacao == null)
    {
        logger.LogError("Failed to retrieve IdOperacao as Response is null.");
        continue;
    }
    //  Liquidar operação...
    var dtoLiquidacao = new DtoLiquidacaoReq(
            IdOperacao: operacao.IdOperacao,
            TipoOperacao: operacao.TipoOperacao,
            ContaParte: operacao.Parte,
            CarteiraParte: operacao.CarteiraParte,
            Titulo: operacao.Titulo,
            Quantidade: operacao.Quantidade,
            PURef: operacao.PURef,
            TaxaRef: operacao.TaxaRef,
            ValorTotal: operacao.ValorTotal,
            ContaContraparte: operacao.Contraparte,
            CarteiraContraparte: operacao.CarteiraContraparte
        );
    var pipelineLiquidar = ObterPipeline<DtoLiquidacaoReq, DtoLiquidacaoResp>("Liquidar", scope);
    var resultadoLiquidar = await ExecutarPipeline(pipelineLiquidar, dtoLiquidacao);
    if (TratarResultadoFalha(resultadoLiquidar, logger))
        continue;

    if (resultadoLiquidar.Response == null)
    {
        logger.LogError("Failed to retrieve IdOperacao as Response is null.");
        continue;
    }
}
ApresentarResultados(serviceProvider);
return;

static void ApresentarResultados(IServiceProvider serviceProvider)
{
    var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

    var operacoes = serviceProvider.GetRequiredService<Operacoes>();
    foreach (var operacao in operacoes)
        logger.LogInformation("> {Operacao}", operacao);
    var posicoes = serviceProvider.GetRequiredService<Posicoes>();
    foreach (var posicao in posicoes)
        logger.LogInformation("> {Posicao}", posicao);
    var posicoesAtividades = serviceProvider.GetRequiredService<PosicoesAtividades>();
    foreach (var posicaoAtividade in posicoesAtividades)
        logger.LogInformation("> {PosicaoAtividade}", posicaoAtividade);
    var alocacoes = serviceProvider.GetRequiredService<Alocacoes>();
    foreach (var alocacao in alocacoes)
        logger.LogInformation("> {Alocacao}", alocacao);
    var alocacoesAtividades = serviceProvider.GetRequiredService<AlocacoesAtividades>();
    foreach (var alocacaoAtividade in alocacoesAtividades)
        logger.LogInformation("> {Alocacao}", alocacaoAtividade);
    var movimentos = serviceProvider.GetRequiredService<Movimentos>();
    foreach (var movimento in movimentos)
        logger.LogInformation("> {Movimento}", movimento);
}

static IServiceProvider PrepararServiceProvider()
{
    var services = new ServiceCollection();
    //  Incluindo serviços
    //  ... logging
    services.AddLogging(builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        builder.AddDebug();
    });
    services.AddSingleton<Operacoes>();
    services.AddSingleton<AlocacoesAtividades>();
    services.AddSingleton<Alocacoes>();
    services.AddSingleton<PosicoesAtividades>();
    services.AddSingleton<Posicoes>();
    services.AddSingleton<Reservas>();
    services.AddSingleton<Movimentos>();
    services.AddSingleton(PrepararOperacaoRegras());

    //  "Serviços" não implementados
    services.AddTransient<ReservarQuantidades>(); // Reservar quantidades na custódia
    //  Pipeline "Registrar"
    services.IncluirCasosUsoOperacoes(); // Adiciona serviços de operações
    services.IncluirCasosUsoCustodia(); // Adiciona serviços de custódia
    return services.BuildServiceProvider();
}

static Pipeline<TReq, TResp> ObterPipeline<TReq, TResp>(string nomePipeline, IServiceScope serviceScope)
{
    if (nomePipeline == "Registrar")
        return serviceScope.ServiceProvider.GetRequiredService<Pipeline<TReq, TResp>>()
            .AppendPhase("Registrar: Preparação")
            .AppendStep<TReq, TResp, RegistrarPrepararPipeline>()
            .AppendPhase("Registrar: Validações")
            .AppendStep<TReq, TResp, RegistrarValidarCotacao>()
            .AppendStep<TReq, TResp, RegistrarValidarQuantidade>()
            .AppendStep<TReq, TResp, RegistrarValidarPartes>()
            .AppendPhase("Registrar: Numeração")
            .AppendStep<TReq, TResp, RegistrarObterNumeroOperacao>()
            .AppendPhase("Registrar: Reserva")
            .AppendStep<TReq, TResp, RegistrarReservarQuantidade>()
            .AppendPhase("Registrar: Persistência")
            .AppendStep<TReq, TResp, RegistrarPersistirOperacao>();
    else if (nomePipeline == "Liquidar")
        return serviceScope.ServiceProvider.GetRequiredService<Pipeline<TReq, TResp>>()
            .AppendPhase("Liquidar: Preparação")
            .AppendStep<TReq, TResp, LiquidarPrepararPipeline>()
            .AppendPhase("Liquidar: Débitos")
            .AppendStep<TReq, TResp, LiquidarDebitarPosicoes>()
            .AppendPhase("Liquidar: Créditos")
            .AppendStep<TReq, TResp, LiquidarCreditarPosicoes>()
            .AppendStep<TReq, TResp, LiquidarManterAlocacoes>()
            .AppendPhase("Liquidar: Movimentos")
            .AppendStep<TReq, TResp, LiquidarManterMovimentos>()
            .AppendPhase("Liquidar: Persistência")
            .AppendStep<TReq, TResp, LiquidarPersistir>();
    else
        throw new ArgumentException($"Pipeline '{nomePipeline}' não reconhecida.");
}

static async Task<Result<TResp>> ExecutarPipeline<TReq, TResp>(Pipeline<TReq, TResp> pipeline, TReq requisicao) => await pipeline.Execute(requisicao);

static bool TratarResultadoFalha<TResp>(Result<TResp> resultado, ILogger logger)
{
    foreach (var fail in resultado.Fails)
    {
        var logLevel = fail.FailLevel switch
        {
            FailLevel.Critical => LogLevel.Critical,
            FailLevel.Error => LogLevel.Error,
            _ => LogLevel.Warning
        };
        logger.Log(logLevel, "{Codigo} ({Mnemonico}) - {Info}", fail.Code, fail.Mnemonic, fail.Info);
    }
    return !resultado.Success;
}

static IList<DtoOperacao> PrepararOperacoes() => [
    new(TipoOperacao: EnumOperacaoTipo.Deposito, Titulo: 101, Quantidade: 1.0m),
    // new(TipoOperacao: EnumOperacaoTipo.Deposito, Titulo: 101, Quantidade: 2.0m),
    new(TipoOperacao: EnumOperacaoTipo.Investimento, ContaParte: 456, Titulo: 101, Quantidade: 0.43m),
    new(TipoOperacao: EnumOperacaoTipo.TrasnfLivreSemTroca, ContaParte: 456, ContaContraparte: 789, Titulo: 101, Quantidade: 0.23m),
    new(TipoOperacao: EnumOperacaoTipo.TrasnfLivreComTroca, ContaParte: 456, ContaContraparte: 321, Titulo: 101, Quantidade: 0.07m),
    new(TipoOperacao: EnumOperacaoTipo.Resgate, ContaParte: 789, Titulo: 101, Quantidade: 0.13m),
    // new(TipoOperacao: EnumOperacaoTipo.Retirada, Titulo: 101, Quantidade: 2.0m),
    // new(TipoOperacao: EnumOperacaoTipo.Retirada, Titulo: 101, Quantidade: 1.0m),
];

static IList<OperacaoRegras> PrepararOperacaoRegras() => [
    new(
        Tipo: EnumOperacaoTipo.Deposito,
        CustodiaComando: EnumCustodiaComando.Criar,
        Parte: new(
            IndicadorParte: EnumOperacaoIndicadorParte.PARTE,
            ComportamentoParte: EnumOperacaoComportamentoParte.PREDEFINIDA,
            PartePredefinida: (int)EnumPartePredefinida.LASTRO,
            Carteira: EnumCarteira.LASTRO,
            MovimentoTipo: EnumMovimentoTipo.CREDITO_DEPOSITO,
            MovimentoSentido: EnumMovimentoSentido.CREDITO
        ),
        QuantidadeMinima: EnumOperacaoQuantidadeMinima.PREDEFINIDA,
        QuantidadeMinimaPredefinida: 1m,
        LimiteDecimaisQuantidade: 0
    ),
    new(
        Tipo: EnumOperacaoTipo.Retirada,
        CustodiaComando: EnumCustodiaComando.Extinguir,
        Parte: new(
            IndicadorParte: EnumOperacaoIndicadorParte.PARTE,
            ComportamentoParte: EnumOperacaoComportamentoParte.PREDEFINIDA,
            PartePredefinida: (int)EnumPartePredefinida.LASTRO,
            Carteira: EnumCarteira.LASTRO,
            MovimentoTipo: EnumMovimentoTipo.DEBITO_RETIRADA,
            MovimentoSentido: EnumMovimentoSentido.DEBITO
        ),
        QuantidadeMinima: EnumOperacaoQuantidadeMinima.PREDEFINIDA,
        QuantidadeMinimaPredefinida: 1m,
        LimiteDecimaisQuantidade: 0,
        CustodiaAlgoritmoDebito: EnumCustodiaAlgoritmoDebito.POSICAO_FIFO
    ),
    new(
        Tipo: EnumOperacaoTipo.Investimento,
        CustodiaComando: EnumCustodiaComando.Movimentar,
        Parte: new(
            IndicadorParte: EnumOperacaoIndicadorParte.PARTE,
            ComportamentoParte: EnumOperacaoComportamentoParte.DECLARADA,
            Carteira: EnumCarteira.LIVRE,
            MovimentoTipo: EnumMovimentoTipo.CREDITO_INVESTIMENTO,
            MovimentoSentido: EnumMovimentoSentido.CREDITO
        ),
        Contraparte: new(
            IndicadorParte: EnumOperacaoIndicadorParte.CONTRAPARTE,
            ComportamentoParte: EnumOperacaoComportamentoParte.PREDEFINIDA,
            Carteira: EnumCarteira.LASTRO,
            MovimentoTipo: EnumMovimentoTipo.DEBITO_LASTRO_INVESTIMENTO,
            MovimentoSentido: EnumMovimentoSentido.DEBITO,
            PartePredefinida: (int)EnumPartePredefinida.LASTRO
        ),
        QuantidadeMinima: EnumOperacaoQuantidadeMinima.MAIOR_QUE_ZERO,
        LimiteDecimaisQuantidade: 2,
        CustodiaAlgoritmoDebito: EnumCustodiaAlgoritmoDebito.POSICAO_FIFO
    ),
    new(
        Tipo: EnumOperacaoTipo.Resgate,
        CustodiaComando: EnumCustodiaComando.Movimentar,
        Parte: new(
            IndicadorParte: EnumOperacaoIndicadorParte.PARTE,
            ComportamentoParte: EnumOperacaoComportamentoParte.DECLARADA,
            Carteira: EnumCarteira.LIVRE,
            MovimentoTipo: EnumMovimentoTipo.DEBITO_RESGATE,
            MovimentoSentido: EnumMovimentoSentido.DEBITO
        ),
        Contraparte: new(
            IndicadorParte: EnumOperacaoIndicadorParte.CONTRAPARTE,
            ComportamentoParte: EnumOperacaoComportamentoParte.PREDEFINIDA,
            Carteira: EnumCarteira.LASTRO,
            MovimentoTipo: EnumMovimentoTipo.CREDITO_LASTRO_RESGATE,
            MovimentoSentido: EnumMovimentoSentido.CREDITO,
            PartePredefinida: (int)EnumPartePredefinida.LASTRO
        ),
        QuantidadeMinima: EnumOperacaoQuantidadeMinima.MAIOR_QUE_ZERO,
        LimiteDecimaisQuantidade: 2,
        CustodiaAlgoritmoDebito: EnumCustodiaAlgoritmoDebito.POSICAO_FIFO
    ),
    new(
        Tipo: EnumOperacaoTipo.TrasnfLivreSemTroca,
        CustodiaComando: EnumCustodiaComando.Movimentar,
        Parte: new(
            IndicadorParte: EnumOperacaoIndicadorParte.PARTE,
            ComportamentoParte: EnumOperacaoComportamentoParte.DECLARADA,
            Carteira: EnumCarteira.LIVRE,
            MovimentoTipo: EnumMovimentoTipo.DEBITO_TRANSFERENCIA,
            MovimentoSentido: EnumMovimentoSentido.DEBITO
        ),
        Contraparte: new(
            IndicadorParte: EnumOperacaoIndicadorParte.CONTRAPARTE,
            ComportamentoParte: EnumOperacaoComportamentoParte.DECLARADA,
            Carteira: EnumCarteira.LIVRE,
            MovimentoTipo: EnumMovimentoTipo.CREDITO_TRANSFERENCIA,
            MovimentoSentido: EnumMovimentoSentido.CREDITO
        ),
        QuantidadeMinima: EnumOperacaoQuantidadeMinima.MAIOR_QUE_ZERO,
        LimiteDecimaisQuantidade: 2,
        CustodiaAlgoritmoDebito: EnumCustodiaAlgoritmoDebito.POSICAO_LIFO
    ),
];
