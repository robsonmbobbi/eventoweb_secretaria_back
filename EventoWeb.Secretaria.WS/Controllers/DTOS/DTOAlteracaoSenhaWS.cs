namespace eventoweb_secretaria_back.Controllers.DTOS
{
    public class DTOAlteracaoSenhaWS
    {
        public required string NovaSenha { get; set; }
        public required string NovaSenhaRepetida { get; set; }
    }
}
