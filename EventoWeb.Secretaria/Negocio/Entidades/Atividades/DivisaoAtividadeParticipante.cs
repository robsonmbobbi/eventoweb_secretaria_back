using EventoWeb.Comum.Negocio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventoWeb.Secretaria.Negocio.Entidades.Atividades
{
    public class DivisaoAtividadeParticipante : Entidade
    {
        public DivisaoAtividadeParticipante(DivisaoAtividade divisao, Inscricao inscricao, bool ehCoordenador)
        {
            Divisao = divisao;
            Inscricao = inscricao;
            EhCoordenador = ehCoordenador;
        }

        protected DivisaoAtividadeParticipante() { }

        public virtual DivisaoAtividade Divisao { get; protected set; }
        public virtual Inscricao Inscricao { get; protected set; }
        public virtual bool EhCoordenador { get; protected set; }
    }
}
