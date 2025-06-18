using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanesiVergari.Modelli
{
    public class Autovelox
    {
        public string cprovincia { get; set; }
        public string ccomune { get; set; }
        public string cregione { get; set; }
        public string cindirizzo { get; set; }
        public string clatitudine { get; set; }
        public string clongitudine { get; set; }
        public string ctipo { get; set; }
        public string cnote { get; set; }
        public string canno_inserimento { get; set; }
        public DateTime cdata_e_ora_inserimento { get; set; }
        public string cidentificatore_in_openstreetmap { get; set; }
    }
}