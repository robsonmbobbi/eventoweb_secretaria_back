using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.ObjetosValor;

namespace EventoWeb.Secretaria.Negocio.Entidades.Atividades
{
    public class Atividade : Entidade
    {
        public IList<AtividadeTipoParticipante> m_TipoParticipantes;
        public IList<DivisaoAtividade> m_Divisoes;
        private NomeCompleto m_Nome;

        public Atividade(Evento evento, EnumTipoInscricao tipoInscricao, NomeCompleto nome) 
        { 
            Evento = evento;
            TipoInscricao = tipoInscricao;
            Nome = nome;
            m_TipoParticipantes = [];
            m_Divisoes = [];
        }

        protected Atividade() { }

        public virtual Evento Evento { get; set; }
        
        public virtual NomeCompleto Nome 
        {
            get => m_Nome;
            set
            {
                m_Nome = value ?? throw new ArgumentNullException(nameof(Nome));
            }
        }

        public virtual EnumTipoInscricao TipoInscricao { get; set; }
        public virtual IEnumerable<AtividadeTipoParticipante> TipoParticipantes { get; protected set; }
        public virtual IEnumerable<DivisaoAtividade> Divisoes { get; protected set; }


        public void AdicionarTipoParticipante(EnumTipoParticipante tipoParticipante)
        {
            if (TipoInscricao == EnumTipoInscricao.Infantil)
            {
                throw new InvalidOperationException("Esta atividade aceita apenas inscrições do tipo Infantil e por isso não é possível informar tipos de participantes");
            }

            if (!m_TipoParticipantes.Any(x => x.TipoParticipante == tipoParticipante))
            {
                m_TipoParticipantes.Add(new AtividadeTipoParticipante(this, tipoParticipante));
            }
        }

        public void RemoverTipoParticipante(AtividadeTipoParticipante tipoParticipante)
        {
            if (!m_TipoParticipantes.Remove(tipoParticipante))
            {
                throw new InvalidOperationException("Tipo de participante não encontrado na lista desta atividade");
            }
        }
    }
}
