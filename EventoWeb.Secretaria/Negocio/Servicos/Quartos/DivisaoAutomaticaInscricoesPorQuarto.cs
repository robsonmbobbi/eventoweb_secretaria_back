using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Quartos;
using EventoWeb.Secretaria.Negocio.Repositorios;

namespace EventoWeb.Secretaria.Negocio.Servicos.Quartos
{
    public class ValidacaoDivisaoQuartos
    {
        public void ValidarGeral(IEnumerable<Quarto> quartos, IEnumerable<IGrouping<EnumSexo, Inscricao>> inscritosPorSexo)
        {
            foreach (var inscricaoSexo in inscritosPorSexo)
            {
                if (quartos.Count(x => (int)x.Sexo == (int)inscricaoSexo.Key) == 0)
                    throw new InvalidOperationException("Não há quartos cadastrados para o sexo " + inscricaoSexo.Key.ToString());

                if (quartos.Count(x => (int)x.Sexo == (int)inscricaoSexo.Key && x.Capacidade == null) == 0 &&
                    inscricaoSexo.Count() > quartos.Where(x => (int)x.Sexo == (int)inscricaoSexo.Key).Sum(x => x.Capacidade.Value))
                    throw new InvalidOperationException(
                        String.Format("O total de vagas dos quartos do sexo {0} não são suficientes para os inscritos.",
                            inscricaoSexo.Key.ToString()));
            }
        }

        public void ValidarFamilia(IEnumerable<Quarto> quartos, IEnumerable<InscricaoFamilia> inscritos)
        {
            // Validar se há capacidade para atender a todos
            if (quartos.Count(x => x.Capacidade == null) == 0 &&
                inscritos.Sum(x=> x.Criancas.Count() + 1) > quartos.Sum(x=>x.Capacidade))
                throw new InvalidOperationException(
                       "O total de vagas dos quartos família não são suficientes para os inscritos.");

            var familiasMisto = inscritos
                .Where(x => x.Criancas.Count(y => y.Pessoa.Sexo != x.Responsavel.Pessoa.Sexo) > 0);

            if (familiasMisto.Count() > 0 && quartos.Count(x => x.Sexo == EnumSexoQuarto.Misto) == 0)
            {
                var mensagem = "";
                foreach(var familia in familiasMisto)
                {
                    mensagem = mensagem + "Responsável: " + familia.Responsavel.Pessoa.Nome + "\n";
                    foreach (var crianca in familia.Criancas.Where(x => x.Pessoa.Sexo != familia.Responsavel.Pessoa.Sexo))
                        mensagem = mensagem + "    - " + crianca.Pessoa.Nome + "\n";
                }

                throw new InvalidOperationException(
                       "Há responsáveis de sexo diferente da criança, e não existe nenhum quarto misto cadastrado para atendê-los.\n\n" +
                       mensagem);
            }

            if (inscritos
                .Count(x=>x.Responsavel.Pessoa.Sexo == EnumSexo.Masculino
                          && x.Criancas.Count(y=>y.Pessoa.Sexo != x.Responsavel.Pessoa.Sexo) == 0) > 0 &&
                quartos.Count(x => x.Sexo == EnumSexoQuarto.Misto || x.Sexo == EnumSexoQuarto.Masculino) == 0)
                throw new InvalidOperationException("Não há quartos família cadastrados para o sexo Masculino");

            if (inscritos
                .Count(x => x.Responsavel.Pessoa.Sexo == EnumSexo.Feminino
                          && x.Criancas.Count(y => y.Pessoa.Sexo != x.Responsavel.Pessoa.Sexo) == 0) > 0 &&
                quartos.Count(x => x.Sexo == EnumSexoQuarto.Misto || x.Sexo == EnumSexoQuarto.Feminino) == 0)
                throw new InvalidOperationException("Não há quartos família cadastrados para o sexo Feminino");           
        }
    }

    public class InscricaoFamilia
    {
        public Inscricao Responsavel { get; set; }
        public IList<InscricaoInfantil> Criancas { get; set; }
    }

    public class DivisaoAutomaticaInscricoesPorQuarto
    {
        private IQuartos mRepQuartos;
        private IInscricoes mRepInscricoes;
        private Evento mEvento;

        public DivisaoAutomaticaInscricoesPorQuarto(Evento evento, IInscricoes inscricoes, IQuartos quartos)
        {
            mEvento = evento;
            mRepInscricoes = inscricoes;
            mRepQuartos = quartos;
        }

        public IList<Quarto> Dividir()
        {
            var quartos = mRepQuartos.ListarTodosQuartosPorEvento(mEvento.Id);
            var listaInscricoes = mRepInscricoes.ListarTodasInscricoesAceitasComPessoasDormemEvento(mEvento.Id);

            var criancas = listaInscricoes
                .Where(x => x is InscricaoInfantil && x.Pessoa.DataNascimento!.CalcularIdadeEmAnos(mEvento.PeriodoRealizacaoEvento.DataInicial) <= 6)
                .Select(x => (InscricaoInfantil)x)
                .ToList();

            var inscricoesFamilia = new List<InscricaoFamilia>();
            foreach (var crianca in criancas)
            {
                Inscricao responsavel1 = null;
                Inscricao responsavel2 = null;
                Inscricao responsavelDefinido = null;

                if (crianca.InscricaoResponsavel1.DormeEvento)
                    responsavel1 = crianca.InscricaoResponsavel1;

                if (crianca.InscricaoResponsavel2 != null && crianca.InscricaoResponsavel2.DormeEvento)
                    responsavel2 = crianca.InscricaoResponsavel2;

                if ((responsavel1 != null && responsavel1.Pessoa.Sexo == crianca.Pessoa.Sexo) ||
                    (responsavel2 != null && responsavel2.Pessoa.Sexo == crianca.Pessoa.Sexo))
                {
                    if (responsavel1 != null && responsavel1.Pessoa.Sexo == crianca.Pessoa.Sexo)
                        responsavelDefinido = responsavel1;
                    else
                        responsavelDefinido = responsavel2;
                }
                else if (responsavel1 != null)
                    responsavelDefinido = responsavel1;
                else
                    responsavelDefinido = responsavel2;

                var item = inscricoesFamilia.SingleOrDefault(x => x.Responsavel == responsavelDefinido);
                if (item != null)
                    item.Criancas.Add(crianca);
                else
                {
                    var familia = new InscricaoFamilia();
                    familia.Responsavel = responsavelDefinido;
                    familia.Criancas = new List<InscricaoInfantil>() { crianca };

                    inscricoesFamilia.Add(familia);

                    listaInscricoes.Remove(responsavelDefinido);
                }

                listaInscricoes.Remove(crianca);
            }

            var inscritosGeral = listaInscricoes
                .Where(x => !(x is InscricaoInfantil && x.Pessoa.DataNascimento!.CalcularIdadeEmAnos(mEvento.PeriodoRealizacaoEvento.DataInicial) <= 6))
                .ToList();

            var inscritosGeralPorSexo = inscritosGeral.GroupBy(x => x.Pessoa.Sexo);
            var quartosGeral = quartos.Where(x=>!x.EhFamilia);
            var quartosFamilia = quartos.Where(x => x.EhFamilia);

            /*var validacao = new ValidacaoDivisaoQuartos();
            validacao.ValidarGeral(quartosGeral, inscritosGeralPorSexo);
            validacao.ValidarFamilia(quartosFamilia, inscricoesFamilia);*/

            foreach (var quarto in quartos)
                quarto.RemoverTodosInscritos();

            RealizarDivisaoGeral(quartosGeral, inscritosGeralPorSexo);
            RealizarDivisaoFamilia(quartosFamilia, inscricoesFamilia);

            foreach (var quarto in quartos)
                mRepQuartos.Atualizar(quarto);

            return quartos;
        }

        private void RealizarDivisaoGeral(IEnumerable<Quarto> quartosGeral, IEnumerable<IGrouping<EnumSexo?, Inscricao>> inscritosGeralPorSexo)
        {
            var dataAtual = DateTime.Today;
            foreach (var inscritosPorSexo in inscritosGeralPorSexo)
            {
                var inscritosOrdenadosIdadeCidade = inscritosPorSexo
                    .OrderBy(x => x.Pessoa.DataNascimento!.CalcularIdadeEmAnos(x.Evento.PeriodoRealizacaoEvento.DataInicial))
                    .ThenBy(x => x.Pessoa.Cidade);

                var listaQuartos = quartosGeral.Where(x=> (int)x.Sexo == (int)inscritosPorSexo.Key).ToList();
                var indice = 0;
                foreach (var inscrito in inscritosOrdenadosIdadeCidade)
                {
                    if (listaQuartos.Any())
                    {
                        listaQuartos[indice].AdicionarInscrito(inscrito);
                        if (listaQuartos[indice].Capacidade != null &&
                             listaQuartos[indice].Inscritos.Count() == listaQuartos[indice].Capacidade.Value)
                            listaQuartos.RemoveAt(indice);

                        indice++;
                        if (indice >= listaQuartos.Count)
                            indice = 0;
                    }
                }
            }
        }

        private void RealizarDivisaoFamilia(IEnumerable<Quarto> quartosFamilia, IEnumerable<InscricaoFamilia> inscritosFamilia)
        {
            var listaInscritos = inscritosFamilia.ToList();
            var inscritosPorSexoDoMesmoSexo = listaInscritos
                .Where(x => x.Criancas.Count(y => y.Pessoa.Sexo != x.Responsavel.Pessoa.Sexo) == 0)
                .GroupBy(x => x.Responsavel.Pessoa.Sexo);
            
            foreach (var inscritosPorSexo in inscritosPorSexoDoMesmoSexo)
            {
                var listaQuartos = quartosFamilia.Where(x => (int)x.Sexo == (int)inscritosPorSexo.Key).ToList();
                
                foreach (var familia in inscritosPorSexo)
                {
                    var indice = 0;
                    //var encontrouQuarto = false;
                    while (indice < listaQuartos.Count /*&& !encontrouQuarto*/)
                    {
                        if (listaQuartos[indice].Capacidade == null ||
                            (listaQuartos[indice].Capacidade != null &&
                             listaQuartos[indice].Inscritos.Count() + familia.Criancas.Count + 1 <= listaQuartos[indice].Capacidade.Value))
                        {
                            listaQuartos[indice].AdicionarInscrito(familia.Responsavel);
                            foreach (var crianca in familia.Criancas)
                                listaQuartos[indice].AdicionarInscrito(crianca);

                            listaInscritos.Remove(familia);
                            //encontrouQuarto = true;
                        }

                        indice++;
                    }
                }
            }

            var quartosMisto = quartosFamilia.Where(x => x.Sexo == EnumSexoQuarto.Misto).ToList();
            foreach(var familia in listaInscritos)
            {
                var indice = 0;
                //var encontrouQuarto = false;
                while (indice < quartosMisto.Count /*&& !encontrouQuarto*/)
                {
                    if (quartosMisto[indice].Capacidade == null ||
                        (quartosMisto[indice].Capacidade != null &&
                         quartosMisto[indice].Inscritos.Count() + familia.Criancas.Count + 1 <= quartosMisto[indice].Capacidade.Value))
                    {
                        quartosMisto[indice].AdicionarInscrito(familia.Responsavel);
                        foreach (var crianca in familia.Criancas)
                            quartosMisto[indice].AdicionarInscrito(crianca);

                        //encontrouQuarto = true;
                    }

                    indice++;
                }

                /*if (!encontrouQuarto)
                {
                    var mensagem = "Não há um quarto família que comporte o responsável " +
                        familia.Responsavel.Pessoa.Nome + " e as crianças ";
                    foreach (var crianca in familia.Criancas)
                        mensagem = mensagem + crianca.Pessoa.Nome + ", ";

                    mensagem = mensagem.Substring(0, mensagem.Length - 2) + " juntos. Verifique os cadastros de quartos.";

                    throw new InvalidOperationException(mensagem);
                }*/
            }
        }
    }
}
