using EventoWeb.Comum.Negocio.Entidades;

namespace EventoWeb.Secretaria.Negocio.Entidades.Quartos
{
    public class QuartoInscrito : Entidade
    {
        private Inscricao m_Inscricao;
        private Quarto m_Quarto;

        public QuartoInscrito(Quarto quarto, Inscricao inscrito, Boolean ehCoordenador)
        {
            m_Quarto = quarto ?? throw new ArgumentNullException(nameof(quarto));
            m_Inscricao = inscrito ?? throw new ArgumentNullException(nameof(inscrito));
            EhCoordenador = ehCoordenador;
        }

        protected QuartoInscrito() { }

        public virtual Quarto Quarto { get { return m_Quarto; } }
        public virtual Inscricao Inscricao { get { return m_Inscricao; } }
        public virtual Boolean EhCoordenador { get; set; }
    }
}
