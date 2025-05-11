namespace GestaoPosicao.Compartilhados.Enumeradores;

public enum EnumOperacaoComportamentoParte
{
    /// <summary>
    /// Deve ser informada durante a requisição
    /// </summary>
    DECLARADA = 1,
    /// <summary>
    /// Prédefinida pelo contexto, nunca deve ser informada
    /// </summary>
    PREDEFINIDA = 2,
}
