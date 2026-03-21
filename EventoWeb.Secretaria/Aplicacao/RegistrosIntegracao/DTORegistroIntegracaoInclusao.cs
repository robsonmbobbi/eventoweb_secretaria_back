namespace EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao;

public class DTORegistroIntegracaoInclusao
{
    public int IdConta { get; set; }
    public int IdFormaPagamento { get; set; }
    public int IdEvento { get; set; }
    public decimal Valor { get; set; }
    public int? NumeroParcelas { get; set; }
}
