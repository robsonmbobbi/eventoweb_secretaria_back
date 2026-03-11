using EventoWeb.Secretaria.Negocio.Entidades.Seguranca;

namespace EventoWeb.Secretaria.Aplicacao.Seguranca
{
    public static class ConversorUsuario
    {
        public static DTOUsuario? Converter(this Usuario usuario)
        {
            if (usuario == null)
                return null;
            else
                return new DTOUsuario
                {
                    Login = usuario.Login,
                    Nome = usuario.Nome,
                    EhAdministrador = usuario.EhAdministrador,
                };
        }
    }
}
