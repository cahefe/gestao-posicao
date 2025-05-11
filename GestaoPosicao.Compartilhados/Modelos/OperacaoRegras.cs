using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Compartilhados.Modelos;

public record OperacaoRegras
(
    EnumOperacaoTipo Tipo,
    EnumCustodiaComando CustodiaComando,
    OperacaoCaracteristicasParte Parte,
    OperacaoCaracteristicasParte? Contraparte = null,
    EnumOperacaoQuantidadeMinima QuantidadeMinima = EnumOperacaoQuantidadeMinima.MAIOR_QUE_ZERO,
    decimal QuantidadeMinimaPredefinida = 0,
    byte LimiteDecimaisQuantidade = 2,
    EnumOperacaoCalculoValorTotal CalculoValorOperacao = EnumOperacaoCalculoValorTotal.NAO_APLICAVEL,
    EnumCustodiaAlgoritmoDebito CustodiaAlgoritmoDebito = EnumCustodiaAlgoritmoDebito.NAO_APLICAVEL
)
{ }
