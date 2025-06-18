using FanesiVergariMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FanesiVergariMVC.Controllers
{
    public class TestController : Controller
    {
        public IActionResult TestAction()
        {
            // ....
            TestTestActionViewModel vm = new TestTestActionViewModel();
            vm.nome = "riccardo";
            vm.cognome = "Cognini";
            vm.elencoLavori = new string[2] { "professore", "programmatore" };

            return View(vm);
        }
    }
}
