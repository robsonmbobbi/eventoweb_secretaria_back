using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.ObjetosValor;
using System.Linq;

namespace EventoWeb.Secretaria.Negocio.Entidades.Atividades
{
    public class DivisaoAtividade: Entidade
    {
        private Atividade m_Atividade;
        private string m_Nome;
        private IList<DivisaoAtividadeParticipante> m_Participantes;
        private FaixaEtaria? m_FaixaEtaria;
        private bool m_DeveSerParNumeroTotalParticipantes;
        private int? m_NumeroTotalParticipantes;
        private int? m_NumeroMinimoParticipantes;

        public DivisaoAtividade(Atividade atividade, string nome)
        {
            m_Participantes = new List<DivisaoAtividadeParticipante>();

            if (atividade == null)
                throw new ArgumentNullException(nameof(atividade), $"{nameof(atividade)} não pode ser nulo.");

            m_Atividade = atividade;
            Nome = nome;
        }

        protected DivisaoAtividade() { }

        public virtual Atividade Atividade { get { return m_Atividade; } }

        public virtual string Nome 
        {
            get { return m_Nome; }
            set 
            { 
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Nome","O nome não pode ser vazio.");

                m_Nome = value; 
            }
        }

        public virtual IEnumerable<DivisaoAtividadeParticipante> Participantes { get { return m_Participantes; } }

        public virtual FaixaEtaria FaixaEtaria
        {
            get { return m_FaixaEtaria; }
            set
            {
                m_FaixaEtaria = value;
            }
        }

        public virtual bool DeveSerParNumeroTotalParticipantes
        {
            get => m_DeveSerParNumeroTotalParticipantes; 
            set => m_DeveSerParNumeroTotalParticipantes = value;
        }

        public virtual int? NumeroTotalParticipantes
        {
            get { return m_NumeroTotalParticipantes; }
            set
            {
                if (value != null && DeveSerParNumeroTotalParticipantes && value % 2 != 0)
                    throw new ArgumentException("O número de participantes deve ser par.", "NumeroTotalParticipantes");

                m_NumeroTotalParticipantes = value;
            }
        }

        public virtual int? NumeroMinimoParticipantes
        {
            get { return m_NumeroMinimoParticipantes; }
            set
            {
                if (value != null && DeveSerParNumeroTotalParticipantes && value % 2 != 0)
                    throw new ArgumentException("O número de participantes deve ser par.", nameof(NumeroMinimoParticipantes));

                m_NumeroMinimoParticipantes = value;
            }
        }



        public virtual void AdicionarParticipante(Inscricao participante, bool ehCoordenador = false)
        {
            ValidarSeParticipanteEhNulo(participante);
            ValidarSeParticipanteEhMesmoEvento(participante);

            if (!ehCoordenador)
            {
                var idade = DataAniversario.ObterIdadeAnos(participante.Pessoa.DataNascimento!.Data, m_Atividade.Evento.PeriodoRealizacaoEvento.DataInicial);

                if (m_FaixaEtaria != null &&
                    (idade < m_FaixaEtaria.IdadeMin ||
                    idade > m_FaixaEtaria.IdadeMax))
                    throw new ArgumentException("Participante fora da faixa etária definida para esta sala.");
            }

            if (!EstaNaListaDeParticipantes(participante))
                m_Participantes.Add(new DivisaoAtividadeParticipante(this, participante, ehCoordenador));
        }

        public virtual void RemoverParticipante(DivisaoAtividadeParticipante participante)
        {
            ValidarSeParticipanteEhNulo(participante.Inscricao);

            if (!EstaNaListaDeParticipantes(participante.Inscricao))
                throw new ArgumentException("Participante não existe na lista de participantes desta atividade.", nameof(participante));

            m_Participantes.Remove(participante);
        }

        public virtual void RemoverTodosParticipantes()
        {
            m_Participantes.Clear();
        }

        public virtual void RemoverTodosParticipantesNaoCoordenadores()
        {
            var coordenadores = m_Participantes.Where(x => x.EhCoordenador).ToList();
            m_Participantes.Clear();
            foreach (var coordenador in coordenadores)
                m_Participantes.Add(coordenador);
        }

        public virtual bool EstaNaListaDeParticipantes(Inscricao participante)
        {
            return m_Participantes.Any(x => x.Inscricao == participante);
        }

        private void ValidarSeParticipanteEhNulo(Inscricao participante)
        {
            ArgumentNullException.ThrowIfNull(participante);
        }

        private void ValidarSeParticipanteEhMesmoEvento(Inscricao participante)
        {
            if (participante.Evento != m_Atividade.Evento)
                throw new ArgumentException("Participante deve ser do mesmo evento da sala.", nameof(participante));
        }
    }
}
