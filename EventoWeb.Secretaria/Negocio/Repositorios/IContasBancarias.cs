using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Negocio.Repositorios;

public interface IContasBancarias : IPersistencia<ContaBancaria>
{
    IList<ContaBancaria> ListarTodos();
}

