using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace VaultDataLayer.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values/5
        public List<string> Get(string searchfor)
        {
            File[] files;
            List<string> fileNames = new List<string>();
            //string rootFolder;
            using (WebServiceManager manager = new WebServiceManager(new UserPasswordCredentials(new ServerIdentities() { FileServer = "192.168.0.22", DataServer = "192.168.0.22" }, "Projekty", "adminmp", "1999kosmos", true)))
            {
                Folder rootFolder = manager.DocumentService.GetFolderRoot();
                PropDef[] filePropDefs = manager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");
                PropDef fileNamePropDef = filePropDefs.Single(n => n.SysName == "Name");
                //rootFolder = folder.Name;
                SrchStatus srchStatus = null;
                SrchCond srchCond = new SrchCond();
                SrchSort srchSort = new SrchSort();
                string bookmark = string.Empty;
                srchCond.SrchTxt = searchfor + "*";
                srchCond.SrchOper = 1;
                srchCond.PropDefId = fileNamePropDef.Id;

                SrchSort[] srchSorts = new SrchSort[1];
                srchSorts[0] = srchSort;

                SrchCond[] srchConds = new SrchCond[1];
                srchConds[0] = srchCond;

                long[] folderIds = new long[1];
                folderIds[0] = rootFolder.Id;

                files = manager.DocumentService.FindFilesBySearchConditions(srchConds, null, null, true, true, ref bookmark, out srchStatus);
                //var f = manager.DocumentService.GetLatestFileAssociationsByMasterIds(
                //    new long[] {files[0].MasterId},
                //    FileAssociationTypeEnum.None,
                //    false,
                //    FileAssociationTypeEnum.Dependency,
                //    true,
                //    false,
                //    false,
                //    true
                //    );
                foreach (File file in files)
                {
                    fileNames.Add(file.Name);
                }
            }

            return fileNames;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
