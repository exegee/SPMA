using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMA.Data;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Optima;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPMA.Controllers.Optima
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptimaController : ControllerBase
    {
        #region Properties
        private OptimaDbContext _optimaDbContext;
        #endregion

        #region Constructor
        public OptimaController(OptimaDbContext optimaDbContext)
        {
            _optimaDbContext = optimaDbContext;
        }
        #endregion

        #region Htttp Action Requests
        /// <summary>
        /// Gets latest warehouse items by item code
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetWarehouseItems(string filter, string magName, string magIdStr)
        {
            int magId = Int16.Parse(magIdStr);

            List<OptimaWare> items;
            if (magId == 0)
            {
                List<OptimaMag> mags = (GetMags("true") as OkObjectResult).Value as List<OptimaMag>;
                string query = @"WITH Tow AS(SELECT Towary.Twr_TwrId,
					Towary.Twr_NieAktywny,
					Towary.Twr_Kod,
					Towary.Twr_Nazwa,
					Towary.Twr_JM,
					Towary.Twr_JmPomPrzelicznikL,
					Ilosci.TwI_Ilosc,
					TwI_Data,
					Grupy.TwG_Kod,
					Grupy.TwG_Nazwa,
					Magazyny.Mag_Nazwa,
					Magazyny.Mag_MagId,
					Magazyny.Mag_Symbol FROM CDN.Towary AS Towary
                    LEFT JOIN CDN.TwrIlosci AS Ilosci
                    ON Towary.Twr_TwrId = Ilosci.TwI_TwrId 
                    LEFT JOIN CDN.Magazyny As Magazyny
                    ON Ilosci.TwI_MagId = Magazyny.Mag_MagId
                    LEFT JOIN CDN.TwrGrupy AS Grupy
                    On Towary.Twr_TwGGIDNumer = Grupy.TwG_GIDNumer
                    WHERE Towary.Twr_Kod LIKE {0}
                    AND Grupy.TwG_GIDTyp = -16
                    AND Towary.Twr_NieAktywny = 0
                    AND 
                    (
                    Mag_Symbol = {1}
                    OR Mag_Symbol = {2}
                    OR Mag_Symbol = {3}
                    OR Mag_Symbol = {4}
                    OR Mag_Symbol = {5}
                    OR Mag_Symbol = {6}
                    OR Mag_Symbol IS NULL
                    ))
                    SELECT * FROM Tow WHERE (TwI_Data = ((SELECT MAX(TwI_Data) FROM CDN.TwrIlosci AS IlosciData WHERE Tow.Twr_TwrId =  IlosciData.TwI_TwrId AND Mag_MagId = TwI_MagId)) OR TwI_Data IS NULL )";
                items = _optimaDbContext.OptimaWares.FromSql(
                    query, "%" + filter + "%",
                    mags[2].Mag_Symbol,
                    mags[3].Mag_Symbol,
                    mags[4].Mag_Symbol,
                    mags[5].Mag_Symbol,
                    mags[6].Mag_Symbol,
                    mags[7].Mag_Symbol)
                    .ToList();
            }
            else if (magId > 0)
            {
                string query = @"WITH Tow AS(SELECT Towary.Twr_TwrId,
					Towary.Twr_NieAktywny,
					Towary.Twr_Kod,
					Towary.Twr_Nazwa,
					Towary.Twr_JM,
					Towary.Twr_JmPomPrzelicznikL,
					Ilosci.TwI_Ilosc,
					TwI_Data,
					Grupy.TwG_Kod,
					Grupy.TwG_Nazwa,
					Magazyny.Mag_Nazwa,
					Magazyny.Mag_MagId,
					Magazyny.Mag_Symbol FROM CDN.Towary AS Towary
                    LEFT JOIN CDN.TwrIlosci AS Ilosci
                    ON Towary.Twr_TwrId = Ilosci.TwI_TwrId 
                    LEFT JOIN CDN.Magazyny As Magazyny
                    ON Ilosci.TwI_MagId = Magazyny.Mag_MagId
                    LEFT JOIN CDN.TwrGrupy AS Grupy
                    On Towary.Twr_TwGGIDNumer = Grupy.TwG_GIDNumer
                    WHERE Towary.Twr_Kod LIKE {0}
                    AND Grupy.TwG_GIDTyp = -16
                    AND Towary.Twr_NieAktywny = 0
                    AND Mag_Symbol = {1}
                    )
                    SELECT * FROM Tow WHERE (TwI_Data = ((SELECT MAX(TwI_Data) FROM CDN.TwrIlosci AS IlosciData WHERE Tow.Twr_TwrId =  IlosciData.TwI_TwrId AND Mag_MagId = TwI_MagId)) OR TwI_Data IS NULL )";
                items = _optimaDbContext.OptimaWares.FromSql(query, "%" + filter + "%", magName).ToList();
            }
            else
            {
                string query = @"WITH Tow AS(SELECT Towary.Twr_TwrId,
					Towary.Twr_NieAktywny,
					Towary.Twr_Kod,
					Towary.Twr_Nazwa,
					Towary.Twr_JM,
					Towary.Twr_JmPomPrzelicznikL,
					Ilosci.TwI_Ilosc,
					TwI_Data,
					Grupy.TwG_Kod,
					Grupy.TwG_Nazwa,
					Magazyny.Mag_Nazwa,
					Magazyny.Mag_MagId,
					Magazyny.Mag_Symbol FROM CDN.Towary AS Towary
                    LEFT JOIN CDN.TwrIlosci AS Ilosci
                    ON Towary.Twr_TwrId = Ilosci.TwI_TwrId 
                    LEFT JOIN CDN.Magazyny As Magazyny
                    ON Ilosci.TwI_MagId = Magazyny.Mag_MagId
                    LEFT JOIN CDN.TwrGrupy AS Grupy
                    On Towary.Twr_TwGGIDNumer = Grupy.TwG_GIDNumer
                    WHERE Towary.Twr_Kod LIKE {0}
                    AND Grupy.TwG_GIDTyp = -16
                    AND Towary.Twr_NieAktywny = 0
                    AND Mag_Symbol IS NULL
                    )
                    SELECT * FROM Tow WHERE (TwI_Data = ((SELECT MAX(TwI_Data) FROM CDN.TwrIlosci AS IlosciData WHERE Tow.Twr_TwrId =  IlosciData.TwI_TwrId)) OR TwI_Data IS NULL )";
                items = _optimaDbContext.OptimaWares.FromSql(query, "%" + filter + "%").ToList();
            }
            if (items == null)
                return NoContent();
            
            // Sprawdz czy materiał jest na magazynie surowcow. Jeśli nie to dodaj z zerową ilością.
            List<OptimaWare> updated_items = new List<OptimaWare>();

            for (int i = 0; i < items.Count; i++)
            {
                var found = items.Find(x => x.Mag_Symbol == "1.MAGAZYN SUROWCÓW" && x.Twr_Kod == items[i].Twr_Kod);
                updated_items.Add(items[i]);

                if (found == null)
                {
                    var new_item = new OptimaWare(items[i]);
                    new_item.Mag_Symbol = "1.MAGAZYN SUROWCÓW";
                    new_item.Mag_Nazwa = "Magazyn główny SUROWCÓW";
                    new_item.TwG_Kod = "STAL ";
                    new_item.TwG_Nazwa = "magazyn stali";
                    new_item.TwI_Ilosc = 0;
                    updated_items.Add(new_item);
                }

            }
            return Ok(updated_items);
        }

        [HttpGet("getWarehouseItemQty")]
        public IActionResult GetWarehouseItemQty(string wareCode, string rwDate, string magName = "1.MAGAZYN SUROWCÓW")
        {
            decimal? wareQty;
            // Get orderNumber from header
            //string wareCode = HttpContext.Request.Headers["wareCode"];
            //string rwDate = HttpContext.Request.Headers["rwDate"];
            string query = @"SELECT TOP 1 Towary.Twr_TwrId, Towary.Twr_Kod, Towary.Twr_Nazwa, Towary.Twr_JM, 
            Towary.Twr_JmPomPrzelicznikL, Ilosci.TwI_Ilosc, TwI_Data, Towary.Twr_NieAktywny, Mag_Symbol, TwI_Rezerwacje FROM CDN.Towary AS Towary
            INNER JOIN CDN.TwrIlosci AS Ilosci
            ON Towary.Twr_TwrId = Ilosci.TwI_TwrId 
			INNER JOIN CDN.Magazyny AS Magazyny
			ON Ilosci.TwI_MagId = Magazyny.Mag_MagId
            WHERE Towary.Twr_Kod = {0}
            AND Mag_Symbol = {1}
            AND Towary.Twr_NieAktywny = 0
            AND TwI_Data = (SELECT MAX(TwI_Data) FROM CDN.TwrIlosci AS IlosciData WHERE Towary.Twr_TwrId =  IlosciData.TwI_TwrId AND TwI_Data <= {2} AND Mag_MagId = TwI_MagId)";

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var wareQtyThen = _optimaDbContext.OptimaWares.FromSql(query, wareCode, magName, rwDate).Select(x => x.TwI_Ilosc).FirstOrDefault();
            var wareQtyNow = _optimaDbContext.OptimaWares.FromSql(query, wareCode, magName, currentDate).Select(x => x.TwI_Ilosc).FirstOrDefault();

            if (wareQtyThen == null)
                wareQtyThen = 0;
            if (wareQtyNow == null)
                wareQtyNow = 0;

            if (wareQtyNow < wareQtyThen)
            {
                wareQty = wareQtyNow;
            }
            else if(wareQtyNow > wareQtyThen)
            {
                wareQty = wareQtyThen;
            }
            else if(wareQtyNow == wareQtyThen)
            {
                wareQty = wareQtyNow;
            }
            else
            {
                wareQty = 0;
            }

            return Ok(wareQty);
        }

        [HttpGet("getRWNumber")]
        public IActionResult GetRWNumberByDescFilter(string orderNumber, string subOrderNumber, string componentNumber)
        {
            string[] rwListType = new string[] { "Lista materiałowa", "Cięcie CNC (plazma)", "Cięcie CNC (kooperacja)", "Lista części handlowych" };
            List<OptimaRW> rws = new List<OptimaRW>();

            string query = @"SELECT TOP 1 TrN_TrNID, TrN_NumerPelny, TrN_Bufor, TrN_RazemNetto, TrN_OpeModNazwisko,
                             TrN_DataDok, TrN_DataOpe from CDN.TraNag
                             WHERE TrN_TypDokumentu = 304
                             AND Trn_Opis LIKE {0}";

            string filter_new;
            string filter_old;
            OptimaRW rw = null;
            foreach (string rwType in rwListType)
            {
                filter_new = orderNumber + " ; " + subOrderNumber + " ; " + componentNumber + " ; " + rwType + "%";

                rw = _optimaDbContext.OptimaRws.FromSql(query, filter_new).FirstOrDefault();

                if (rw == null)
                {
                    filter_old = orderNumber + "(" + subOrderNumber + ")-" + componentNumber + "%";

                    rw = _optimaDbContext.OptimaRws.FromSql(query, filter_old).FirstOrDefault();

                    if (rw == null)
                    {
                        rw = new OptimaRW()
                        {
                            TrN_TrNID = -1,
                            TrN_Bufor = -1,
                            TrN_DataDok = null,
                            TrN_DataOpe = null,
                            TrN_NumerPelny = "b/d",
                            TrN_OpeModNazwisko = "b/d",
                            TrN_RazemNetto = null
                        };
                    }
                }

                rws.Add(rw);
            }

            return Ok(rws);
        }


        [HttpGet("getMags")]
        public IActionResult GetMags(string extendedListStr)
        {
            string query = @"SELECT Mag_MagId, Mag_Symbol, Mag_Nazwa from CDN.Magazyny";
            bool extendedList = Boolean.Parse(extendedListStr);

            var mags = _optimaDbContext.OptimaMags.FromSql(query).ToList();

            if (extendedList)
            {
                mags.Insert(0, new OptimaMag() { Mag_MagId = -1, Mag_Nazwa = "NIE PRZYPISANY", Mag_Symbol = "NIE PRZYPISANY" });
                mags.Insert(1, new OptimaMag() { Mag_MagId = 0, Mag_Nazwa = "WSZYSTKIE", Mag_Symbol = "WSZYSTKIE" });
            }

            if (mags == null)
                return NoContent();
            return Ok(mags);
        }


        [HttpGet("searchClient")]
        public IActionResult GetClients(string filter)
        {
            string query = @"SELECT Knt_KntId, Knt_Kod, Knt_Nazwa1, Knt_Nazwa2, Knt_Nazwa3, Knt_Kraj, Knt_Wojewodztwo,
                            Knt_Powiat, Knt_Gmina, Knt_Ulica, Knt_NrDomu, Knt_NrLokalu, Knt_Miasto, Knt_KodPocztowy, Knt_Poczta, Knt_Adres2,
                            Knt_NipKraj, Knt_NipE, Knt_Nip, Knt_Regon, Knt_Pesel, Knt_Telefon1, Knt_Telefon2, Knt_Fax, Knt_Email, Knt_URL, Knt_KrajISO
                            FROM CDN.Kontrahenci
                            WHERE Knt_Kod LIKE {0} OR Knt_Nazwa1 LIKE {0}";

            var clients = _optimaDbContext.OptimaClients.FromSql(query, "%" + filter + "%").ToList();

            if (clients == null)
                return NoContent();
            return Ok(clients);
        }
        #endregion
    }
}