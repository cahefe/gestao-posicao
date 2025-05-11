namespace GestaoPosicao.Compartilhados.Enumeradores;

public enum EnumCustodiaAlgoritmoDebito
{
    /// <summary>
    /// Não existem um algorítmo a ser aplicado
    /// </summary>
    NAO_APLICAVEL = 0,
    /// <summary>
    /// Realiza débito somente no escopo de alocação, sem afetar as posições existentes
    /// </summary>
    ALOCACAO = 10,
    /// <summary>
    /// Usar regra LIFO
    /// </summary>
    POSICAO_LIFO = 100,
    /// <summary>
    /// Realizar o débito proporcional FIFO considerando TODAS posições existentes
    /// </summary>
    POSICAO_LIFO_PROPORCIONAL = 101,
    /// <summary>
    /// Usar regra FIFO
    /// </summary>
    POSICAO_FIFO = 110,
    /// <summary>
    /// Realizar o débito proporcional FIFO considerando TODAS posições existentes
    /// </summary>
    POSICAO_FIFO_PROPORCIONAL = 111,
}
