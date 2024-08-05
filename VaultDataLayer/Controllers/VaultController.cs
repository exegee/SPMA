using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VaultDataLayer.Models;

namespace VaultDataLayer.Controllers
{
    public class VaultController : ApiController
    {
        [HttpGet]
        public List<VaultFile> GetFiles(string searchfor)
        {
            List<VaultFile> vaultFiles = new List<VaultFile>();
            File[] files;
            using (WebServiceManager manager = new WebServiceManager(new UserPasswordCredentials(new ServerIdentities() { FileServer = "192.168.0.22", DataServer = "192.168.0.22" }, "Projekty", "adminmp", "1999kosmos", true)))
            {
                //Get folder root
                Folder rootFolder = manager.DocumentService.GetFolderRoot();

                //Get file properties definition
                PropDef[] filePropDefs = manager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE");

                //Get file name property definition
                PropDef fileNamePropDef = filePropDefs.Single(n => n.SysName == "Name");

                //Initialize empty search status
                SrchStatus srchStatus = null;
                //Initialize new search condition
                SrchCond srchCond = new SrchCond();
                //Initialize empty bookmark
                string bookmark = string.Empty;

                //Defince search criteria
                srchCond.SrchTxt = searchfor + "*";
                srchCond.SrchOper = 1;
                srchCond.PropDefId = fileNamePropDef.Id;

                SrchCond[] srchConds = new SrchCond[1];
                srchConds[0] = srchCond;

                long[] folderIds = new long[1];
                folderIds[0] = rootFolder.Id;

                files = manager.DocumentService.FindFilesBySearchConditions(srchConds, null, null, true, true, ref bookmark, out srchStatus);

                
                foreach (File file in files)
                {
                    BOM bom = manager.DocumentService.GetBOMByFileId(file.Id);
                    vaultFiles.Add(
                        new VaultFile() { MasterId = file.MasterId, FileName = file.Name }
                        );
                }
            }
            return vaultFiles;
        }

        [HttpPost]
        public List<VaultFile> GetFileWithDependencies(VaultFile vaultFile)
        {
            List<VaultFile> vaultFiles = new List<VaultFile>();
            vaultFile.Level = 0;
            vaultFiles.Add(vaultFile);

            using (WebServiceManager manager = new WebServiceManager(new UserPasswordCredentials(new ServerIdentities() { FileServer = "192.168.0.22", DataServer = "192.168.0.22" }, "Projekty", "adminmp", "1999kosmos", true)))
            {


                var deps = manager.DocumentService.GetLatestFileAssociationsByMasterIds(
                    new long[] { vaultFile.MasterId },
                    FileAssociationTypeEnum.None,
                    false,
                    FileAssociationTypeEnum.Dependency,
                    true,
                    false,
                    false,
                    true
                    );

                

                //Get file properties definition
                //PropDef[] filePropDefs = manager.PropertyService.GetPropertyDefinitionsByEntityClassId("BOM");

                var fileAssocs = deps[0].FileAssocs;
                foreach (FileAssoc fileAssoc in fileAssocs) 
                {

                    File child = fileAssoc.CldFile;
                    BOM childBom = manager.DocumentService.GetBOMByFileId(child.Id);

                    File parent = fileAssoc.ParFile;
                    BOM parentBom = manager.DocumentService.GetBOMByFileId(parent.Id);


                    var currentParent = vaultFiles.Find(f => f.MasterId == parent.MasterId);
                    double qty = 1;

                    if (parentBom != null)
                        qty = parentBom.InstArray[0].Quant;
                    

                    vaultFiles.Add(new VaultFile()
                    {
                        MasterId = child.MasterId,
                        FileName = child.Name,
                        Quantity = qty,
                        ParentFile = new VaultFile()
                        {
                            MasterId = parent.MasterId,
                            FileName = parent.Name
                        },
                        Level = currentParent.Level + 1
                    }); ;

                }
                return vaultFiles;
            }
        }
    }
}
