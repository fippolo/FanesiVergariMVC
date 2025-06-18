using FanesiVergari.Modelli;
using System.ServiceModel;

namespace Soap_FanesiVergari.WSSoap
{
    public class ServizioAutoveloxItalia : IServizioAutoveloxItalia
    {
        public async Task<Autovelox[]> GetElencoAutoveloxItalia()
        {
            return await ServiziAutoveloxItalia.ElencoAutoveloxItalia();
        }

        public async Task<Autovelox[]> GetElencoAutoveloxPerRegione(string regione)
        {
            return await ServiziAutoveloxItalia.ElencoAutoveloxPerRegione(regione);
        }

        public async Task<string[]> GetElencoRegioniDisponibili()
        {
            return await ServiziAutoveloxItalia.ElencoRegioniDisponibili();
        }
    }
}