namespace eventoweb_secretaria_back.Controllers.DTOS
{
    public class DTOAlteracaoSenhaComumWS : DTOAlteracaoSenhaWS
    {
        public required string SenhaAtual { get; set; }
    }
}
