using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Net.Http.Headers;

namespace SPMA.Controllers.Shared
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : Controller
    {
        #region Properties
        private IHostingEnvironment _hostingEnvironment;
        #endregion

        #region Constructor
        public UploadController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region Htttp Action Requests
        /// <summary>
        /// Uploads file to a specific folderName passed in header
        /// </summary>
        /// <returns></returns>
        [HttpPost, DisableRequestSizeLimit]
        public ActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                StringValues folderName;
                Request.Headers.TryGetValue("FileUploadPath", out folderName);
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, "Upload" ,folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Json("Pomyślnie wgrano plik.");
            }
            catch (Exception ex)
            {
                return Json("Niepowodzenie: " + ex.Message);
            }
        }
        #endregion
    }
}