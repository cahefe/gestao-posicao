using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Custodia.Modelos;

public class Alocacoes : List<Alocacao> {}
public record Alocacao
(
    int Conta,
    EnumCarteira Carteira,
    byte Titulo,
    decimal Quantidade
)
{
    static int _idAlocacao = 0;
    public int IdAlocacao { get; init; } = ++_idAlocacao;
    public decimal Quantidade { get; set; } = Quantidade;
    public override string ToString() => $"Alc AId:{IdAlocacao,4} Co:{Conta,4} Ca:{Carteira,-10} Ti:{Titulo,4} Qt:{Quantidade,10}";
}
