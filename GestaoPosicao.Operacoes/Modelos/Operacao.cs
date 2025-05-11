using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Operacoes.Modelos;
public class Operacoes : List<Operacao>;
public record Operacao
(
    int IdOperacao,
    EnumOperacaoTipo TipoOperacao,
    int Parte,
    EnumCarteira CarteiraParte,
    byte Titulo,
    decimal Quantidade,
    decimal? PURef,
    decimal? TaxaRef,
    decimal? ValorTotal = null,
    int? Contraparte = null,
    EnumCarteira? CarteiraContraparte = null
)
{
    public EnumOperacaoSituacao Situacao { get; set; } = EnumOperacaoSituacao.Registrada;
    public readonly DateTimeOffset DataHoraRegistro = DateTimeOffset.Now;
    public override string ToString() => $"Ope Op:{IdOperacao,4} To:{TipoOperacao,-20} Si:{Situacao,-10} (Co:{Parte}/Ka:{CarteiraParte} > Cp:{Contraparte}/Kp:{CarteiraContraparte}) Ti:{Titulo,3} Qt:{Quantidade,6} Pu:{PURef,6} Tx:{TaxaRef,6} Dr:{DataHoraRegistro:mm:ss.fff}";
}