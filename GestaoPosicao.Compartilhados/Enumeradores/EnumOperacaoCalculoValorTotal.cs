namespace GestaoPosicao.Compartilhados.Enumeradores;

/// <summary>
/// Indica qual a regra para cálculo do valor da operação
/// </summary>
public enum EnumOperacaoCalculoValorTotal
{
    NAO_APLICAVEL = 0,
    SOMENTE_QUANTIDADE_X_PU = 1,
    QUANTIDADE_X_PU_SOMA_TAXAS = 2,
    QUANTIDADE_X_PU_SUBTRAI_TAXAS = 3,
}
