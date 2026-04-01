using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces
{
    public class DadosCadernoInscrito
    {
        public required int Id { get; set; }
        public required String Nome { get; set; }
        public required String Cidade { get; set; }
        public required String UF { get; set; }
        public String? Quarto { get; set; }
        public IList<DadosAtividadeInscrito> Atividades { get; set; } = [];
    }
}
