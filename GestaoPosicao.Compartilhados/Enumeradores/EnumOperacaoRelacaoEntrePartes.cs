namespace GestaoPosicao.Compartilhados.Enumeradores;
/// <summary>
/// Quando temos duas partes (parte e contraparte), indica a relação de integridade entre elas
/// </summary>
public enum EnumOperacaoRelacaoEntrePartes : byte
{
    NAO_APLICAVEL = 0,
    /// <summary>
    /// Indica que as partes são idênticas, ou seja, a operação é feita entre a mesma parte e contraparte
    /// </summary>
    PARTES_IDENTICAS = 1,
    /// <summary>
    /// Indica que as partes são distintas, ou seja, a operação é feita entre partes diferentes
    /// </summary>
    PARTES_DISTINTAS = 2,
}