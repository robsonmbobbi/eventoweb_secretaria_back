using EventoWeb.Secretaria.Aplicacao.Seguranca;

namespace eventoweb_secretaria_back.Controllers.DTOS
{
    public class DTWAutenticacao
    {
        public required DTOUsuario Usuario { get; set; }
        public required string TokenAutenticacao { get; set; }
        public required DateTime Validade { get; set; }
    }
}
