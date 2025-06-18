using Microsoft.AspNetCore.Mvc;
using Soap_FanesiVergari.WSSoap;
using FanesiVergari.Modelli;
using FanesiVergariMVC.ViewModels;

namespace FanesiVergariMVC.Controllers
{
    public class AutoveloxController : Controller
    {
        public async Task<IActionResult> MostraAutovelox(string regione = "", string ricerca = "")
        {
            var viewModel = new AutoveloxItaliaMostraAutovelox
            {
                RegioneSelezionata = regione,
                TermineRicerca = ricerca,
                ListaAutovelox = new Autovelox[0],
                RegioniDisponibili = new string[0],
                TotaleAutovelox = 0
            };

            try
            {
                // Istanzia direttamente il servizio SOAP
                var soapService = new ServizioAutoveloxItalia();

                // Chiama i metodi direttamente
                viewModel.RegioniDisponibili = await soapService.GetElencoRegioniDisponibili();

                Autovelox[] autovelox;
                if (string.IsNullOrEmpty(regione))
                {
                    autovelox = await soapService.GetElencoAutoveloxItalia();
                }
                else
                {
                    autovelox = await soapService.GetElencoAutoveloxPerRegione(regione);
                }

                // Applica filtro ricerca
                if (!string.IsNullOrEmpty(ricerca))
                {
                    var searchLower = ricerca.ToLower();
                    autovelox = autovelox.Where(a =>
                        (a.ccomune?.ToLower().Contains(searchLower) == true) ||
                        (a.cprovincia?.ToLower().Contains(searchLower) == true) ||
                        (a.cindirizzo?.ToLower().Contains(searchLower) == true)
                    ).ToArray();
                }

                viewModel.ListaAutovelox = autovelox ?? new Autovelox[0];
                viewModel.TotaleAutovelox = viewModel.ListaAutovelox.Length;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Errore: {ex.Message}";
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FiltraAutovelox(string regione, string ricerca)
        {
            return RedirectToAction("MostraAutovelox", new
            {
                regione = regione?.Trim() ?? "",
                ricerca = ricerca?.Trim() ?? ""
            });
        }

        public async Task<IActionResult> Debug()
        {
            try
            {
                var soapService = new ServizioAutoveloxItalia();
                var autovelox = await soapService.GetElencoAutoveloxItalia();

                var debug = $"TOTALE: {autovelox?.Length ?? 0}\n";
                if (autovelox?.Length > 0)
                {
                    var primo = autovelox[0];
                    debug += $"Primo: {primo?.ccomune} - {primo?.cprovincia}\n";
                }

                return Content(debug, "text/plain");
            }
            catch (Exception ex)
            {
                return Content($"ERRORE: {ex.Message}", "text/plain");
            }
        }
    }
}