namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public class DTOUsuario
    {
        public string Login { get; set; }
        public string Nome { get; set; }
        public bool EhAdministrador { get; set; }
    }

    public class DTOUsuarioInclusao : DTOUsuario
    {
        public string Senha { get; set; }
        public string RepeticaoSenha { get; set; }
    }
}
