using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FanesiVergari.Modelli
{
    [DataContract]
    public class Autovelox
    {
        [DataMember]
        public string cprovincia { get; set; }

        [DataMember]
        public string ccomune { get; set; }

        [DataMember]
        public string cregione { get; set; }

        [DataMember]
        public string cindirizzo { get; set; }

        [DataMember]
        public string clatitudine { get; set; }

        [DataMember]
        public string clongitudine { get; set; }

        [DataMember]
        public string ctipo { get; set; }

        [DataMember]
        public string cnote { get; set; }

        [DataMember]
        public string canno_inserimento { get; set; }

        [DataMember]
        public DateTime cdata_e_ora_inserimento { get; set; }

        [DataMember]
        public string cidentificatore_in_openstreetmap { get; set; }
    }
}