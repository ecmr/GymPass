using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebCorrida.Controllers
{
    public class HomeController : Controller
    {
        readonly DateTime dateTime = new DateTime();

        public int Ano { get { return dateTime.Year; } }
        public int Mes { get { return dateTime.Month; } }
        public int Dia { get { return dateTime.Day; } }


        public ActionResult Index()
        {
            IList<String> lista = new List<String>();
            List<Piloto> pilotos = new List<Piloto>();

            StreamReader logFile = new StreamReader(Server.MapPath("~/Models/logCorrida.txt"));
            
            if (logFile != null)
            {
                String linha;
                Piloto piloto;

                while ((linha = logFile.ReadLine()) != null)
                {
                    if (linha.Substring(0, 4) == "Hora")
                        linha = logFile.ReadLine();

                    linha = linha.Replace("\t", " ");

                    piloto = new Piloto()
                    {
                        Hora = new DateTime(Ano, Mes, Dia, int.Parse(RetornaHora(linha)), int.Parse(RetornaMinuto(linha)), int.Parse(RetornaSegundo(linha)), int.Parse(RetornaMileSegundo(linha))),
                        Numero = linha.Substring(19, 3),
                        Nome = linha.Substring(24, 13),
                        Volta = linha.Substring(48, 1),
                        TempoVolta = RetornaTempoVolta(linha),
                        VelocidadeMediaVolta = RetornaVMVolta(linha)
                    };

                    lista.Add(linha);
                    pilotos.Add(piloto);
                }
                
            }



            pilotos = PosicaoChegada(pilotos);

            return View(pilotos);
        }

        private List<Piloto> PosicaoChegada(List<Piloto> pilotos)
        {

             var polotoOrdem = from ordemPiloto in pilotos group ordemPiloto by ordemPiloto.Numero into lista select lista;


            List<Piloto> _pilotos = new List<Piloto>();

            foreach (var pOrdem in polotoOrdem)
            {
                Piloto pp = new Piloto();

                foreach (var p in pOrdem)
                {
                    pp.Numero = p.Numero;
                    pp.TempoVolta += new TimeSpan(0, (0), (p.TempoVolta.Minutes), (p.TempoVolta.Seconds), (p.TempoVolta.Milliseconds));
                    pp.Hora = p.Hora;
                    pp.Nome = p.Nome;
                    pp.Volta = p.Volta;
                    var res = pOrdem.OrderByDescending(a => a.TempoVolta);
                    pp.MelhorVolta = res.LastOrDefault().Volta + " - " + res.LastOrDefault().TempoVolta.ToString();
                    pp.VelocidadeMediaVolta = p.VelocidadeMediaVolta;
                    pp.TempoTotal += p.TempoVolta;
                }
                _pilotos.Add(pp);

            }

 
            var tempoCorrida =  _pilotos.OrderBy(y => y.TempoVolta);

            List<Piloto> ordemChegada = new List<Piloto>();
            int posicao = 1;

            foreach (Piloto p in tempoCorrida)
            {

                Piloto piloto = new Piloto()
                {
                    posicao = posicao,
                    Nome = p.Nome,
                    Hora = p.Hora,
                    Numero = p.Numero,
                    TempoVolta = p.TempoVolta,
                    Volta = p.Volta,
                    MelhorVolta = p.MelhorVolta,
                    VelocidadeMediaVolta = p.VelocidadeMediaVolta,
                    TempoTotal = p.TempoTotal
                };
                posicao++;
                ordemChegada.Add(piloto);
            }
                

            return ordemChegada;
        }

        private String RetornaHora(string data)
        {
            return data.Substring(0, 2).PadLeft(2, '0');
        }

        private String RetornaMinuto(string data)
        {
            return data.Substring(3, 2).PadLeft(2, '0');
        }

        private String RetornaSegundo(string data)
        {
            return data.Substring(6, 2).PadLeft(2, '0');
        }

        private String RetornaMileSegundo(String data)
        {
            return data.Substring(9, 3).PadLeft(3, '0');
        }

        private TimeSpan RetornaTempoVolta(String data)
        {
            return new TimeSpan(0, 0, int.Parse(data.Substring(60, 1)), int.Parse(data.Substring(62, 2)), int.Parse(data.Substring(65, 3)));
        }

        private Decimal RetornaVMVolta(String data)
        {
            if (data.Length < 86)
                return decimal.Parse(data.Substring(80, 5).Replace(",", "."));
            return decimal.Parse(data.Substring(80, 6).Replace(",", "."));
        }

        private List<Piloto> PosicaoChegada(StreamReader logFile)
        {
            List<Piloto> pilotos = new List<Piloto>();
            string linha;

            //while ((linha = logFile.ReadLine()) != null)
            //{



            //    pilotos.Add(linha);
            //}

            return pilotos;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}