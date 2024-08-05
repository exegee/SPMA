using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace SPMA.Controllers.Vault
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaultController : ControllerBase
    {
        private Thread _tLoginToVault;

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            string result = null;

            //using (WebServiceManager manager = new WebServiceManager(new UserPasswordCredentials(new ServerIdentities() { FileServer = "192.168.0.22", DataServer = "192.168.0.22" }, "Projekty", "adminmp", "1999kosmos", true)))
            //{
            //    Folder folder = manager.DocumentService.GetFolderRoot();
            //    result = folder.Name;
            //}

            return Ok(result);
        }



    }
}