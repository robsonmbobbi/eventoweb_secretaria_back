using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.ObjetosValor;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Negocio.Servicos.Contas;

public class SrvLiquidacaoConta
{
    private readonly IPersistencia<Conta> m_Contas;
    private readonly IContasBancarias m_ContasBancarias;

    public SrvLiquidacaoConta(
        IPersistencia<Conta> contas,
        IContasBancarias contasBancarias)
    {
        m_Contas = contas ?? throw new ArgumentNullException(nameof(contas));
        m_ContasBancarias = contasBancarias ?? throw new ArgumentNullException(nameof(contasBancarias));
    }

    public void Liquidar(int idConta, int idContaBancaria, decimal valorPago)
    {
        // 1. Obter e validar Conta
        var conta = m_Contas.Obter(idConta) 
            ?? throw new Exception($"Conta com ID {idConta} não encontrada");
        
        // 2. Obter e validar ContaBancaria
        var contaBancaria = m_ContasBancarias.Obter(idContaBancaria)
            ?? throw new Exception($"Conta Bancária com ID {idContaBancaria} não encontrada");
        
        // 3. Calcular diferença
        decimal diferenca = valorPago - conta.Valor.Valor;
        
        ValorMonetario? juros = null;
        ValorMonetario? desconto = null;
        
        if (diferenca > 0)
        {
            // Valor pago > Saldo da conta
            juros = new ValorMonetario(diferenca);
        }
        else if (diferenca < 0)
        {
            // Valor pago < Saldo da conta
            desconto = new ValorMonetario(Math.Abs(diferenca));
        }
        
        // 4. Adicionar transação à conta
        conta.AdicionarTransacao(
            contaBancaria,
            DateTime.Now,
            new ValorMonetario(valorPago),
            null,  // multa
            juros,
            desconto
        );
        
        // 5. Persistir
        m_Contas.Atualizar(conta);
    }
}
