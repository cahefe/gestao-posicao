namespace GestaoPosicao.Compartilhados.Enumeradores;
/// <summary>
/// Catalogo de tipos de operações
/// </summary>
/// <remarks>
/// Observe que a dezena indica o grupo de operações (Investimento: 2, Resgate: 3), enquanto a unidade representa um subgrupo ou tipo específico de operação (Ex: 20 - Investimento (Regular), 21 - Investimento Programado, etc.).
/// </remarks>
public enum EnumOperacaoTipo
{
    Indefinido = 0,
    Deposito = 10,
    Retirada = 11,
    Investimento = 20,
    InvestimentoProgramado = 21,
    InvestimentoReinvestimento = 22,
    InvestimentoSorteio = 23,
    Resgate = 30,
    ResgateExecucao = 31,
    BloqGarn = 40,
    DesbloqGarn = 41,
    TrasnfLivreSemTroca = 50,
    TrasnfLivreComTroca = 51,
    TrasnfGarnSemTroca = 52,
}
