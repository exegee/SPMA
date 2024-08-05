using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SPMA.Controllers.Production;
using SPMA.Core.Production;
using SPMA.Data;
using SPMA.Dtos.Production;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Production;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using static SPMA.Core.Enums;

namespace SPMA.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportOrderController : ControllerBase
    {
        #region Properties
        private IHostingEnvironment _hostingEnvironment;
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private ComponentsController _componentsController;
        #endregion

        #region Constructor
        public ImportOrderController(IHostingEnvironment hostingEnvironment, ApplicationDbContext dbContext,
            ComponentsController componentsController, IMapper mapper)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = dbContext;
            _mapper = mapper;
            _componentsController = componentsController;
        }
        #endregion

        #region Http Action Requests
        /// <summary>
        /// Gets sorted BOM list
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(string fileName)
        {
            
            // Define new empty component list
            List<ComponentDto> components = new List<ComponentDto>();

            // Set the upload paths
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Upload\\xlsOrders", fileName);

            // Get file info
            FileInfo fileInfo = new FileInfo(newPath);
            if (!fileInfo.Exists)
                return BadRequest("Cannot find specified file");
            
                // Extract data from xls file
                using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

                // Search criteria filters
                Regex numberMatchPattern = new Regex(@"^\d{3}\.\d{2}");
                string[] plazmaCutFilter = new string[] { "Palone", "Palenie" };
                
                int order = 1;

                // Iterate each row in xls file
                for (var rowNum = 2; rowNum <= workSheet.Dimension.End.Row; rowNum++)
                {

                    // Get the values from each column
                    var name = Convert.ToString(workSheet.GetValue(rowNum, 2));
                    var number = workSheet.GetValue(rowNum, 3).ToString();
                    var quantity = Convert.ToInt16(workSheet.GetValue(rowNum, 4));
                    var materialType = (workSheet.GetValue(rowNum, 5) == null) ? "" : workSheet.GetValue(rowNum, 5).ToString();
                    var weightVal = workSheet.GetValue(rowNum, 6).ToString();
                    var description = (workSheet.GetValue(rowNum, 7) == null) ? "" : workSheet.GetValue(rowNum, 7).ToString();
                    float? weight = 0.0f;
                    //var name = "";
                    //var number = "";
                    //var quantity = 0;
                    //var materialType = "";
                    //var weightVal = "";
                    //float? weight = 0.0f;
                    //var description = "";

                    //try
                    //{
                    //    // Get the values from each column
                    //    name = Convert.ToString(workSheet.GetValue(rowNum, 2));
                    //    number = workSheet.GetValue(rowNum, 3).ToString();
                    //    quantity = Convert.ToInt16(workSheet.GetValue(rowNum, 4));
                    //    materialType = (workSheet.GetValue(rowNum, 5) == null) ? "" : workSheet.GetValue(rowNum, 5).ToString();
                    //    weightVal = workSheet.GetValue(rowNum, 6).ToString();
                    //    description = (workSheet.GetValue(rowNum, 7) == null) ? "" : workSheet.GetValue(rowNum, 7).ToString();
                    //}
                    //catch
                    //{
                    //    continue;
                    //}


                    string decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    try
                    {
                        if(decSep == ",")
                        {
                            weight = Convert.ToSingle(weightVal.Remove(weightVal.Length - 3));
                        }
                        else
                        {
                            weight = Convert.ToSingle(weightVal.Remove(weightVal.Length - 3).Replace(".",","));
                        }
                        
                    }
                    catch(Exception e)
                    {
                        return BadRequest(e);
                    }
                    var comment = (workSheet.GetValue(rowNum, 7) == null) ? "" : workSheet.GetValue(rowNum, 7).ToString();
                    
                    var file = workSheet.GetValue(rowNum, 8).ToString();
                    var fileExtension = file.Substring(file.Length - 3, 3);
                  
                    ComponentDto componentIdDb;
                    
                        componentIdDb = _mapper.Map<Component, ComponentDto>(_context.Components.Where(c => c.Number == number).Include(i => i.Ware).SingleOrDefault());
                    

                    int id = (componentIdDb != null) ? componentIdDb.ComponentId : 0;

                    // Check if component exists in database
                    bool existInDb = (componentIdDb != null) ? true : false;
                    // Check for assosiated ware with the component
                    WareDto ware = (componentIdDb != null) ? componentIdDb.Ware : null;
                    // Check whether component is a part or assembly 
                    ComponentType componentType = (fileExtension == "ipt") ? ComponentType.Part : ComponentType.Assembly;

                    // Declare sourceType
                    ComponentSourceType sourceType;
                    bool isAdditionallyPurchasable = false;

                    // Sort components
                    if (numberMatchPattern.IsMatch(number))
                    {
                        // Check and get source type of the component
                        if(componentIdDb != null)
                        {
                            sourceType = (ComponentSourceType)componentIdDb.LastSourceType;
                            isAdditionallyPurchasable = componentIdDb.IsAdditionallyPurchasable;
                        }
                        else
                        {
                            if (comment.Contains(plazmaCutFilter[0]) || comment.Contains(plazmaCutFilter[1]))
                            {
                                // Plasma component
                                sourceType = (componentIdDb != null) ? (ComponentSourceType)componentIdDb.LastSourceType : ComponentSourceType.PlasmaIn;
                            }
                            else
                            {
                                // Standard component
                                sourceType = (componentIdDb != null) ? (ComponentSourceType)componentIdDb.LastSourceType : ComponentSourceType.Standard;
                            }
                        }
                        
                    }
                    // If component do not match the search criteria then component is purchase item
                    else
                    {
                        // Purchase component
                        sourceType = (componentIdDb != null) ? (ComponentSourceType)componentIdDb.LastSourceType : ComponentSourceType.Purchase;
                    }


                    // Add component to components list
                    components.Add(new ComponentDto()
                    {
                        ComponentId = id,
                        Name = name,
                        Number = number,
                        Quantity = quantity,
                        MaterialType = materialType,
                        Weight = weight,
                        Description = description,
                        ComponentType = (sbyte)componentType,
                        Comment = comment,
                        ExistInDatabase = existInDb,
                        LastSourceType = (sbyte)sourceType,
                        TreeNumber = workSheet.GetValue(rowNum, 1).ToString(),
                        Level = workSheet.GetValue(rowNum, 1).ToString().Count(c => c == '.') + 1,
                        Order = order,
                        Ware = ware,
                        SinglePieceQty = quantity,
                        IsAdditionallyPurchasable = isAdditionallyPurchasable
                    });
                    order++;
                    
                }
                
            }
            // Multiply quantities
            //components = MultiplyQuantities(components);
            // Count duplicates
            components = CountDuplicates(components);

            return Ok(components);
            
        }
        #endregion

        // To be moved later
        #region Helper methods

        /// <summary>
        /// Helper method that multiply quantities of child components
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        private List<ComponentDto> MultiplyQuantities(List<ComponentDto> components)
        {
            // Check if components is null
            if (components == null)
                return null;
            components.OrderBy(c => c.Order);

            int[] q = new int[10] { 1,1,1,1,1,1,1,1,1,1 };
            int[] m = new int[10] { 1,1,1,1,1,1,1,1,1,1 };
            for (int i = 0; i <= components.Count-1; i++)
            {
                var component = components[i];
                int level = component.Level;

                component.SinglePieceQty *= m[level];
                q[level] = component.Quantity;
                component.Quantity *= m[level];

                if (component.ComponentType == (int)ComponentType.Assembly)
                {
                    

                    m[level + 1] = 1;
                    for (int n = 0; n <= level; n++)
                    {
                        m[level+1] *= q[n];
                    }
                }
                
                
            }

            return components;
        }


        /// <summary>
        /// Helper method for counting duplicates
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        private List<ComponentDto> CountDuplicates(List<ComponentDto> components)
        {
            if (components == null)
                return null;
            // Get duplicates for each list
            var duplicates = components.GetDuplicates();

            components.ForEach(item =>
            {
                // Check if any number in components list match the duplicates list
                if (duplicates.Any(x => x.Number == item.Number))
                {
                    // Change the SumQuantity value - FirstOrDefault returns default value for that field
                    item.SumQuantity = duplicates.Where(z => z.Number == item.Number).Select(s => s.Quantity).FirstOrDefault();
                }
            });

            //duplicates = components.storeBoughtComponents.GetDuplicates();

            //components.storeBoughtComponents.ForEach(item =>
            //{
            //    Check if any number in components list match the duplicates list
            //    if (duplicates.Any(x => x.Number == item.Number))
            //    {
            //        Change the SumQuantity value -FirstOrDefault returns default value for that field

            //      item.SumQuantity = duplicates.Where(z => z.Number == item.Number).Select(s => s.Quantity).FirstOrDefault();
            //    }
            //});

            return components;
        }
        #endregion
    }
}