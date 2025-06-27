using FanesiVergari.Modelli;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Rest_Fanesi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public Task<Autovelox[]> Get()
        {
            return ServiziAutoveloxItalia.ElencoAutoveloxItalia();
        }

        [HttpPost]
        public Task<Autovelox[]> GetPerRegione([FromBody] string regione)
        {
            return ServiziAutoveloxItalia.ElencoAutoveloxPerRegione(regione);
        }

        [HttpGet]
        public Task<string[]> GetRegioni()
        {
            return ServiziAutoveloxItalia.ElencoRegioniDisponibili();
        }
    }
}
