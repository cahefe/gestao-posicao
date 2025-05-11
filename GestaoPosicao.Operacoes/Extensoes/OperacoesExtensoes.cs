using cahefe.UseCases;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Modelos;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoPosicao.Operacoes.Extensoes;

public static class OperacoesExtensoes
{
    public static IServiceCollection IncluirCasosUsoOperacoes(this IServiceCollection services) => services
        .AddScoped<Pipeline<DtoOperacao, Operacao>>()
        .AddScoped<Bag<DtoOperacao, Operacao>>()
        .AddTransient<RegistrarPrepararPipeline>()
        .AddTransient<RegistrarValidarCotacao>()
        .AddTransient<RegistrarValidarQuantidade>()
        .AddTransient<RegistrarValidarPartes>()
        .AddTransient<RegistrarObterNumeroOperacao>()
        .AddTransient<RegistrarReservarQuantidade>()
        .AddTransient<RegistrarPersistirOperacao>();

}
