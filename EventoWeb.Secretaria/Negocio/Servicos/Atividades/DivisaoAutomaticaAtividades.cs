using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Negocio.Servicos.Atividades
{
    public class DivisaoAutomaticaAtividades
    {
        private Evento m_Evento;
        private IInscricoes m_Inscricoes;
        private IAtividades m_Atividades;

        public DivisaoAutomaticaAtividades(Evento evento, IInscricoes inscricoes, IAtividades atividades)
        {
            m_Evento = evento;
            m_Inscricoes = inscricoes;
            m_Atividades = atividades;
        }

        public void Dividir(Atividade atividade)
        {
            var participantes = m_Inscricoes.ListarPorSituacao(m_Evento.Id, EnumSituacaoInscricao.Aceita);
            participantes = participantes
                .Where(x => x is InscricaoParticipante part && (part.Tipo! == EnumTipoParticipante.Participante || part.Tipo! == EnumTipoParticipante.ParticipanteTrabalhador))
                .ToList();

            IList <DivisaoAtividade> divisoes = atividade.Divisoes.ToList();
            if (!divisoes.Any())
                throw new InvalidOperationException("Não há divisões de atividade.");

            foreach (var divisao in divisoes)
                divisao.RemoverTodosParticipantesNaoCoordenadores();

            var coordenadores = divisoes.SelectMany(x => x.Participantes);
            participantes = participantes.Where(x => !coordenadores.Any(c => c.Inscricao.Id == x.Id)).ToList();

            int indiceDivisao = 0;

            foreach (var participante in participantes
                                            .OrderByDescending(x => x.Pessoa.DataNascimento.Data)
                                            .ThenBy(x=>x.Pessoa.Cidade))
            {
                divisoes[indiceDivisao].AdicionarParticipante(participante);

                indiceDivisao++;
                if (indiceDivisao == divisoes.Count)
                    indiceDivisao = 0;
            }

            m_Atividades.Atualizar(atividade);
        }
    }
}
