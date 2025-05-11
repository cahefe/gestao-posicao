namespace GestaoPosicao.Custodia.Modelos;
public class PosicoesAtividades : List<PosicaoAtividade> {}
public record PosicaoAtividade
(
    int IdPosicao,
    int IdOperacao,
    decimal Quantidade
)
{
    public override string ToString() => $"Pat Po:{IdPosicao,4} Op:{IdOperacao,4} Qt:{Quantidade,10}";
}
