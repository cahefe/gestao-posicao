using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Custodia;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Modelos;

namespace GestaoPosicao.Operacoes;

/// <summary>
/// Verifica se a quantidade da operação é válida e reserva a quantidade na carteira.
/// </summary>
/// <param name="bag"></param>
/// <param name="operacaoRegras"></param>
public class RegistrarReservarQuantidade(Bag<DtoOperacao, Operacao> bag, ReservarQuantidades reservarQuantidades) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var dtoOperacao = bag.Request;
        if (!bag.TryGet<OperacaoRegras>(RegistrarItensBag.OperacaoRegras, out var regra))
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_REGRAS_NAO_ENCONTRADA", $"Regras para operação {dtoOperacao.TipoOperacao} não encontradas", FailLevel.Error)));

        OperacaoCaracteristicasParte? parte = null;
        //  Identificar a parte associada a movimento de débito
        if (regra.Parte.MovimentoSentido == EnumMovimentoSentido.DEBITO)
            parte = regra.Parte;
        else if (regra.Contraparte is not null && regra.Contraparte.MovimentoSentido == EnumMovimentoSentido.DEBITO)
            parte = regra.Contraparte;

        if (parte is null)
            return Task.FromResult(Result.GoSuccess());

        if (!bag.TryGet<int>(RegistrarItensBag.NumeroOperacao, out var idOperacao))
            return Task.FromResult(Result.GoFails(new Fail(-1, "NUMERO_OPERACAO_NAO_ENCONTRADO", $"Número da operação não encontrado", FailLevel.Error)));

        //  Identifica a a conta a ser utilizada para reserva de quantidade
        var conta = parte.PartePredefinida;
        if (parte.ComportamentoParte == EnumOperacaoComportamentoParte.DECLARADA)
            conta = parte.IndicadorParte == EnumOperacaoIndicadorParte.PARTE ? dtoOperacao.ContaParte ?? 0 : dtoOperacao.ContaContraparte ?? 0;
        //  Reservar quantidades
        var dtoReserva = new DtoReserva(
            IdOperacao: idOperacao,
            Conta: conta,
            Carteira: parte.Carteira,
            Titulo: dtoOperacao.Titulo,
            Quantidade: dtoOperacao.Quantidade);
        var reservarResultado = reservarQuantidades.Execute(dtoReserva, cancellationToken);

        if (!reservarResultado.Result.Success)
            return Task.FromResult(Result.GoFails([.. reservarResultado.Result.Fails]));

        return Task.FromResult(Result.GoSuccess());
    }
}
