using System;

namespace WebCorrida
{
    public class Piloto
    {
        public int posicao { get; set; }
        public String Numero { get; set; }
        public String Nome { get; set; }
        public DateTime Hora { get; set; }
        public String Volta { get; set; }
        public string MelhorVolta { get; set; }
        public TimeSpan TempoVolta { get; set; }
        public decimal VelocidadeMediaVolta { get; set; }
        public TimeSpan TempoTotal { get; set; }
    }
}