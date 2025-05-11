namespace GestaoPosicao.Compartilhados.Enumeradores;
/// <summary>
/// Enumeração que define os tipos de comandos possíveis na custódia
/// </summary>
public enum EnumCustodiaComando : byte
{
    /// <summary>
    /// Cria uma nova posição na custódia
    /// </summary>
    Criar = 1,

    /// <summary>
    /// Atualiza uma posição existente na custódia
    /// </summary>
    Movimentar = 2,

    /// <summary>
    /// Atualiza informações gerais da posição na custódia sem alterar as quantidades
    /// </summary>
    Atualizar = 3,

    /// <summary>
    /// Extirguir uma posição existente na custódia
    /// </summary>
    Extinguir = 4,
}
