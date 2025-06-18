using Microsoft.AspNetCore.Mvc;
using FanesiVergariMVC.ViewModels;
using FanesiVergari.Modelli;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace FanesiVergariMVC.Controllers
{
    public class AutoveloxController : Controller
    {
        private readonly ILogger<AutoveloxController> _logger;
        private readonly string _soapServiceUrl = "http://localhost:5000/ServizioAutoveloxItalia.wsdl"; // HTTP invece di HTTPS

        public AutoveloxController(ILogger<AutoveloxController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> MostraAutovelox(string regione = "", string ricerca = "")
        {
            _logger.LogInformation("Richiesta MostraAutovelox - Regione: {Regione}, Ricerca: {Ricerca}", regione, ricerca);

            var viewModel = new AutoveloxItaliaMostraAutovelox
            {
                RegioneSelezionata = regione,
                TermineRicerca = ricerca
            };

            try
            {
                _logger.LogInformation("Connessione al servizio SOAP: {Url}", _soapServiceUrl);

                // Configurazione del binding SOAP 1.2 per HTTP (senza sicurezza)
                var binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.None; // Nessuna sicurezza per HTTP
                binding.MessageEncoding = WSMessageEncoding.Text;
                binding.MaxReceivedMessageSize = 10000000; // 10MB per gestire grandi risposte
                binding.ReceiveTimeout = TimeSpan.FromMinutes(2); // Timeout di 2 minuti

                // Configurazione per SOAP 1.2
                var customBinding = new CustomBinding(binding);
                var textElement = customBinding.Elements.Find<TextMessageEncodingBindingElement>();
                if (textElement != null)
                {
                    textElement.MessageVersion = MessageVersion.Soap12WSAddressing10;
                }

                var endpoint = new EndpointAddress(_soapServiceUrl);
                var channelFactory = new ChannelFactory<IServizioAutoveloxItaliaSoap>(customBinding, endpoint);

                var client = channelFactory.CreateChannel();

                _logger.LogInformation("Client SOAP creato con successo");

                // 1. Ottenere le regioni disponibili
                _logger.LogInformation("Recupero regioni disponibili...");
                viewModel.RegioniDisponibili = await client.GetElencoRegioniDisponibili();
                _logger.LogInformation("Trovate {Count} regioni", viewModel.RegioniDisponibili?.Length ?? 0);

                // 2. Ottenere gli autovelox
                Autovelox[] autovelox;
                if (!string.IsNullOrEmpty(regione))
                {
                    _logger.LogInformation("Recupero autovelox per regione: {Regione}", regione);
                    autovelox = await client.GetElencoAutoveloxPerRegione(regione);
                }
                else
                {
                    _logger.LogInformation("Recupero tutti gli autovelox...");
                    autovelox = await client.GetElencoAutoveloxItalia();
                }

                _logger.LogInformation("Recuperati {Count} autovelox dal servizio SOAP", autovelox?.Length ?? 0);

                // 3. Applicare filtro di ricerca se presente
                if (!string.IsNullOrEmpty(ricerca) && autovelox != null)
                {
                    _logger.LogInformation("Applicazione filtro di ricerca: {Ricerca}", ricerca);
                    var ricercaLower = ricerca.ToLowerInvariant();

                    autovelox = autovelox.Where(a =>
                        (a.ccomune != null && a.ccomune.ToLowerInvariant().Contains(ricercaLower)) ||
                        (a.cprovincia != null && a.cprovincia.ToLowerInvariant().Contains(ricercaLower)) ||
                        (a.cindirizzo != null && a.cindirizzo.ToLowerInvariant().Contains(ricercaLower)) ||
                        (a.cregione != null && a.cregione.ToLowerInvariant().Contains(ricercaLower))
                    ).ToArray();

                    _logger.LogInformation("Dopo filtro ricerca: {Count} autovelox", autovelox.Length);
                }

                // 4. Aggiornare il viewModel
                viewModel.ListaAutovelox = autovelox ?? new Autovelox[0];
                viewModel.TotaleAutovelox = viewModel.ListaAutovelox.Length;

                // 5. Cleanup delle risorse
                try
                {
                    ((ICommunicationObject)client).Close();
                    channelFactory.Close();
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "Errore durante la chiusura del client SOAP");
                }

                _logger.LogInformation("Operazione completata con successo. Totale autovelox: {Total}", viewModel.TotaleAutovelox);
            }
            catch (EndpointNotFoundException ex)
            {
                _logger.LogError(ex, "Servizio SOAP non raggiungibile: {Url}", _soapServiceUrl);
                ViewBag.ErrorMessage = "Il servizio degli autovelox non è disponibile. Assicurati che il servizio SOAP sia avviato su HTTP.";
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, "Timeout durante la connessione al servizio SOAP");
                ViewBag.ErrorMessage = "Il servizio degli autovelox ha impiegato troppo tempo a rispondere. Riprova più tardi.";
            }
            catch (CommunicationException ex)
            {
                _logger.LogError(ex, "Errore di comunicazione con il servizio SOAP");
                ViewBag.ErrorMessage = "Errore di comunicazione con il servizio degli autovelox. Verifica la connessione.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore generico durante il recupero dei dati");
                ViewBag.ErrorMessage = $"Errore imprevisto: {ex.Message}";
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FiltraAutovelox(string regione, string ricerca)
        {
            _logger.LogInformation("Filtro applicato - Regione: {Regione}, Ricerca: {Ricerca}", regione, ricerca);

            // Sanitizza i parametri
            regione = string.IsNullOrWhiteSpace(regione) ? "" : regione.Trim();
            ricerca = string.IsNullOrWhiteSpace(ricerca) ? "" : ricerca.Trim();

            return RedirectToAction("MostraAutovelox", new { regione = regione, ricerca = ricerca });
        }

        // Action per ottenere le regioni via AJAX (opzionale)
        [HttpGet]
        public async Task<IActionResult> GetRegioni()
        {
            try
            {
                var binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.None; // HTTP senza sicurezza

                var customBinding = new CustomBinding(binding);
                var textElement = customBinding.Elements.Find<TextMessageEncodingBindingElement>();
                if (textElement != null)
                {
                    textElement.MessageVersion = MessageVersion.Soap12WSAddressing10;
                }

                var endpoint = new EndpointAddress(_soapServiceUrl);
                var channelFactory = new ChannelFactory<IServizioAutoveloxItaliaSoap>(customBinding, endpoint);
                var client = channelFactory.CreateChannel();

                var regioni = await client.GetElencoRegioniDisponibili();

                ((ICommunicationObject)client).Close();
                channelFactory.Close();

                return Json(regioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle regioni via AJAX");
                return Json(new string[0]);
            }
        }

        // Action per statistiche (opzionale)
        public async Task<IActionResult> Statistiche()
        {
            try
            {
                var binding = new BasicHttpBinding();
                binding.Security.Mode = BasicHttpSecurityMode.None; // HTTP senza sicurezza

                var customBinding = new CustomBinding(binding);
                var textElement = customBinding.Elements.Find<TextMessageEncodingBindingElement>();
                if (textElement != null)
                {
                    textElement.MessageVersion = MessageVersion.Soap12WSAddressing10;
                }

                var endpoint = new EndpointAddress(_soapServiceUrl);
                var channelFactory = new ChannelFactory<IServizioAutoveloxItaliaSoap>(customBinding, endpoint);
                var client = channelFactory.CreateChannel();

                var autovelox = await client.GetElencoAutoveloxItalia();
                var regioni = await client.GetElencoRegioniDisponibili();

                ((ICommunicationObject)client).Close();
                channelFactory.Close();

                var statistiche = new
                {
                    TotaleAutovelox = autovelox.Length,
                    TotaleRegioni = regioni.Length,
                    AutoveloxPerRegione = autovelox.GroupBy(a => a.cregione)
                                                  .Select(g => new { Regione = g.Key, Conteggio = g.Count() })
                                                  .OrderByDescending(x => x.Conteggio)
                                                  .Take(10),
                    ProvinceTop = autovelox.GroupBy(a => a.cprovincia)
                                          .Select(g => new { Provincia = g.Key, Conteggio = g.Count() })
                                          .OrderByDescending(x => x.Conteggio)
                                          .Take(10)
                };

                return Json(statistiche);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel calcolo delle statistiche");
                return StatusCode(500, "Errore nel calcolo delle statistiche");
            }
        }
    }

    // Interfaccia per il client SOAP (deve corrispondere all'interfaccia del servizio)
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