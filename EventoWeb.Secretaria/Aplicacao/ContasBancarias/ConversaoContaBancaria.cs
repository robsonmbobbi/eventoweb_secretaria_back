using EventoWeb.Comum.Negocio.Entidades.Financeiro;

namespace EventoWeb.Secretaria.Aplicacao.ContasBancarias;

public static class ConversaoContaBancaria
{
    public static DTOContaBancaria Converter(this ContaBancaria contaBancaria)
    {
        return new DTOContaBancaria
        {
            Id = contaBancaria.Id,
            NomeConta = contaBancaria.NomeConta.Nome
        };
    }
}
