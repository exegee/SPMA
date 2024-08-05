using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using SPMA.Dtos.Orders;
using SPMA.Dtos.Production;
using SPMA.Models.Orders;
using SPMA.Models.Production;
using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;

namespace SPMA.Core.Tools
{
    public class PdfGenerator
    {
        public Document document;

        public PdfGenerator()
        {
            document = new Document();
            DefineStyles();
            document.DefaultPageSetup.LeftMargin = "1.0cm";
            document.DefaultPageSetup.RightMargin = "1.0cm";

        }

        public void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = this.document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Times New Roman";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            //style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            // Create a new style called Reference based on style Normal
            style = this.document.Styles.AddStyle("GreyBackground", "Normal");
            style.ParagraphFormat.Shading.Color = new Color(225, 225, 225);
        }

        public Document GenerateRW(List<InProductionRW> wares, string orderNumber
            , string bookNumber, string bookComponentNumber, string subOrderNumber, bool generateZD, sbyte listType)
        {
            Section section = document.AddSection();

            var border02 = new Border();
            border02.Width = 0.5;
            var border03 = new Border();
            border03.Width = 0.3;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            // Create the text frame for the address
            TextFrame addressFrame = section.AddTextFrame();
            addressFrame.Height = "2.3cm";
            addressFrame.Width = "9.2cm";
            var borderFormat = new LineFormat();
            borderFormat.Color = new Color(0, 0, 0);
            addressFrame.LineFormat = borderFormat;
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "1.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            // Create the text frame for the address
            TextFrame rwFrame = section.AddTextFrame();
            rwFrame.Height = "2.3cm";
            rwFrame.Width = "9.2cm";
            rwFrame.Left = ShapePosition.Right;
            rwFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            rwFrame.Top = "1.0cm";
            rwFrame.RelativeVertical = RelativeVertical.Page;

            // Put sender in address frame
            paragraph.Format.Font.Size = 5;
            paragraph.Format.SpaceAfter = 0;

            paragraph = addressFrame.AddParagraph("SPÓŁKA STOLARCZYK Tomasz Stolarczyk, Janusz Stolarczyk");
            paragraph.AddLineBreak();
            paragraph.AddText("Kochanowskiego 30");
            paragraph.AddLineBreak();
            paragraph.AddText("33-100 Tarnów");
            paragraph.AddLineBreak();
            paragraph.AddText("NIP: 873-020-41-85");
            paragraph.Format.Font.Bold = true;
            paragraph.Format.LeftIndent = "0.1cm";
            paragraph.Format.SpaceBefore = "0.1cm";


            paragraph = rwFrame.AddParagraph("Rozchód wewnętrzny RW");
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Style = "GreyBackground";
            paragraph.AddLineBreak();

            paragraph = rwFrame.AddParagraph("");
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = rwFrame.AddParagraph("Data wystawienia: ");
            paragraph.AddFormattedText(DateTime.Today.ToString("dd.MM.yyyy"), TextFormat.Bold);
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "2.5cm";
            paragraph.Style = "Reference";
            paragraph.AddText("Zlecenie: ");
            paragraph.AddFormattedText(orderNumber, TextFormat.Bold);
            paragraph.Format.SpaceAfter = "0.1cm";

            Paragraph paragraph3 = section.AddParagraph();
            paragraph3.AddText("Numer podzlecenia: ");
            paragraph3.AddFormattedText(subOrderNumber, TextFormat.Bold);
            paragraph3.Format.SpaceAfter = "0.1cm";

            Paragraph paragraph2 = section.AddParagraph();
            paragraph2.AddText("Numer książki: ");
            paragraph2.AddFormattedText(bookNumber, TextFormat.Bold);
            paragraph2.AddFormattedText(" (", TextFormat.Bold);
            paragraph2.AddFormattedText(bookComponentNumber, TextFormat.Bold);
            paragraph2.AddFormattedText(")", TextFormat.Bold);
            paragraph2.Format.SpaceAfter = "2cm";

            Table table = section.AddTable();
            //table.Style = "Table";
            //table.Borders.Width = 0.15;
            table.Rows.LeftIndent = 0;
            table.Rows.VerticalAlignment = VerticalAlignment.Center;
            table.Rows.Height = "0.5cm";

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("4.0cm");
            column.Format.Alignment = ParagraphAlignment.Left;


            column = table.AddColumn("5.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            // Image
            column = table.AddColumn("0.4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Quantity
            column = table.AddColumn("1.7cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Length
            column = table.AddColumn("2.0cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Sum
            column = table.AddColumn("1.4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Unit
            column = table.AddColumn("1.0cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Wydano
            column = table.AddColumn("2.0cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            Row row = table.AddRow();
            row.Cells[0].AddParagraph("Lp.");
            row.Format.Font.Size = 8;
            row.Format.Font.Bold = true;
            row.Borders.Top = border02;
            row.Borders.Bottom = border02.Clone();
            row.Cells[0].Borders.Left = border02.Clone();

            row.Cells[1].AddParagraph("Numer rysunku");

            row.Cells[2].AddParagraph("Kod towaru");

            row.Cells[3].AddParagraph("");

            row.Cells[4].AddParagraph("Liczba szt.");

            row.Cells[5].AddParagraph("Długość 1 szt.");

            row.Cells[6].AddParagraph("Łącznie");

            row.Cells[7].AddParagraph("J.m.");

            row.Cells[8].AddParagraph("Wydano");
            row.Cells[8].Borders.Right = border02.Clone();


            var lp = 1;

            //var sum = wares.SelectMany(x => x.ComponentWareDtos).GroupBy(u => new { u.WareCode, u.Length });

            //if (generateZD)
            //{
            //    Section sectionZD = document.AddSection();

            //    // Create footer
            //    Paragraph paragraphZD = sectionZD.Footers.Primary.AddParagraph();
            //    //paragraph.AddText("Spółka Stolarczyk ·  · 56789 Cologne · Germany");
            //    //paragraph.Format.Font.Size = 9;
            //    //paragraph.Format.Alignment = ParagraphAlignment.Center;

            //    // Create the text frame for the address
            //    TextFrame addressFrameZD = sectionZD.AddTextFrame();
            //    addressFrameZD.Height = "2.3cm";
            //    addressFrameZD.Width = "9.2cm";
            //    addressFrameZD.LineFormat = borderFormat.Clone();
            //    addressFrameZD.Left = ShapePosition.Left;
            //    addressFrameZD.RelativeHorizontal = RelativeHorizontal.Margin;
            //    addressFrameZD.Top = "1.0cm";
            //    addressFrameZD.RelativeVertical = RelativeVertical.Page;



            //    // Create the text frame for the address
            //    TextFrame zdFrame = sectionZD.AddTextFrame();
            //    zdFrame.Height = "2.3cm";
            //    zdFrame.Width = "9.2cm";
            //    zdFrame.Left = ShapePosition.Right;
            //    zdFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            //    zdFrame.Top = "1.0cm";
            //    zdFrame.RelativeVertical = RelativeVertical.Page;

            //    // Put sender in address frame
            //    //paragraph.Format.Font.Name = "Verdana";
            //    paragraphZD.Format.Font.Size = 5;
            //    paragraphZD.Format.SpaceAfter = 0;

            //    paragraphZD = addressFrameZD.AddParagraph("SPÓŁKA STOLARCZYK Tomasz Stolarczyk, Janusz Stolarczyk");
            //    paragraphZD.AddLineBreak();
            //    paragraphZD.AddText("Kochanowskiego 30");
            //    paragraphZD.AddLineBreak();
            //    paragraphZD.AddText("33-100 Tarnów");
            //    paragraphZD.AddLineBreak();
            //    paragraphZD.AddText("NIP: 873-020-41-85");
            //    paragraphZD.Format.Font.Bold = true;
            //    paragraphZD.Format.LeftIndent = "0.1cm";
            //    paragraphZD.Format.SpaceBefore = "0.1cm";


            //    paragraphZD = zdFrame.AddParagraph("Zamówienie u dostawcy ZD");
            //    paragraphZD.Format.Font.Size = 12;
            //    paragraphZD.Format.Font.Bold = true;
            //    paragraphZD.Format.Alignment = ParagraphAlignment.Center;
            //    paragraphZD.Style = "GreyBackground";
            //    paragraphZD.AddLineBreak();

            //    paragraphZD = zdFrame.AddParagraph("");
            //    paragraphZD.Format.Font.Size = 10;
            //    paragraphZD.Format.Font.Bold = true;
            //    paragraphZD.Format.Alignment = ParagraphAlignment.Center;
            //    paragraphZD.AddLineBreak();
            //    paragraphZD.AddLineBreak();
            //    paragraphZD = zdFrame.AddParagraph("Data wystawienia: ");
            //    paragraphZD.AddFormattedText(DateTime.Today.ToString("dd.MM.yyyy"), TextFormat.Bold);
            //    paragraphZD.Format.Font.Size = 10;
            //    paragraphZD.Format.Alignment = ParagraphAlignment.Right;

            //    // Add the print date field
            //    paragraphZD = sectionZD.AddParagraph();
            //    paragraphZD.Format.SpaceBefore = "2.5cm";
            //    paragraphZD.Style = "Reference";
            //    paragraphZD.AddText("Zlecenie: ");
            //    paragraphZD.AddFormattedText(orderNumber, TextFormat.Bold);
            //    paragraphZD.Format.SpaceAfter = "0.1cm";

            //    Paragraph paragraphZD3 = sectionZD.AddParagraph();
            //    paragraphZD3.AddText("Numer podzlecenia: ");
            //    paragraphZD3.AddFormattedText(subOrderNumber, TextFormat.Bold);
            //    paragraphZD3.Format.SpaceAfter = "0.1cm";

            //    Paragraph paragraphZD2 = sectionZD.AddParagraph();
            //    paragraphZD2.AddText("Numer książki: ");
            //    paragraphZD2.AddFormattedText(bookNumber, TextFormat.Bold);
            //    paragraphZD2.AddFormattedText(" (", TextFormat.Bold);
            //    paragraphZD2.AddFormattedText(bookComponentNumber, TextFormat.Bold);
            //    paragraphZD2.AddFormattedText(")", TextFormat.Bold);
            //    paragraphZD2.Format.SpaceAfter = "2cm";

            //    Table tableZD = sectionZD.AddTable();
            //    //table.Style = "Table";
            //    //table.Borders.Width = 0.15;
            //    tableZD.Rows.LeftIndent = 0;
            //    tableZD.Rows.VerticalAlignment = VerticalAlignment.Center;
            //    tableZD.Rows.Height = "0.5cm";

            //    // Before you can add a row, you must define the columns
            //    Column columnZD = tableZD.AddColumn("1cm");
            //    columnZD.Format.Alignment = ParagraphAlignment.Center;

            //    // Ware Code
            //    columnZD = tableZD.AddColumn("7.5cm");
            //    columnZD.Format.Alignment = ParagraphAlignment.Left;

            //    // Quantity
            //    columnZD = tableZD.AddColumn("2.0cm");
            //    columnZD.Format.Alignment = ParagraphAlignment.Center;

            //    // Length
            //    columnZD = tableZD.AddColumn("2.0cm");
            //    columnZD.Format.Alignment = ParagraphAlignment.Center;

            //    // Missing quantity
            //    columnZD = tableZD.AddColumn("1.0cm");
            //    columnZD.Format.Alignment = ParagraphAlignment.Center;

            //    // Unit
            //    columnZD = tableZD.AddColumn("3.0cm");
            //    columnZD.Format.Alignment = ParagraphAlignment.Center;

            //    // Supplier
            //    columnZD = tableZD.AddColumn("2.5cm");
            //    columnZD.Format.Alignment = ParagraphAlignment.Center;


            //    Row rowZD = tableZD.AddRow();
            //    rowZD.Cells[0].AddParagraph("Lp.");
            //    rowZD.Format.Font.Size = 8;
            //    rowZD.Format.Font.Bold = true;
            //    rowZD.Borders.Top = border02.Clone();
            //    rowZD.Borders.Bottom = border02.Clone();
            //    rowZD.Cells[0].Borders.Left = border02.Clone();

            //    rowZD.Cells[1].AddParagraph("Kod towaru");

            //    rowZD.Cells[2].AddParagraph("Liczba szt.");

            //    rowZD.Cells[3].AddParagraph("Długość 1 szt.");

            //    rowZD.Cells[4].AddParagraph("J.m.");

            //    rowZD.Cells[5].AddParagraph("Brakująca ilość");

            //    rowZD.Cells[6].AddParagraph("Dostawca");
            //    rowZD.Cells[6].Borders.Right = border02.Clone();

            //    // Group by wareCode and length
            //    var groupedMissingWares = wares.SelectMany(c => c.ComponentWareDtos)
            //        .Where(y => y.QtyWhDiff < 0)
            //        .GroupBy(x => new { x.WareCode, x.Length, x.Quantity, x.QtyWhDiff })
            //        .Select(f => new ComponentWareDto
            //        {
            //            Quantity = f.Key.Quantity,
            //            WareCode = f.Key.WareCode,
            //            Unit = f.Select(v => v.Unit).FirstOrDefault(),
            //            Length = f.Key.Length,
            //            TotalQuantity = f.Sum(i => i.Quantity),
            //            QtyWhDiff = f.Key.QtyWhDiff
            //        });

            //    // Add rows to pdf ( ZD )
            //    foreach (ComponentWareDto ware in groupedMissingWares)
            //    {
            //        Row itemRow = tableZD.AddRow();
            //        itemRow.Borders.Bottom = border03.Clone();
            //        itemRow.TopPadding = "0.0cm";
            //        itemRow.Format.Font.Size = 8;
            //        itemRow.Cells[0].AddParagraph(lp.ToString());

            //        itemRow.Cells[1].AddParagraph(ware.WareCode);
            //        string quantity = String.Concat(ware.Quantity.ToString(), " (",
            //            ware.TotalQuantity.ToString(), ")");
            //        itemRow.Cells[2].AddParagraph(quantity);
            //        itemRow.Cells[2].Format.Font.Bold = true;

            //        itemRow.Cells[3].AddParagraph(ware.Length.ToString().Replace(".", ","));

            //        itemRow.Cells[4].AddParagraph(ware.Unit);
            //        itemRow.Cells[5].AddParagraph((ware.QtyWhDiff * -1).ToString());

            //        lp++;

            //    }
            //}
            lp = 1;
            // Add rows to pdf ( RW )
            if (listType == 0)
            {
                foreach (InProductionRW item in wares)
                {
                    Row itemRow = table.AddRow();
                    itemRow.Borders.Bottom = border03.Clone();
                    //var currentNumber = item.ComponentWare.Component.Number;
                    itemRow.TopPadding = "0.0cm";
                    itemRow.Format.Font.Size = 8;

                    string number = item.InProduction.Component.Number;
                    Ware ware = item.InProduction.Ware;
                    string wareCode;
                    if (ware == null)
                    {
                        wareCode = string.Empty;
                    }
                    else
                    {
                        wareCode = item.InProduction.Ware.Code;
                    }

                    string toIssue = item.ToIssue.ToString();
                    string wareLength = item.InProduction.WareLength.ToString();
                    string totalToIssue = (item.InProduction.WareLength * item.ToIssue).ToString();
                    string wareUnit = item.InProduction.WareUnit == null ? string.Empty : item.InProduction.WareUnit;
                    //int level = item.InProduction.Level;

                    itemRow.Cells[0].AddParagraph(lp.ToString());
                    //itemRow.Cells[1].Format.LeftIndent = level * 8;
                    itemRow.Cells[1].AddParagraph(number);
                    itemRow.Cells[2].AddParagraph(wareCode);
                    itemRow.Cells[4].AddParagraph(toIssue);
                    itemRow.Cells[5].AddParagraph(wareLength);
                    itemRow.Cells[6].AddParagraph(totalToIssue);
                    itemRow.Cells[7].AddParagraph(wareUnit);



                    var i = 0;
                    //foreach (ComponentWareDto ware in item.ComponentWareDtos)
                    //{
                    //if (i > 0)
                    //{
                    //    itemRow.Cells[0].AddParagraph("");
                    //    itemRow.Cells[1].AddParagraph("");
                    //}
                    //itemRow.Cells[2].AddParagraph(item.ComponentWare.Ware.Code);
                    //string quantity = String.Concat(item.Issued.ToString(), " (",
                    //    item.TotalToIssue.ToString(), ")");
                    //itemRow.Cells[4].AddParagraph(quantity);
                    //itemRow.Cells[4].Format.Font.Bold = true;
                    //if ()
                    //{
                    //    var image = itemRow.Cells[3].AddImage("Images\\Icons\\Pdf\\exclamation.png");
                    //    image.ScaleHeight = 0.07;
                    //}

                    //itemRow.Cells[5].AddParagraph(item.ComponentWare.Length.ToString().Replace(".", ","));
                    //itemRow.Cells[6].AddParagraph((item.ComponentWare.Quantity * item.ComponentWare.Length).ToString().Replace(".", ","));
                    //itemRow.Cells[7].AddParagraph(item.ComponentWare.Unit);

                    //i++;
                    //}


                    lp++;
                }
            }




            return document;
        }

        public Document GenerateBomList(List<BookComponentDto> bomComponentsDto, string orderNumber
    , string bookNumber, string bookName, string bookComponentNumber, string subOrderNumber, bool generateZD, sbyte listType)
        {
            Section section = document.AddSection();

            var border02 = new Border();
            border02.Width = 0.5;
            var border03 = new Border();
            border03.Width = 0.3;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            // Create the text frame for the address
            TextFrame rwFrame = section.AddTextFrame();
            rwFrame.Height = "2.3cm";
            rwFrame.Width = "19cm";
            rwFrame.Left = ShapePosition.Right;
            rwFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            rwFrame.Top = "1.0cm";
            rwFrame.RelativeVertical = RelativeVertical.Page;

            paragraph = rwFrame.AddParagraph("LISTA CZĘŚCI");
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Style = "GreyBackground";
            paragraph.AddLineBreak();

            paragraph = rwFrame.AddParagraph("");
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = rwFrame.AddParagraph("Data wystawienia: ");
            paragraph.AddFormattedText(DateTime.Today.ToString("dd.MM.yyyy"), TextFormat.Bold);
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "-0.2cm";
            paragraph.Style = "Reference";
            paragraph.AddText("Zlecenie: ");
            paragraph.AddFormattedText(orderNumber, TextFormat.Bold);
            paragraph.Format.SpaceAfter = "0.1cm";

            Paragraph paragraph3 = section.AddParagraph();
            paragraph3.AddText("Podzlecenie: ");
            paragraph3.AddFormattedText(subOrderNumber, TextFormat.Bold);
            paragraph3.Format.SpaceAfter = "0.1cm";

            Paragraph paragraph2 = section.AddParagraph();
            paragraph2.AddText("Książka: ");
            paragraph2.AddFormattedText(bookNumber, TextFormat.Bold);
            paragraph2.AddFormattedText(" (", TextFormat.Bold);
            paragraph2.AddFormattedText(bookComponentNumber, TextFormat.Bold);
            paragraph2.AddFormattedText(")", TextFormat.Bold);
            paragraph2.AddFormattedText(" - ", TextFormat.Bold);
            paragraph2.AddFormattedText(bookName, TextFormat.Bold);
            paragraph2.Format.SpaceAfter = "2cm";

            Table table = section.AddTable();
            //table.Style = "Table";
            //table.Borders.Width = 0.15;
            table.Rows.LeftIndent = 0;
            table.Rows.VerticalAlignment = VerticalAlignment.Center;
            table.Rows.Height = "0.5cm";

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("5.0cm");
            column.Format.Alignment = ParagraphAlignment.Left;


            column = table.AddColumn("6.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            // Image
            column = table.AddColumn("1.7cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Quantity
            column = table.AddColumn("4.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            Row row = table.AddRow();
            row.Cells[0].AddParagraph("Lp.");
            row.Format.Font.Size = 8;
            row.Format.Font.Bold = true;
            row.Borders.Top = border02;
            row.Borders.Bottom = border02.Clone();
            row.Borders.Right = border02.Clone();
            row.Borders.Left = border02.Clone();
            row.Cells[0].Borders.Left = border02.Clone();

            row.Cells[1].AddParagraph("Numer rysunku");

            row.Cells[2].AddParagraph("Nazwa");

            row.Cells[3].AddParagraph("L. szt.");

            row.Cells[4].AddParagraph("Uwagi");

            var lp = 1;

            foreach (BookComponentDto bookComponentDto in bomComponentsDto)
            {
                Row itemRow = table.AddRow();
                itemRow.Borders.Bottom = border03.Clone();
                itemRow.Borders.Right = border03.Clone();
                itemRow.Borders.Left = border03.Clone();
                //var currentNumber = item.ComponentWare.Component.Number;
                itemRow.TopPadding = "0.0cm";
                itemRow.Format.Font.Size = 8;

                string number = bookComponentDto.Component.Number;
                string name = bookComponentDto.Component.Name;
                int? quantity = bookComponentDto.Quantity;
                string descrtiption = bookComponentDto.Component.Description;
                int level = bookComponentDto.Level;

                itemRow.Cells[0].AddParagraph(lp.ToString());
                itemRow.Cells[1].Format.LeftIndent = level * 8;
                itemRow.Cells[1].AddParagraph(number);
                itemRow.Cells[2].AddParagraph(name);
                itemRow.Cells[3].AddParagraph(quantity.ToString());
                itemRow.Cells[4].AddParagraph(descrtiption == null ? "" : descrtiption);


                lp++;
            }
            return document;
        }

        public Document GenerateWareList(List<InProductionRWDto> InProductionRWDtos, Order order, OrderBook subOrder
    , string bookNumber, string bookName, string bookComponentNumber, string subOrderNumber, bool generateZD, sbyte listType)
        {
            Section section = document.AddSection();

            var border02 = new Border();
            border02.Width = 0.5;
            var border03 = new Border();
            border03.Width = 0.3;
            var noBorder = new Border();
            noBorder.Width = 0.0;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            // Create the text frame for the address
            TextFrame addressFrame = section.AddTextFrame();
            addressFrame.Height = "3.1cm";
            addressFrame.Width = "9.2cm";
            var borderFormat = new LineFormat();
            borderFormat.Color = new Color(0, 0, 0);
            addressFrame.LineFormat = borderFormat;
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "1.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;
            addressFrame.Margin = "0.1cm";

            // Create the text frame for the address
            TextFrame rwFrame = section.AddTextFrame();
            rwFrame.Height = "2.3cm";
            rwFrame.Width = "9.2cm";
            rwFrame.Left = ShapePosition.Right;
            rwFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            rwFrame.Top = "1.0cm";
            rwFrame.RelativeVertical = RelativeVertical.Page;

            if (listType == 0)
            {
                paragraph = rwFrame.AddParagraph("LISTA MATERIAŁOWA");
            }
            else if(listType == 1)
            {
                paragraph = rwFrame.AddParagraph("ELEMENTY HANDLOWE");
            }
            else if (listType == 2)
            {
                paragraph = rwFrame.AddParagraph("ELEMENTY CIĘTE CNC PLAZMA");
            }
            else if (listType == 3)
            {
                paragraph = rwFrame.AddParagraph("ELEMENTY CIĘTE CNC KOOPERACJA");
            }

            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Style = "GreyBackground";
            paragraph.AddLineBreak();

            paragraph = addressFrame.AddParagraph("Zlecenie:  ");
            paragraph.AddFormattedText(order.Number, TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Ilość: ");
            paragraph.AddFormattedText(order.PlannedQty.ToString(), TextFormat.Bold);
            paragraph.Format.SpaceAfter = "0.15cm";
            paragraph = addressFrame.AddParagraph("Podzlecenie:  ");
            paragraph.AddFormattedText(subOrderNumber, TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Ilość: ");
            paragraph.AddFormattedText(subOrder.PlannedQty.ToString(), TextFormat.Bold);
            paragraph.Format.SpaceAfter = "0.15cm";
            paragraph = addressFrame.AddParagraph("Książka:  ");
            paragraph.AddFormattedText(bookNumber, TextFormat.Bold);
            paragraph.AddFormattedText(" (", TextFormat.Bold);
            paragraph.AddFormattedText(bookComponentNumber, TextFormat.Bold);
            paragraph.AddFormattedText(")", TextFormat.Bold);
            
            paragraph.Format.SpaceAfter = "0.15cm";
            paragraph = addressFrame.AddParagraph(bookName);
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            paragraph = rwFrame.AddParagraph();

            paragraph.AddFormattedText(order.Number, TextFormat.Bold);
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Size = 18;
            paragraph.Format.SpaceBefore = "0.1cm";
            paragraph.Format.SpaceAfter = "0.1cm";
            paragraph = rwFrame.AddParagraph();
            paragraph.AddFormattedText(order.Name, TextFormat.Bold);
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Size = 12;


            paragraph.AddLineBreak();
            paragraph = rwFrame.AddParagraph("Data wystawienia: ");
            paragraph.Format.SpaceBefore = "1cm";
            paragraph.AddFormattedText(DateTime.Today.ToString("dd.MM.yyyy"), TextFormat.Bold);
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            Paragraph paragraph2 = section.AddParagraph();
            paragraph2.Format.SpaceAfter = "2cm";
            Table table = section.AddTable();
            //table.Style = "Table";
            //table.Borders.Width = 0.15;
            table.Rows.LeftIndent = 0;
            table.Rows.VerticalAlignment = VerticalAlignment.Center;
            table.Rows.Height = "0.5cm";

            // Before you can add a row, you must define the columns
            // 0 - # number
            Column column = table.AddColumn("0.75cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // ListType
            // 0 - Saw
            // 1 - Purchase
            // 2 - BOM
            if (listType == 0)
            {
                // 1 - Drawing number
                column = table.AddColumn("3.5cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 2 - Ware code
                column = table.AddColumn("5cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 3 - Ware replacement (blank)
                column = table.AddColumn("2.5cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 4 - Quantities
                column = table.AddColumn("1.25cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                //// 5 - Quantities
                //column = table.AddColumn("1cm");
                //column.Format.Alignment = ParagraphAlignment.Center;

                // 6 - 1 piece length
                column = table.AddColumn("1.75cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 7 - Sum quantity
                column = table.AddColumn("1.75cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 8 - Ware unit
                column = table.AddColumn("1.0cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 9 - Issued info
                column = table.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                Row row = table.AddRow();
                row.Cells[0].AddParagraph("Lp.");
                row.Format.Font.Size = 8;
                row.Format.Font.Bold = true;
                row.Borders.Top = border02;
                row.Borders.Bottom = border02.Clone();
                row.Borders.Left = border02.Clone();
                row.Borders.Right = border02.Clone();

                row.Cells[0].Borders.Left = border02.Clone();

                row.Cells[1].AddParagraph("Numer rysunku");

                row.Cells[2].AddParagraph("Kod towaru");

                row.Cells[3].AddParagraph("Zamiennik");
                row.Cells[3].Format.Alignment = ParagraphAlignment.Center;

                row.Cells[4].AddParagraph("Szt.");
                //row.Cells[4].MergeRight = 1;
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;

                row.Cells[5].AddParagraph("Dł. 1 szt.");

                row.Cells[6].AddParagraph("Łącznie");

                row.Cells[7].AddParagraph("J.m.");

                row.Cells[8].AddParagraph("Wydano");


                var lp = 1;

                foreach (InProductionRWDto inProductionRWDto in InProductionRWDtos)
                {
                    Row itemRow = table.AddRow();
                    //itemRow.Borders.Bottom = border03.Clone();
                    itemRow.Borders.Left = border03.Clone();
                    itemRow.Borders.Right = border03.Clone();
                    itemRow.TopPadding = "0.0cm";
                    itemRow.Format.Font.Size = 8;

                    string componentName = inProductionRWDto.InProduction.Component.Name;
                    string componentNumber = inProductionRWDto.InProduction.Component.Number;
                    string wareName = inProductionRWDto.Ware.Name;
                    string wareCode = inProductionRWDto.Ware.Code;
                    int? qtyTotal = inProductionRWDto.ToIssue;
                    //int? qtyPerBook = inProductionRWDto.ToIssuePerBook;
                    //int? qtyPerSuborder = inProductionRWDto.ToIssuePerSubOrder;
                    decimal? wareLength = inProductionRWDto.WareLength;
                    decimal? wareSum = inProductionRWDto.ToIssue * inProductionRWDto.WareLength;
                    string wareUnit = inProductionRWDto.WareUnit;

                    itemRow.Cells[0].AddParagraph(lp.ToString());
                    itemRow.Cells[0].MergeDown = 1;

                    itemRow.Cells[1].AddParagraph(componentNumber);
                    //itemRow.Cells[1].MergeDown = 1;

                    itemRow.Cells[2].AddParagraph(wareCode);
                    //itemRow.Cells[2].MergeDown = 1;

                    itemRow.Cells[3].MergeDown = 1;

                    // If quantity difference beetween optima and required is below 0 display exclamation sign
                    if (inProductionRWDto.QtyWhDiff < 0)
                    {
                        //var image = itemRow.Cells[3].AddImage("Images\\Icons\\Pdf\\exclamation.png");
                        //image.ScaleHeight = 0.07;
                        itemRow.Cells[3].AddParagraph("!!!");
                    }

                    itemRow.Cells[4].AddParagraph(qtyTotal.ToString());
                    itemRow.Cells[4].Format.Font.Bold = true;
                    itemRow.Cells[4].MergeDown = 1;
                    //itemRow.Cells[4].AddParagraph(qtyPerBook.ToString());

                    //itemRow.Cells[5].AddParagraph(qtyPerSuborder.ToString());

                    itemRow.Cells[5].AddParagraph(wareLength.ToString());
                    itemRow.Cells[5].MergeDown = 1;

                    itemRow.Cells[6].AddParagraph(wareSum.ToString());
                    itemRow.Cells[6].MergeDown = 1;

                    itemRow.Cells[7].AddParagraph(wareUnit);
                    itemRow.Cells[7].MergeDown = 1;

                    itemRow.Cells[8].MergeDown = 1;

                    itemRow = table.AddRow();
                    itemRow.Borders.Bottom = border03.Clone();
                    itemRow.Borders.Right = border03.Clone();
                    //itemRow.TopPadding = "0.0cm";
                    itemRow.Format.Font.Size = 6;
                    itemRow.Cells[1].AddParagraph(componentName);
                    itemRow.Cells[2].AddParagraph(wareName);
                    //itemRow.Cells[4].MergeRight = 1;


                    lp++;
                }
            }
            else if (listType == 1)
            {
                // 1 - Ware code
                column = table.AddColumn("6cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 2 - Comment (blank)
                column = table.AddColumn("3cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 3 - Ware replacement (blank)
                column = table.AddColumn("5.75cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 4 - Quantities
                column = table.AddColumn("1cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 5 - Ware unit
                column = table.AddColumn("1cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 6 - Issued info
                column = table.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                Row row = table.AddRow();
                row.Cells[0].AddParagraph("Lp.");
                row.Format.Font.Size = 8;
                row.Format.Font.Bold = true;
                row.Borders.Top = border02;
                row.Borders.Bottom = border02.Clone();
                row.Borders.Left = border02.Clone();
                row.Borders.Right = border02.Clone();

                row.Cells[0].Borders.Left = border02.Clone();

                row.Cells[1].AddParagraph("Kod towaru");

                row.Cells[2].AddParagraph("Uwagi");

                row.Cells[3].AddParagraph("Zamiennik");

                row.Cells[4].AddParagraph("Szt.");
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;

                row.Cells[5].AddParagraph("J.m.");

                row.Cells[6].AddParagraph("Wydano");


                var lp = 1;

                foreach (InProductionRWDto inProductionRWDto in InProductionRWDtos)
                {
                    Row itemRow = table.AddRow();
                    itemRow.Borders.Bottom = border03.Clone();
                    itemRow.Borders.Left = border03.Clone();
                    itemRow.Borders.Right = border03.Clone();
                    itemRow.TopPadding = "0.0cm";
                    itemRow.Format.Font.Size = 8;

                    string wareName = inProductionRWDto.Ware.Name;
                    string comment = inProductionRWDto.InProduction.Component.Comment;
                    string wareCode = inProductionRWDto.Ware.Code;
                    int? qtyTotal =  inProductionRWDto.ToIssue;
                    string wareUnit = inProductionRWDto.WareUnit;

                    itemRow.Cells[0].AddParagraph(lp.ToString());
                    itemRow.Cells[0].MergeDown = 1;

                    itemRow.Cells[1].AddParagraph(wareCode);
                    itemRow.Cells[1].Borders.Bottom = noBorder.Clone();
                    //itemRow.Cells[1].MergeDown = 1;

                    itemRow.Cells[2].AddParagraph(comment);
                    itemRow.Cells[2].MergeDown = 1;

                    itemRow.Cells[3].MergeDown = 1;

                    // If quantity difference beetween optima and required is below 0 display exclamation sign
                    if (inProductionRWDto.QtyWhDiff < 0)
                    {
                        //var image = itemRow.Cells[3].AddImage("Images\\Icons\\Pdf\\exclamation.png");
                        //image.ScaleHeight = 0.07;
                        itemRow.Cells[3].AddParagraph("!!!");
                    }

                    itemRow.Cells[4].AddParagraph(qtyTotal.ToString());
                    itemRow.Cells[4].MergeDown = 1;

                    itemRow.Cells[5].AddParagraph(wareUnit);
                    itemRow.Cells[5].MergeDown = 1;

                    itemRow.Cells[6].MergeDown = 1;

                    itemRow = table.AddRow();
                    itemRow.Borders.Bottom = border03.Clone();
                    //////itemRow.Borders.Top = noBorder.Clone();
                    //itemRow.Borders.Right = border03.Clone();
                    //itemRow.TopPadding = "0.0cm";
                    itemRow.Format.Font.Size = 6;
                    itemRow.Cells[1].AddParagraph(wareName);

                    lp++;
                }
            }
            else if (listType == 2 || listType == 3)
            {
                // 1 - Drawing number
                column = table.AddColumn("3.5cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 2 - Ware code
                column = table.AddColumn("5cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 3 - Ware replacement (blank)
                column = table.AddColumn("2.5cm");
                column.Format.Alignment = ParagraphAlignment.Left;

                // 4 - Quantities
                column = table.AddColumn("1cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 5 - 1 piece weight
                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 6 - total weight
                column = table.AddColumn("2cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 7 - Ware unit
                column = table.AddColumn("0.75cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                // 8 - Issued info
                column = table.AddColumn("1.5cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                Row row = table.AddRow();
                row.Cells[0].AddParagraph("Lp.");
                row.Format.Font.Size = 8;
                row.Format.Font.Bold = true;
                row.Borders.Top = border02;
                row.Borders.Bottom = border02.Clone();
                row.Borders.Left = border02.Clone();
                row.Borders.Right = border02.Clone();

                row.Cells[0].Borders.Left = border02.Clone();

                row.Cells[1].AddParagraph("Numer rysunku");

                row.Cells[2].AddParagraph("Kod towaru");

                row.Cells[3].AddParagraph("Zamiennik");

                row.Cells[4].AddParagraph("Szt.");
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;

                row.Cells[5].AddParagraph("Waga 1 szt.");

                row.Cells[6].AddParagraph("Waga łączna");

                row.Cells[7].AddParagraph("J.m.");

                row.Cells[8].AddParagraph("Wydano");


                var lp = 1;

                foreach (InProductionRWDto inProductionRWDto in InProductionRWDtos)
                {
                    Row itemRow = table.AddRow();
                    itemRow.Borders.Bottom = border03.Clone();
                    itemRow.Borders.Left = border03.Clone();
                    itemRow.Borders.Right = border03.Clone();
                    itemRow.TopPadding = "0.0cm";
                    itemRow.Format.Font.Size = 8;

                    string componentNumber = inProductionRWDto.InProduction.Component.Number;
                    string wareCode = "";
                    string wareName = "";
                    if (inProductionRWDto.Ware != null)
                    {
                        wareCode = inProductionRWDto.Ware.Code;
                        wareName = inProductionRWDto.Ware.Name;

                    }
                    string componentName = inProductionRWDto.InProduction.Component.Name;
                    int? qtyTotal = inProductionRWDto.ToIssue;
                    string wareUnit = inProductionRWDto.WareUnit == null ? "" : inProductionRWDto.WareUnit;
                    decimal? wareLength = inProductionRWDto.WareLength == null ? 0 : inProductionRWDto.WareLength;

                    itemRow.Cells[0].AddParagraph(lp.ToString());
                    itemRow.Cells[0].MergeDown = 1;
                    itemRow.Cells[1].AddParagraph(componentNumber);
                    itemRow.Cells[1].Borders.Bottom = noBorder.Clone();


                    itemRow.Cells[2].AddParagraph(wareCode);
                    itemRow.Cells[2].Borders.Bottom = noBorder.Clone();

                    //itemRow.Cells[1].MergeDown = 1;

                    //itemRow.Cells[2].MergeDown = 1;

                    itemRow.Cells[3].MergeDown = 1;
                    // If quantity difference beetween optima and required is below 0 display exclamation sign
                    if (inProductionRWDto.QtyWhDiff < 0)
                    {
                        //var image = itemRow.Cells[2].AddImage("Images\\Icons\\Pdf\\exclamation.png");
                        //image.ScaleHeight = 0.07;
                        itemRow.Cells[3].AddParagraph("!!!");
                    }

                    itemRow.Cells[4].AddParagraph(qtyTotal.ToString());
                    itemRow.Cells[4].MergeDown = 1;

                    itemRow.Cells[5].MergeDown = 1;

                    if (inProductionRWDto.Ware != null)
                    {
                        var weight = wareLength;
                        itemRow.Cells[5].AddParagraph(weight.ToString());
                        
                        itemRow.Cells[6].AddParagraph((weight * qtyTotal).ToString());
                    }
                    itemRow.Cells[6].MergeDown = 1;
                    itemRow.Cells[7].AddParagraph(wareUnit);
                    itemRow.Cells[7].MergeDown = 1;

                    itemRow.Cells[8].MergeDown = 1;

                    itemRow = table.AddRow();
                    itemRow.Borders.Bottom = border03.Clone();
                    itemRow.Borders.Right = border03.Clone();
                    itemRow.Format.Font.Size = 6;
                    itemRow.Cells[1].AddParagraph(componentName);
                    itemRow.Cells[2].AddParagraph(wareName);

                    lp++;
                }
            }

            return document;
        }

        public Document GenerateOrdersList(List<OrderDto> orders)
        {
            Section section = document.AddSection();

            var border02 = new Border();
            border02.Width = 0.5;
            var border03 = new Border();
            border03.Width = 0.3;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            // Create the text frame for the address
            TextFrame rwFrame = section.AddTextFrame();
            rwFrame.Height = "2.3cm";
            rwFrame.Width = "19cm";
            rwFrame.Left = ShapePosition.Right;
            rwFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            rwFrame.Top = "1.0cm";
            rwFrame.RelativeVertical = RelativeVertical.Page;

            paragraph = rwFrame.AddParagraph("LISTA ZLECEŃ");
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Style = "GreyBackground";
            //paragraph.AddLineBreak();

            paragraph = rwFrame.AddParagraph("");
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph = rwFrame.AddParagraph("Data wystawienia: ");
            paragraph.AddFormattedText(DateTime.Today.ToString("dd.MM.yyyy"), TextFormat.Bold);
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            Table table = section.AddTable();
            //table.Style = "Table";
            //table.Borders.Width = 0.15;
            table.Rows.LeftIndent = 0;
            table.Rows.VerticalAlignment = VerticalAlignment.Center;
            table.Rows.Height = "0.5cm";

            // Before you can add a row, you must define the columns
            // LP
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Status
            column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Quantity
            column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Number
            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // RW State
            column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Name
            column = table.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Client
            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Date added
            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Finished date
            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            row.Cells[0].AddParagraph("Lp.");
            row.Format.Font.Size = 8;
            row.Format.Font.Bold = true;
            row.Borders.Top = border02;
            row.Borders.Bottom = border02.Clone();
            row.Borders.Right = border02.Clone();
            row.Borders.Left = border02.Clone();

            row.Cells[0].Borders.Left = border02.Clone();
            row.Cells[1].AddParagraph("Status");
            row.Cells[2].AddParagraph("L. szt.");
            row.Cells[3].AddParagraph("Numer");
            row.Cells[4].AddParagraph("Stan RW");
            row.Cells[5].AddParagraph("Nazwa");
            row.Cells[6].AddParagraph("Klient");
            row.Cells[7].AddParagraph("Data wpłynięcia");
            row.Cells[8].AddParagraph("Termin wykonania");

            var lp = 1;
            foreach(OrderDto order in orders)
            {
                Row itemRow = table.AddRow();
                itemRow.Borders.Bottom = border03.Clone();
                itemRow.Borders.Right = border03.Clone();
                itemRow.Borders.Left = border03.Clone();
                itemRow.TopPadding = "0.0cm";
                itemRow.Format.Font.Size = 8;

                int? status = order.State;
                int? plannedQty = order.PlannedQty;
                int? finishedQty = order.FinishedQty;
                string number = order.Number;
                decimal rwState = order.RwCompletion;
                string name = order.Name;
                string client = order.ClientName;

                DateTime? dateAdded = order.OrderDate;//.Value.ToString("dd.MM.yyyy");
                DateTime? requiredDate = order.RequiredDate;//.Value.ToString("dd.MM.yyyy");

                itemRow.Cells[0].AddParagraph(lp.ToString());
                itemRow.Cells[1].AddParagraph(status.ToString());
                var column2paragraph = itemRow.Cells[2].AddParagraph(plannedQty.ToString());
                column2paragraph.AddText(" / ");
                column2paragraph.AddText(finishedQty.ToString());
                itemRow.Cells[3].AddParagraph(number);
                itemRow.Cells[4].AddParagraph(rwState.ToString("n1") + " %");
                itemRow.Cells[5].AddParagraph(name);
                if(client != null)
                    itemRow.Cells[6].AddParagraph(client);
                if(dateAdded != null)
                    itemRow.Cells[7].AddParagraph(dateAdded.Value.ToString("dd.MM.yyyy"));
                if(requiredDate != null)
                    itemRow.Cells[8].AddParagraph(requiredDate.Value.ToString("dd.MM.yyyy"));
                lp++;
            }


            return document;
        }

        public Document GenerateSubOrdersList(List<OrderBookDto> orderbooks)
        {
            Section section = document.AddSection();

            var border02 = new Border();
            border02.Width = 0.5;
            var border03 = new Border();
            border03.Width = 0.3;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            // Create the text frame for the address
            TextFrame rwFrame = section.AddTextFrame();
            rwFrame.Height = "2.3cm";
            rwFrame.Width = "19cm";
            rwFrame.Left = ShapePosition.Right;
            rwFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            rwFrame.Top = "1.0cm";
            rwFrame.RelativeVertical = RelativeVertical.Page;

            paragraph = rwFrame.AddParagraph("LISTA PODZLECEŃ");
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Style = "GreyBackground";
            //paragraph.AddLineBreak();

            paragraph = rwFrame.AddParagraph("");
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph = rwFrame.AddParagraph("Data wystawienia: ");
            paragraph.AddFormattedText(DateTime.Today.ToString("dd.MM.yyyy"), TextFormat.Bold);
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            Table table = section.AddTable();
            //table.Style = "Table";
            //table.Borders.Width = 0.15;
            table.Rows.LeftIndent = 0;
            table.Rows.VerticalAlignment = VerticalAlignment.Center;
            table.Rows.Height = "0.5cm";

            // Before you can add a row, you must define the columns
            // LP
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Number
            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Component Nimber
            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Name
            column = table.AddColumn("8cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Quantity
            column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            // Date added
            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            Row row = table.AddRow();
            row.Cells[0].AddParagraph("Lp.");
            row.Format.Font.Size = 8;
            row.Format.Font.Bold = true;
            row.Borders.Top = border02;
            row.Borders.Bottom = border02.Clone();
            row.Borders.Right = border02.Clone();
            row.Borders.Left = border02.Clone();

            row.Cells[0].Borders.Left = border02.Clone();
            row.Cells[1].AddParagraph("Numer");
            row.Cells[2].AddParagraph("Numer Części");
            row.Cells[3].AddParagraph("Nazwa");
            row.Cells[4].AddParagraph("L. szt.");
            row.Cells[5].AddParagraph("Data wpłynięcia");

            var lp = 1;
            foreach (OrderBookDto orderBook in orderbooks)
            {
                Row itemRow = table.AddRow();
                itemRow.Borders.Bottom = border03.Clone();
                itemRow.Borders.Right = border03.Clone();
                itemRow.Borders.Left = border03.Clone();
                itemRow.TopPadding = "0.0cm";
                itemRow.Format.Font.Size = 8;

                //int? status = orderBook.State;
                int? plannedQty = orderBook.PlannedQty;
                int? finishedQty = orderBook.FinishedQty;
                string number = orderBook.Number;
                //decimal rwState = orderBook.RwCompletion;
                string officeNumber = orderBook.Book.OfficeNumber;
                string name = orderBook.Book.Name;
                string componentNumber = orderBook.ComponentNumber;
                //string client = orderBook.ClientName;

                DateTime? dateAdded = orderBook.AddedDate;//.Value.ToString("dd.MM.yyyy");
                //DateTime? requiredDate = orderBook.RequiredDate;//.Value.ToString("dd.MM.yyyy");

                itemRow.Cells[0].AddParagraph(lp.ToString());

                itemRow.Cells[1].AddParagraph(number);

                itemRow.Cells[2].AddParagraph(componentNumber);

                var column3paragraph = itemRow.Cells[3].AddParagraph(officeNumber);
                column3paragraph.AddText(" - ");
                column3paragraph.AddText(name);

                var column4paragraph = itemRow.Cells[4].AddParagraph(plannedQty.ToString());
                column4paragraph.AddText(" / ");
                column4paragraph.AddText(finishedQty.ToString());
                if (dateAdded != null)
                    itemRow.Cells[5].AddParagraph(dateAdded.Value.ToString("dd.MM.yyyy"));
                //if (requiredDate != null)
                //    itemRow.Cells[8].AddParagraph(requiredDate.Value.ToString("dd.MM.yyyy"));
                lp++;
            }


            return document;
        }

        public Document GenerateOrderDetails(Order order, List<InProduction> inProductionItems)
        {
            var clientAddress = order.ShippingAddress == null ? "" : order.ShippingAddress;
            var clientCity = order.ShippingCity == null ? "" : order.ShippingCity;
            var clientPostalCode = order.ShippingPostalCode == null ? "" : order.ShippingPostalCode;
            var clientCountry = order.ShippingCountry == null ? "" : order.ShippingCountry;
            DateTime? dateAdded = order.OrderDate;
            DateTime? requiredDate = order.RequiredDate;

            Section section = document.AddSection();

            var border02 = new Border();
            border02.Width = 0.5;
            var border03 = new Border();
            border03.Width = 0.3;
            var noBorder = new Border();
            noBorder.Width = 0.0;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            // Create the text frame for the address
            TextFrame addressFrame = section.AddTextFrame();
            addressFrame.Height = "3.1cm";
            addressFrame.Width = "9.2cm";
            var borderFormat = new LineFormat();
            borderFormat.Color = new Color(0, 0, 0);
            addressFrame.LineFormat = borderFormat;
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "1.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;
            addressFrame.Margin = "0.1cm";

            // Create the text frame for the address
            TextFrame rwFrame = section.AddTextFrame();
            rwFrame.Height = "2.5cm";
            rwFrame.Width = "9.2cm";
            rwFrame.Left = ShapePosition.Right;
            rwFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            rwFrame.Top = "1.0cm";
            rwFrame.RelativeVertical = RelativeVertical.Page;

            paragraph = rwFrame.AddParagraph("SZCZEGÓŁY ZLECENIA");
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Style = "GreyBackground";
            paragraph.AddLineBreak();

            paragraph = addressFrame.AddParagraph("Zamawiający:  ");
            paragraph = addressFrame.AddParagraph(order.ClientName);
            paragraph.Format.Font.Size = 14;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceBefore = "0.15cm";
            paragraph.Format.SpaceAfter = "0.15cm";
            paragraph = addressFrame.AddParagraph();
            paragraph.AddFormattedText(clientAddress, TextFormat.Bold);
            paragraph = addressFrame.AddParagraph();
            paragraph.AddText(clientPostalCode);
            paragraph.AddTab();
            paragraph.AddText(clientCity);
            paragraph = addressFrame.AddParagraph();
            paragraph.AddText(clientCountry);


            paragraph = rwFrame.AddParagraph();

            paragraph.AddFormattedText(order.Number, TextFormat.Bold);
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Size = 18;
            paragraph.Format.SpaceBefore = "0.1cm";
            paragraph.Format.SpaceAfter = "0.1cm";
            paragraph = rwFrame.AddParagraph();
            paragraph.AddFormattedText(order.Name, TextFormat.Bold);
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Size = 12;

            paragraph.AddLineBreak();
            paragraph.AddLineBreak();

            paragraph = section.AddParagraph("Data wystawienia: ");
            paragraph.AddFormattedText(dateAdded.Value.ToString("dd.MM.yyyy"), TextFormat.Bold);
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceBefore = "2.25cm";
            paragraph.Format.SpaceAfter = "0.10cm";
            paragraph = section.AddParagraph("Termin wykonania: ");
            paragraph.AddFormattedText(requiredDate.Value.ToString("dd.MM.yyyy"), TextFormat.Bold);
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceAfter = "1cm";

            Table table = section.AddTable();
            table.Rows.LeftIndent = 0;
            table.Rows.VerticalAlignment = VerticalAlignment.Center;
            table.Rows.Height = "0.5cm";

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("5.0cm");
            column.Format.Alignment = ParagraphAlignment.Left;


            column = table.AddColumn("6.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            // Image
            column = table.AddColumn("1.7cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            // Quantity
            column = table.AddColumn("4.5cm");
            column.Format.Alignment = ParagraphAlignment.Center;


            var lp = 1;
            foreach (InProduction item in inProductionItems)
            {
                int itemLevel = item.BookLevelTree ?? 0;
                // Check for Book number ( main book assembly )
                if(itemLevel == 0)
                {
                    Row emptyRow = table.AddRow();
                    emptyRow.Borders.Top = new Border();
                    emptyRow.Borders.Right = new Border();
                    emptyRow.Borders.Left = new Border();
                    emptyRow.Borders.Bottom = new Border();

                    Row itemRow1 = table.AddRow();
                    itemRow1.Borders.Top = border03.Clone();
                    itemRow1.Borders.Right = border03.Clone();
                    itemRow1.Borders.Left = border03.Clone();
                    itemRow1.TopPadding = "0.0cm";
                    itemRow1.Format.Font.Size = 10;
                    for(int i=0; i < 5; i++)
                    {
                        itemRow1.Cells[i].Shading.Color = new Color(200, 200, 200);
                    }
                    itemRow1.Cells[0].MergeRight = 4;
                    itemRow1.Format.Font.Bold = true;
                    string bookInfo = item.Component.Number.ToString() + " (" + item.OrderBook.Book.OfficeNumber + ")" + " - " + item.Component.Name;
                    itemRow1.Cells[0].AddParagraph(bookInfo);
                    itemRow1.Cells[2].AddParagraph(item.Component.Name);
                    itemRow1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    itemRow1.Cells[0].Format.LeftIndent = "1cm";

                    Row row = table.AddRow();
                    row.Cells[0].AddParagraph("Lp.");
                    row.Format.Font.Size = 8;
                    row.Format.Font.Bold = true;
                    row.Borders.Top = border02.Clone();
                    row.Borders.Bottom = border02.Clone();
                    row.Borders.Right = border02.Clone();
                    row.Borders.Left = border02.Clone();
                    row.Cells[0].Borders.Left = border02.Clone();

                    row.Cells[1].AddParagraph("Numer rysunku");

                    row.Cells[2].AddParagraph("Nazwa");

                    row.Cells[3].AddParagraph("L. szt.");

                    row.Cells[4].AddParagraph("Uwagi");
                    lp = 1;
                }
                else
                {
                    Row itemRow1 = table.AddRow();
                    itemRow1.Borders.Top = border03.Clone();
                    itemRow1.Borders.Right = border03.Clone();
                    itemRow1.Borders.Left = border03.Clone();
                    itemRow1.Borders.Bottom = border03.Clone();
                    itemRow1.TopPadding = "0.0cm";
                    itemRow1.Format.Font.Size = 8;
                    itemRow1.Cells[0].AddParagraph(lp.ToString());
                    itemRow1.Cells[1].Format.LeftIndent = itemLevel * 8;
                    itemRow1.Cells[1].AddParagraph(item.Component.Number.ToString());
                    itemRow1.Cells[2].AddParagraph(item.Component.Name);
                    itemRow1.Cells[3].AddParagraph(item.PlannedQty.ToString());
                    itemRow1.Cells[4].AddParagraph(item.Component.Comment);

                    lp++;
                }

            }


            return document;

        }

        public Document GenerateZD(List<ComponentDto> wares, string orderNumber
           , string bookNumber, string bookComponentNumber, string subOrderNumber)
        {
            Section section = document.AddSection();

            var border02 = new Border();
            border02.Width = 0.5;
            var border03 = new Border();
            border03.Width = 0.3;

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();

            // Create the text frame for the address
            TextFrame addressFrame = section.AddTextFrame();
            addressFrame.Height = "2.3cm";
            addressFrame.Width = "9.2cm";
            var borderFormat = new LineFormat();
            borderFormat.Color = new Color(0, 0, 0);
            addressFrame.LineFormat = borderFormat;
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "1.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            // Create the text frame for the address
            TextFrame rwFrame = section.AddTextFrame();
            rwFrame.Height = "2.3cm";
            rwFrame.Width = "9.2cm";
            rwFrame.Left = ShapePosition.Right;
            rwFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            rwFrame.Top = "1.0cm";
            rwFrame.RelativeVertical = RelativeVertical.Page;

            // Put sender in address frame
            //paragraph.Format.Font.Name = "Verdana";
            paragraph.Format.Font.Size = 5;
            paragraph.Format.SpaceAfter = 0;

            paragraph = addressFrame.AddParagraph("SPÓŁKA STOLARCZYK Tomasz Stolarczyk, Janusz Stolarczyk");
            paragraph.AddLineBreak();
            paragraph.AddText("Kochanowskiego 30");
            paragraph.AddLineBreak();
            paragraph.AddText("33-100 Tarnów");
            paragraph.AddLineBreak();
            paragraph.AddText("NIP: 873-020-41-85");
            paragraph.Format.Font.Bold = true;
            paragraph.Format.LeftIndent = "0.1cm";
            paragraph.Format.SpaceBefore = "0.1cm";

            paragraph = rwFrame.AddParagraph("Zamówienie u dostawcy ZD");
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Style = "GreyBackground";
            paragraph.AddLineBreak();

            paragraph = rwFrame.AddParagraph("");
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            paragraph = rwFrame.AddParagraph("Data wystawienia: ");
            paragraph.AddFormattedText(DateTime.Today.ToString("dd.MM.yyyy"), TextFormat.Bold);
            paragraph.Format.Font.Size = 10;
            paragraph.Format.Alignment = ParagraphAlignment.Right;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "2.5cm";
            paragraph.Style = "Reference";
            paragraph.AddText("Zlecenie: ");
            paragraph.AddFormattedText(orderNumber, TextFormat.Bold);
            paragraph.Format.SpaceAfter = "0.1cm";

            Paragraph paragraph3 = section.AddParagraph();
            paragraph3.AddText("Numer podzlecenia: ");
            paragraph3.AddFormattedText(subOrderNumber, TextFormat.Bold);
            paragraph3.Format.SpaceAfter = "0.1cm";

            Paragraph paragraph2 = section.AddParagraph();
            paragraph2.AddText("Numer książki: ");
            paragraph2.AddFormattedText(bookNumber, TextFormat.Bold);
            paragraph2.AddFormattedText(" (", TextFormat.Bold);
            paragraph2.AddFormattedText(bookComponentNumber, TextFormat.Bold);
            paragraph2.AddFormattedText(")", TextFormat.Bold);
            paragraph2.Format.SpaceAfter = "2cm";

            Table table = section.AddTable();
            //table.Style = "Table";
            //table.Borders.Width = 0.15;
            table.Rows.LeftIndent = 0;
            table.Rows.VerticalAlignment = VerticalAlignment.Center;
            table.Rows.Height = "0.5cm";

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;
            column = table.AddColumn("3.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("7.5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            //Quantity
            column = table.AddColumn("1.4cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            // Length
            column = table.AddColumn("1.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Sum
            column = table.AddColumn("1.4cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Unit
            column = table.AddColumn("1.0cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Wydano
            column = table.AddColumn("1.6cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            row.Cells[0].AddParagraph("Lp.");
            row.Format.Font.Size = 8;
            row.Format.Font.Bold = true;
            row.Borders.Top = border02;
            row.Borders.Bottom = border02.Clone();
            row.Cells[0].Borders.Left = border02.Clone();

            row.Cells[1].AddParagraph("Numer rysunku");

            row.Cells[2].AddParagraph("Kod towaru");

            row.Cells[3].AddParagraph("Liczba szt.");

            row.Cells[4].AddParagraph("Długość 1 szt.");

            row.Cells[5].AddParagraph("Łącznie");

            row.Cells[6].AddParagraph("J.m.");

            row.Cells[7].AddParagraph("Wydano");
            row.Cells[7].Borders.Right = border02.Clone();

            var lp = 1;

            //var sum = wares.SelectMany(x => x.ComponentWareDtos).GroupBy(u => new { u.WareCode, u.Length });

            foreach (ComponentDto item in wares)
            {
                Row itemRow = table.AddRow();
                itemRow.Borders.Bottom = border03.Clone();
                var currentNumber = item.Number;
                itemRow.TopPadding = "0.2cm";
                itemRow.Format.Font.Size = 8;
                itemRow.Cells[0].AddParagraph(lp.ToString());
                itemRow.Cells[1].AddParagraph(currentNumber);

                var i = 0;
                //foreach (ComponentWareDto ware in item.ComponentWareDtos)
                //{
                //    if (i > 0)
                //    {
                //        itemRow.Cells[0].AddParagraph("");
                //        itemRow.Cells[1].AddParagraph("");
                //    }
                //    itemRow.Cells[2].AddParagraph(ware.WareCode);
                //    string quantity = String.Concat(ware.Quantity.ToString(), " (",
                //        ware.TotalQuantity.ToString(), ")");
                //    if (ware.QtyWhDiff < 0)
                //    {
                //        quantity = String.Concat(quantity, " !!!");
                //    }
                //    itemRow.Cells[3].AddParagraph(quantity);
                //    itemRow.Cells[3].Format.Font.Bold = true;
                //    itemRow.Cells[4].AddParagraph(ware.Length.ToString().Replace(".", ","));
                //    itemRow.Cells[5].AddParagraph((ware.Quantity * ware.Length).ToString().Replace(".", ","));
                //    itemRow.Cells[6].AddParagraph(ware.Unit);
                //    i++;
                //}


                lp++;
            }


            return document;
        }

    }
}
