using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;

namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao
{
    public static class ConversorDTORegistroIntegracaoLog
    {
        public static DTORegistroIntegracaoLog Converter(this RegistroIntegracaoLog log)
        {
            return new DTORegistroIntegracaoLog
            {
                Mensagem = log.Mensagem,
                Tipo = log.Tipo,
                Data = log.Data,
                Dados = log.Dados
            };
        }
    }
}
