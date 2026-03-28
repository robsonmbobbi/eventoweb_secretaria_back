using EventoWeb.Comum.Negocio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventoWeb.Secretaria.Negocio.Entidades.Atividades
{
    public class AtividadeTipoParticipante : Entidade
    {

        public AtividadeTipoParticipante(Atividade atividade, EnumTipoParticipante tipoParticipante)
        {
            Atividade = atividade;
            TipoParticipante = tipoParticipante;
        }

        protected AtividadeTipoParticipante() { }

        public virtual Atividade Atividade { get; protected set;  }
        public virtual EnumTipoParticipante TipoParticipante { get; protected set; }
    }
}
