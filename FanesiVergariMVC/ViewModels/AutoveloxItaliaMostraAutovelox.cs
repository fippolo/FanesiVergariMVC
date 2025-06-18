using FanesiVergari.Modelli;

namespace FanesiVergariMVC.ViewModels
{
    public class AutoveloxItaliaMostraAutovelox
    {
        public Autovelox[] ListaAutovelox { get; set; }
        public string[] RegioniDisponibili { get; set; }
        public string RegioneSelezionata { get; set; }
        public string TermineRicerca { get; set; }
        public int TotaleAutovelox { get; set; }
    }
}