using EventoWeb.Comum.Aplicacao;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Aplicacao.ContasBancarias;

public class AppContaBancariaListagem : AppBase
{
    private readonly IContasBancarias m_ContasBancarias;

    public AppContaBancariaListagem(
        IContexto contexto,
        IContasBancarias contasBancarias) : base(contexto)
    {
        m_ContasBancarias = contasBancarias ?? throw new ArgumentNullException(nameof(contasBancarias));
    }

    public IEnumerable<DTOContaBancaria> Listar()
    {
        var resultado = new List<DTOContaBancaria>();

        ExecutarSeguramente(() =>
        {
            var contas = m_ContasBancarias.ListarTodos();
            resultado = contas.Select(c => c.Converter()).ToList();
        });

        return resultado;
    }
}
