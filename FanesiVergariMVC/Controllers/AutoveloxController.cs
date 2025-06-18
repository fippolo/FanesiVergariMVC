using Microsoft.AspNetCore.Mvc;
using FanesiVergariMVC.ViewModels;
using System.ServiceModel;
using FanesiVergari.Modelli;

namespace FanesiVergariMVC.Controllers
{
    public class AutoveloxController : Controller
    {
        private readonly string _soapServiceUrl = "https://localhost:7295/ServizioAutoveloxItalia.wsdl"; // Modifica con l'URL del tuo servizio SOAP

        public async Task<IActionResult> MostraAutovelox(string regione = "", string ricerca = "")
        {
            var viewModel = new AutoveloxItaliaMostraAutovelox();

            try
            {
                // Connessione al servizio SOAP
                var binding = new BasicHttpBinding();
                var endpoint = new EndpointAddress(_soapServiceUrl);
                var channelFactory = new ChannelFactory<IServizioAutoveloxItaliaSoap>(binding, endpoint);
                var client = channelFactory.CreateChannel();

                // Ottenere le regioni disponibili
                viewModel.RegioniDisponibili = await client.GetElencoRegioniDisponibili();
                viewModel.RegioneSelezionata = regione;
                viewModel.TermineRicerca = ricerca;

                // Ottenere gli autovelox
                Autovelox[] autovelox;
                if (!string.IsNullOrEmpty(regione))
                {
                    autovelox = await client.GetElencoAutoveloxPerRegione(regione);
                }
                else
                {
                    autovelox = await client.GetElencoAutoveloxItalia();
                }

                // Applicare filtro di ricerca se presente
                if (!string.IsNullOrEmpty(ricerca))
                {
                    autovelox = autovelox.Where(a =>
                        (a.ccomune != null && a.ccomune.Contains(ricerca, StringComparison.OrdinalIgnoreCase)) ||
                        (a.cprovincia != null && a.cprovincia.Contains(ricerca, StringComparison.OrdinalIgnoreCase)) ||
                        (a.cindirizzo != null && a.cindirizzo.Contains(ricerca, StringComparison.OrdinalIgnoreCase))
                    ).ToArray();
                }

                viewModel.ListaAutovelox = autovelox;
                viewModel.TotaleAutovelox = autovelox.Length;

                // Chiudere la connessione
                channelFactory.Close();
            }
            catch (Exception ex)
            {
                // Log dell'errore
                ViewBag.ErrorMessage = $"Errore nel recupero dei dati: {ex.Message}";
                viewModel.ListaAutovelox = new Autovelox[0];
                viewModel.RegioniDisponibili = new string[0];
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult FiltraAutovelox(string regione, string ricerca)
        {
            return RedirectToAction("MostraAutovelox", new { regione = regione, ricerca = ricerca });
        }
    }

    // Interfaccia per il client SOAP
    [ServiceContract]
    public interface IServizioAutoveloxItaliaSoap
    {
        [OperationContract]
        Task<Autovelox[]> GetElencoAutoveloxItalia();

        [OperationContract]
        Task<Autovelox[]> GetElencoAutoveloxPerRegione(string regione);

        [OperationContract]
        Task<string[]> GetElencoRegioniDisponibili();
    }
}