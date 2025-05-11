using cahefe.UseCases;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Custodia.Modelos;

namespace GestaoPosicao.Custodia;

public class ReservarQuantidades(Alocacoes alocacoes, Reservas reservas) : IUseCase<DtoReserva, Reserva>
{
    public Task<Result<Reserva>> Execute(DtoReserva dto, CancellationToken cancellationToken = default)
    {
        //  Verificar se já existe reserva para a operação
        if (reservas.Any(r => r.IdOperacao == dto.IdOperacao))
            return Task.FromResult(Result<Reserva>.GoFails(new Fail(-1, "CUSTODIA_RESERVA_EXISTENTE", $"Já existe uma reserva para a operação {dto.IdOperacao}", FailLevel.Error)));

        var alocacao = alocacoes.FirstOrDefault(a => a.Conta == dto.Conta && a.Carteira == dto.Carteira && a.Titulo == dto.Titulo);
        if (alocacao is null)
            return Task.FromResult(Result<Reserva>.GoFails(new Fail(-1, "CUSTODIA_RESERVA_ALOCACAO_INDISPONÍVEL", $"Não há saldo disponível para reserva", FailLevel.Error)));

        var reserva = new Reserva(dto.IdOperacao, dto.Conta, dto.Carteira, dto.Titulo, dto.Quantidade);
        reservas.Add(reserva);

        return Task.FromResult(Result<Reserva>.GoSuccess(reserva));
    }
}
