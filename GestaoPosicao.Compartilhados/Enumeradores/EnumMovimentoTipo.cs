namespace GestaoPosicao.Compartilhados.Enumeradores;

public enum EnumMovimentoTipo : byte
{
    CREDITO_DEPOSITO = 1,
    DEBITO_RETIRADA = 2,
    CREDITO_INVESTIMENTO = 3,
    DEBITO_LASTRO_INVESTIMENTO = 4,
    CREDITO_LASTRO_RESGATE = 5,
    DEBITO_RESGATE = 6,
    CREDITO_TRANSFERENCIA = 7,
    DEBITO_TRANSFERENCIA = 8,
}
