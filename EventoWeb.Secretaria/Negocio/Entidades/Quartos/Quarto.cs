using EventoWeb.Comum.Negocio.Entidades;

namespace EventoWeb.Secretaria.Negocio.Entidades.Quartos
{
    public class Quarto : Entidade
    {
        private String m_Nome;
        private Evento m_Evento;
        private int? m_Capacidade;
        private IList<QuartoInscrito> m_Inscritos;

        public Quarto(Evento evento, string nome, Boolean ehFamilia, EnumSexoQuarto sexo)
        {
            Nome = nome;
            Evento = evento;
            Sexo = sexo;
            AtribuirSexoEEhFamilia(ehFamilia, sexo);
            m_Inscritos = new List<QuartoInscrito>();
        }

        protected Quarto() { }

        public virtual Evento Evento
        {
            get { return m_Evento; }
            protected set
            {
                m_Evento = value ?? throw new ArgumentException("Evento não pode ser nulo", nameof(Evento));
            }
        }

        public virtual string Nome
        {
            get { return m_Nome; }
            set
            {
                if (value == null) 
                    ArgumentNullException.ThrowIfNull(value, "Nome não pode ser nulo.");

                if (value == "")
                    ArgumentException.ThrowIfNullOrEmpty(nameof(Nome), "Nome não pode ser vazio.");

                m_Nome = value;
            }
        }

        public virtual int? Capacidade
        {
            get { return m_Capacidade; }
            set
            {
                if (value != null && value <= 0)
                    throw new ArgumentException("A capacidade informada deve ser maior que zero.", nameof(Capacidade));

                m_Capacidade = value;
            }
        }

        public virtual Boolean EhFamilia { get; protected set; }

        public virtual EnumSexoQuarto Sexo { get; protected set; }


        public virtual IEnumerable<QuartoInscrito> Inscritos { get { return m_Inscritos; } }

        public virtual void AtribuirSexoEEhFamilia(bool ehFamilia, EnumSexoQuarto sexo)
        {
            if (!ehFamilia && sexo == EnumSexoQuarto.Misto)
                throw new ArgumentException("Somente quarto família pode ter o sexo misto.");

            EhFamilia = ehFamilia;
            Sexo = sexo;
        }

        public virtual void AdicionarInscrito(Inscricao inscrito, Boolean ehCoordenador = false)
        {
            if (inscrito.Evento != Evento)
                throw new ArgumentException("A inscrição não é do mesmo evento do quarto.");

            if (inscrito is InscricaoParticipante && this.Sexo != EnumSexoQuarto.Misto && (int)inscrito.Pessoa.Sexo != (int)this.Sexo)
                throw new ArgumentException("O inscrito não é do mesmo sexo definido para o quarto.");

            if (Capacidade != null && m_Inscritos.Count == Capacidade.Value)
                throw new ArgumentException("Nâo é possível incluir mais participantes neste quarto.");


            m_Inscritos.Add(new QuartoInscrito(this, inscrito, ehCoordenador));
        }

        public virtual void RemoverInscrito(QuartoInscrito inscrito)
        {
            if (!m_Inscritos.Remove(inscrito))
                throw new ArgumentException("Nâo existe este inscrito no quarto.");
        }

        public virtual void RemoverTodosInscritos()
        {
            m_Inscritos.Clear();
        }
    }
}
