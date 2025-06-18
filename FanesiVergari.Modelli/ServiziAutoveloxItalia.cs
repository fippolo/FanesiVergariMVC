using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanesiVergari.Modelli
{
    public static class ServiziAutoveloxItalia
    {
        public static async Task<Autovelox[]> ElencoAutoveloxItalia()
        {
            string BaseUri = "http://www.datiopen.it/export/json/Mappa-degli-autovelox-in-italia.json";

            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(BaseUri);
            string contents = await response.Content.ReadAsStringAsync();

            Autovelox[] ListaAutoveloxItalia = JsonConvert.DeserializeObject<Autovelox[]>(contents);

            return ListaAutoveloxItalia.ToArray();
        }

        public static async Task<Autovelox[]> ElencoAutoveloxPerRegione(string regione)
        {
            var tuttiAutovelox = await ElencoAutoveloxItalia();

            if (string.IsNullOrEmpty(regione))
                return tuttiAutovelox;

            return tuttiAutovelox.Where(a => a.cregione != null &&
                                           a.cregione.Equals(regione, StringComparison.OrdinalIgnoreCase))
                                .ToArray();
        }

        public static async Task<string[]> ElencoRegioniDisponibili()
        {
            var tuttiAutovelox = await ElencoAutoveloxItalia();

            return tuttiAutovelox.Where(a => !string.IsNullOrEmpty(a.cregione))
                                .Select(a => a.cregione)
                                .Distinct()
                                .OrderBy(r => r)
                                .ToArray();
        }
    }
}