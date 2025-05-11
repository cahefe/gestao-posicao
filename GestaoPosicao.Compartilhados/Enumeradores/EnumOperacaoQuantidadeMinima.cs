namespace GestaoPosicao.Compartilhados.Enumeradores;

/// <summary>
/// Orienta quanto ao menor valor mínimo definido para um tipo de operação
/// </summary>
/// <remarks>
/// Utilizado para valores ou quantidades
/// </remarks>
public enum EnumOperacaoQuantidadeMinima: byte
{
    /// <summary>
    /// Qualquer quantidade maior que zero
    /// </summary>
    MAIOR_QUE_ZERO = 1,
    /// <summary>
    /// Valor pré-definido (a partir de algum parâmetro)
    /// </summary>
    PREDEFINIDA = 2,
}
