using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public static class ConversorDTORegistroIntegracao
    {
        public static DTORegistroIntegracao Converter(this RegistroIntegracaoFinanceira registro)
        {
            return new DTORegistroIntegracao
            {
                Id = registro.Id,
                IdentificacaoNoIntegrador = registro.IdentificacaoNoIntegrador.Valor,
                Situacao = registro.Situacao,
                Tipo = registro.Tipo,
                Valor = registro.Valor.Valor,
                DataRegistro = registro.DataRegistro,
                NumeroParcelas = registro.NumeroParcelas?.Valor,
                DataConcluidoAbortado = registro.DataConcluidoAbortado,
                Logs = registro.Logs?.Select(l => l.Converter()).ToList() ?? new List<DTORegistroIntegracaoLog>()
            };
        }
    }
}
