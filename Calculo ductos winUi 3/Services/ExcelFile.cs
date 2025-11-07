using Aspose.Cells;
using Aspose.Cells.Drawing;
using Calculo_ductos.Params;
using Calculo_ductos_winUi_3.Models;
using Calculo_ductos_winUi_3.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using supporExcel = ClosedXML;

namespace Calculo_ductos_winUi_3.Services
{
    internal static class ExcelFile
    {
        #region fields
        private static Style greenBackground = new Style();
        private static Style greenBackgroundYellowWords = new Style();
        private static Style yellowBackground = new Style();
        private static Style yellowBackgroundBorder = new Style();
        private static Style yellowBackgroundRedWords = new Style();
        private static Style styleYellowFont = new Style();
        private static Style redWords = new Style();
        private static Style greenWords = new Style();
        private static Style defaultStyle = new Style();
        private static Style redWordsBlackBorder = new Style();
        private static Style redWordsYellowBackground = new Style();
        private static int _widthCoulmnA = 13;
        #endregion

        private static StateViewModel _state; 
        public static async Task ExportToExcel(this StateViewModel state, string filePath = null) {
            try
            {
                _state = state;
                using var workbook = new Workbook();
                //var worksheet = workbook.Worksheets.Add("DUCTO 1");
                var worksheet = workbook.Worksheets[0];

                worksheet.Name = "DUCTO 1";
                await CreateTemplateSheet(worksheet);
                workbook.Save(filePath, SaveFormat.Xlsx);
            }
            catch (Exception ex)
            {

                var message = ex.Message;
                Console.WriteLine(message); 
            }
            

        }
        public static async Task FinishExport(this StateViewModel state, string filePath = null)
        {
            try
            {
                if (filePath != null)
                {
                    using (var workbook = new supporExcel.Excel.XLWorkbook(filePath))
                    {
                        // Eliminar la hoja llamada "Hoja2"
                        workbook.Worksheet("Evaluation Warning").Delete();
                        //Obtener la primera hoja
                        var ductSheet = workbook.Worksheet("DUCTO 1");
                        // Proteger la hoja con contraseña
                        ductSheet.Protect("proyectosVertical2025");
                        // Guardar los cambios
                        workbook.SaveAs(filePath);
                    }
                }
            }
            catch (Exception ex)
            {

                var message = ex.Message;
                Console.WriteLine(message);
            }
            
        }
        public static async Task CreateTemplateSheet(Worksheet worksheet)
        {
            // Alineación horizontal centrada a toda la hoja
            Style styleCenter = worksheet.Cells.Rows[0].GetStyle();
            styleCenter.HorizontalAlignment = TextAlignmentType.Center;
            styleCenter.IsTextWrapped = true;
            worksheet.Cells.SetColumnWidth(0, _widthCoulmnA);
            worksheet.Cells.ApplyStyle(styleCenter, new StyleFlag { HorizontalAlignment = true, WrapText = true });


            SetStyles(ref worksheet);
            // Encabezados
            WriteHeaders(worksheet, out int currentRow);
            InsertPicture(worksheet);
            // Cargar datos del JSON
            KitCollection dataTemplate = await LoadKitsFromJsonAsync();

            // Escribir listas de kits
            //WriteKitList(dataTemplate.Ducts, worksheet, ref currentRow);
            WriteKitList(worksheet, ref currentRow);

            //currentRow++;
            //currentRow++;

            //// Escribir títulos de columna
            //worksheet.Cells[currentRow, 0].PutValue("Kit");           // Columna A
            //worksheet.Cells[currentRow, 1].PutValue("Descripcion");   // Columna B
            //worksheet.Cells[currentRow, 8].PutValue("Cantidad");      // Columna I
            //worksheet.Cells[currentRow, 9].PutValue("Total");         // Columna J

            //// Combinar celdas de B hasta H (índices 1 a 7)
            //worksheet.Cells.Merge(currentRow, 1, 1, 7);

            //// Estilizar desde A hasta J (índices 0 a 9)
            //var range = worksheet.Cells.CreateRange(currentRow, 0, 1, 10);
            //Style style = worksheet.Workbook.CreateStyle();
            //style.Font.Color = System.Drawing.ColorTranslator.FromHtml("#00B0AC");
            //style.Font.IsBold = true;

            //StyleFlag flag = new StyleFlag { FontColor = true, FontBold = true };
            //range.ApplyStyle(style, flag);

            //currentRow++;
            ////basura
            //if (_state.CompleteDuctVm.PurposeId == 1)
            //{
            //    // Escribir más listas
            //    WriteKitList(dataTemplate.Guillotine, worksheet, ref currentRow, TextAlignmentType.Left);
            //    currentRow++;

            //    WriteKitList(dataTemplate.Container, worksheet, ref currentRow, TextAlignmentType.Left);
            //    currentRow++;

            //    WriteKitList(dataTemplate.General, worksheet, ref currentRow, TextAlignmentType.Left);
            //    currentRow++;
            //}
            ////ropa
            //else {
            //    WriteKitList(dataTemplate.Clothes, worksheet, ref currentRow, TextAlignmentType.Left);
            //    currentRow++;
            //}

            // Pie de página
            WriteFooters(worksheet, ref currentRow);

            // Ajustar ancho de columnas
            worksheet.AutoFitColumns(1,14);
            worksheet.AutoFitRows();
        }

        private static void SetStyles(ref Worksheet worksheet)
        {
            Color green = ColorTranslator.FromHtml("#00B0AC");
            Color yellow = ColorTranslator.FromHtml("#FFD966");
            Color yellowB = ColorTranslator.FromHtml("#FFF2CC");
            Color red = ColorTranslator.FromHtml("#FF0000");

            greenBackground = worksheet.Workbook.CreateStyle();
            greenBackground.ForegroundColor = green;
            greenBackground.Pattern = BackgroundType.Solid;
            greenBackground.Font.Color = Color.White;
            greenBackground.Font.IsBold = true;
            greenBackground.VerticalAlignment = TextAlignmentType.Center;
            greenBackground.HorizontalAlignment = TextAlignmentType.Center;
            greenBackground.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Dotted;
            greenBackground.Borders[BorderType.TopBorder].Color = green;
            greenBackground.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Dotted;
            greenBackground.Borders[BorderType.LeftBorder].Color = green;
            greenBackground.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Dotted;
            greenBackground.Borders[BorderType.RightBorder].Color = green;
            greenBackground.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Dotted;
            greenBackground.Borders[BorderType.BottomBorder].Color = green;

            greenBackgroundYellowWords = worksheet.Workbook.CreateStyle();
            greenBackgroundYellowWords.ForegroundColor = green;
            greenBackgroundYellowWords.Pattern = BackgroundType.Solid;
            greenBackgroundYellowWords.Font.Color = yellow;
            greenBackgroundYellowWords.Font.IsBold = true;
            greenBackgroundYellowWords.HorizontalAlignment = TextAlignmentType.Center;

            yellowBackground = worksheet.Workbook.CreateStyle();
            yellowBackground.ForegroundColor = yellowB;
            yellowBackground.Pattern = BackgroundType.Solid;
            yellowBackground.Font.IsBold = true;
            yellowBackground.HorizontalAlignment = TextAlignmentType.Center;
            
            yellowBackgroundBorder = worksheet.Workbook.CreateStyle();
            yellowBackgroundBorder.ForegroundColor = yellowB;
            yellowBackgroundBorder.Pattern = BackgroundType.Solid;
            yellowBackgroundBorder.Font.IsBold = true;
            yellowBackgroundBorder.HorizontalAlignment = TextAlignmentType.Center;
            yellowBackgroundBorder.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            yellowBackgroundBorder.Borders[BorderType.TopBorder].Color = green;
            yellowBackgroundBorder.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            yellowBackgroundBorder.Borders[BorderType.LeftBorder].Color = green;
            yellowBackgroundBorder.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            yellowBackgroundBorder.Borders[BorderType.RightBorder].Color = green;
            yellowBackgroundBorder.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            yellowBackgroundBorder.Borders[BorderType.BottomBorder].Color = green;

            yellowBackgroundRedWords = worksheet.Workbook.CreateStyle();
            yellowBackgroundRedWords.ForegroundColor = yellowB;
            yellowBackgroundRedWords.Pattern = BackgroundType.Solid;
            yellowBackgroundRedWords.Font.IsBold = true;
            yellowBackgroundRedWords.Font.Color = red;
            yellowBackgroundRedWords.HorizontalAlignment = TextAlignmentType.Center;

            styleYellowFont = worksheet.Workbook.CreateStyle();
            styleYellowFont.Font.Color = yellow;
            styleYellowFont.Font.IsBold = true;
            styleYellowFont.HorizontalAlignment = TextAlignmentType.Center;

            redWords = worksheet.Workbook.CreateStyle();
            redWords.Font.Color = red;
            redWords.Font.IsBold = true;
            redWords.HorizontalAlignment = TextAlignmentType.Center;
            redWords.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            redWords.Borders[BorderType.TopBorder].Color = green;
            redWords.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            redWords.Borders[BorderType.LeftBorder].Color = green;
            redWords.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            redWords.Borders[BorderType.RightBorder].Color = green;
            redWords.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            redWords.Borders[BorderType.BottomBorder].Color = green;

            redWordsBlackBorder = worksheet.Workbook.CreateStyle();
            redWordsBlackBorder.Font.Color = red;
            redWordsBlackBorder.Font.IsBold = true;
            redWordsBlackBorder.HorizontalAlignment = TextAlignmentType.Left;
            redWordsBlackBorder.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Dotted;
            redWordsBlackBorder.Borders[BorderType.TopBorder].Color = Color.Black;
            redWordsBlackBorder.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Dotted;
            redWordsBlackBorder.Borders[BorderType.LeftBorder].Color = Color.Black;
            redWordsBlackBorder.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Dotted;
            redWordsBlackBorder.Borders[BorderType.RightBorder].Color = Color.Black;
            redWordsBlackBorder.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Dotted;
            redWordsBlackBorder.Borders[BorderType.BottomBorder].Color = Color.Black;


            redWordsYellowBackground = worksheet.Workbook.CreateStyle();
            redWordsYellowBackground.Font.Color = red;
            redWordsYellowBackground.ForegroundColor = yellowB;
            redWordsYellowBackground.Pattern = BackgroundType.Solid;
            redWordsYellowBackground.Font.IsBold = true;
            redWordsYellowBackground.HorizontalAlignment = TextAlignmentType.Center;
            redWordsYellowBackground.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Dotted;
            redWordsYellowBackground.Borders[BorderType.TopBorder].Color = green;
            redWordsYellowBackground.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Dotted;
            redWordsYellowBackground.Borders[BorderType.BottomBorder].Color = green;
            redWordsYellowBackground.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Dotted;
            redWordsYellowBackground.Borders[BorderType.LeftBorder].Color = green;
            redWordsYellowBackground.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Dotted;
            redWordsYellowBackground.Borders[BorderType.RightBorder].Color = green;

            greenWords = worksheet.Workbook.CreateStyle();
            greenWords.Font.Color = green;
            greenWords.Font.IsBold = true;
            greenWords.HorizontalAlignment = TextAlignmentType.Center;
            greenWords.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            greenWords.Borders[BorderType.TopBorder].Color = green;
            greenWords.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
            greenWords.Borders[BorderType.BottomBorder].Color = green;
            greenWords.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            greenWords.Borders[BorderType.LeftBorder].Color = green;
            greenWords.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            greenWords.Borders[BorderType.RightBorder].Color = green;

            defaultStyle = worksheet.Workbook.CreateStyle();
            defaultStyle.HorizontalAlignment = TextAlignmentType.Center;
            defaultStyle.Font.Color = default;
            defaultStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Dotted;
            defaultStyle.Borders[BorderType.TopBorder].Color = green;
            defaultStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Dotted;
            defaultStyle.Borders[BorderType.BottomBorder].Color = green;
            defaultStyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Dotted;
            defaultStyle.Borders[BorderType.LeftBorder].Color = green;
            defaultStyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Dotted;
            defaultStyle.Borders[BorderType.RightBorder].Color = green;
            defaultStyle.IsTextWrapped = true;


        }
        private static async Task<KitCollection> LoadKitsFromJsonAsync()
        {
            try
            {
                var assembly = typeof(StateViewModel).Assembly;
                foreach (var name in assembly.GetManifestResourceNames())
                {
                    System.Diagnostics.Debug.WriteLine("RECURSO: " + name);
                }
                //string basePath = AppContext.BaseDirectory;
                //var assembly = typeof(StateViewModel).Assembly;
                using Stream stream = assembly.GetManifestResourceStream("Calculo_ductos_winUi_3.Assets.Ducts.json");

                //using FileStream stream = File.OpenRead(jsonPath);

                var kitList = await JsonSerializer.DeserializeAsync<List<KitCollection>>(stream);

                // Como viene dentro de un array con un solo objeto
                return kitList?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error leyendo JSON: {ex.Message}");
                return default;
            }
        }

        private static void WriteHeaders(Worksheet worksheet, out int currentRow)
        {
            // Primera fila
            var proporsito = _state.CompleteDuctVm.PurposeId == 0 ? "BASURA" : "ROPA SUCIA";
            worksheet.Cells[0, 0].PutValue($"DUCTO DE {proporsito}");
            worksheet.Cells.CreateRange(0, 0, 1, 10).Merge();
            var range = worksheet.Cells.CreateRange(0, 0, 1, 10);
            range.SetStyle(greenBackground, true);

            // Fila 2
            worksheet.Cells[1, 1].PutValue("Proyecto:");
            worksheet.Cells[1, 1].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells.CreateRange(1, 2, 1, 3).Merge();
            worksheet.Cells.CreateRange(1, 6, 1, 2).Merge();
            //worksheet.Cells.CreateRange(1, 8, 1, 1).Merge();
            worksheet.Cells.CreateRange(1, 2, 4, 3).SetStyle(yellowBackground,true);
            worksheet.Cells.CreateRange(1, 6, 4, 2).SetStyle(yellowBackground, true);
            worksheet.Cells.CreateRange(1, 8, 3, 1).SetStyle(yellowBackground, true);
            worksheet.Cells.CreateRange(1, 9, 3, 1).SetStyle(yellowBackground, true);

            worksheet.Cells[1, 5].PutValue("Obra:");
            worksheet.Cells[1, 5].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells[1, 8].PutValue("Version:");
            worksheet.Cells[1, 8].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells.CreateRange(1, 13, 1, 2).Merge();
            worksheet.Cells.CreateRange(1, 13, 1, 2).SetStyle(greenBackground);

            // Fila 3
            worksheet.Cells[2, 1].PutValue("Sistema:");
            worksheet.Cells[2, 1].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells[2, 5].PutValue("Oportunidad:");
            worksheet.Cells[2, 5].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells[2, 8].PutValue("Fecha:");
            worksheet.Cells[2, 8].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells.CreateRange(2, 2, 1, 3).Merge();
            worksheet.Cells.CreateRange(2, 6, 1, 2).Merge();
            worksheet.Cells.CreateRange(2, 14, 2, 1).SetStyle(redWords);

            // Fila 4
            worksheet.Cells[3, 1].PutValue("Diametro:");
            worksheet.Cells[3, 1].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells[3, 5].PutValue("Unidad:");
            worksheet.Cells[3, 5].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells[3, 8].PutValue("Realizo:");
            worksheet.Cells[3, 8].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Right });
            worksheet.Cells.CreateRange(3, 2, 1, 3).Merge();
            worksheet.Cells.CreateRange(3, 6, 1, 2).Merge();

            // Fila 5
            worksheet.Cells[4, 1].PutValue("Contacto:");
            worksheet.Cells[4, 1].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Left });
            worksheet.Cells[4, 5].PutValue("Estimacion:");
            worksheet.Cells[4, 5].SetStyle(new Style() { HorizontalAlignment = TextAlignmentType.Left });
            worksheet.Cells[4, 8].PutValue("Dias:");
            worksheet.Cells[4, 9].Formula = "=O107"; // Fórmula de celda
            worksheet.Cells.CreateRange(4, 2, 1, 3).Merge();
            worksheet.Cells.CreateRange(4, 6, 1, 2).Merge();
            worksheet.Cells.CreateRange(4, 13, 1, 2).Merge();
            worksheet.Cells.CreateRange(4, 13, 1, 2).SetStyle(greenBackground);
            worksheet.Cells.CreateRange(4, 8, 1, 2).SetStyle(greenBackgroundYellowWords, true);

            // Fila 6
            worksheet.Cells[5, 0].PutValue("Lamina Galvanizada (Optimizado Calibre 18 - 20)");
            worksheet.Cells.CreateRange(5, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(5, 13, 2, 1).Merge();
            worksheet.Cells.CreateRange(5, 14, 2, 1).Merge();
            worksheet.Cells.CreateRange(5, 0, 1, 10).SetStyle(greenBackground, true);
            worksheet.Cells.CreateRange(5, 13, 2, 2).SetStyle(greenBackground, true);

            // Fila 7
            worksheet.Cells[6, 0].PutValue("Kit");
            worksheet.Cells[6, 1].PutValue("Descripcion");
            worksheet.Cells[6, 8].PutValue("Cantidad");
            worksheet.Cells[6, 9].PutValue("Total");
            worksheet.Cells.CreateRange(6, 1, 1, 7).Merge();
            worksheet.Cells.CreateRange(6, 0, 1, 10).SetStyle(greenWords);

            // Información adicional
            worksheet.Cells[1, 13].PutValue("UBICACIÓN");
            worksheet.Cells[2, 13].PutValue("CDMX Y ZM");
            worksheet.Cells[2, 13].SetStyle(defaultStyle, new StyleFlag { WrapText = true});
            worksheet.Cells[3, 13].PutValue("FORÁNEO");
            worksheet.Cells[3, 13].SetStyle(defaultStyle, new StyleFlag { WrapText = true });
            worksheet.Cells[2, 14].PutValue("0");
            worksheet.Cells[3, 14].PutValue("1");
            worksheet.Cells[4, 13].PutValue("TIEMPOS DE EJECUCIÓN");
            worksheet.Cells[5, 13].PutValue("HRS DE INSTALACIÓN");
            worksheet.Cells[5, 14].PutValue("TOTAL");

            currentRow = 7;
        }
        public static void WriteKitList(Worksheet sheet, ref int currentRow, TextAlignmentType alignmentType = TextAlignmentType.Center)
        {
            TextAlignmentType alignmentTypeB = TextAlignmentType.Center;
            foreach (CatalogKitModel kit in _state.CompleteDuctVm.AvailableKits)
            {
                var kitCount = GetElementCount(kit.Item);
                if (kitCount > 0)
                {
                    sheet.Cells[currentRow, 0].PutValue(kit.Item);          // Columna A
                    sheet.Cells[currentRow, 0].SetStyle(defaultStyle);          // Columna A
                    sheet.Cells[currentRow, 1].PutValue(kit.Description);  // Columna B
                    sheet.Cells[currentRow, 1].SetStyle(defaultStyle);          // Columna A
                    sheet.Cells.Merge(currentRow, 1, 1, 7);                  // Bx:Hx
                    defaultStyle.HorizontalAlignment = alignmentType;
                    sheet.Cells.CreateRange(currentRow, 1, 1, 7).SetStyle(defaultStyle);                  // Bx:Hx

                    defaultStyle.HorizontalAlignment = alignmentTypeB;

                    //sheet.Cells[currentRow, 8].PutValue(item.Count);        // Columna I
                    //sheet.Cells[currentRow, 8].PutValue(GetElementCount(item.Kit));        // Columna I
                    sheet.Cells[currentRow, 8].PutValue(kitCount);        // Columna I
                    sheet.Cells[currentRow, 8].SetStyle(defaultStyle);          // Columna A
                    sheet.Cells[currentRow, 9].Formula = $"=I{currentRow + 1}"; // Jx
                    sheet.Cells[currentRow, 9].SetStyle(defaultStyle);          // Columna A

                    sheet.Cells[currentRow, 13].PutValue(0); // N
                    sheet.Cells[currentRow, 13].SetStyle(defaultStyle);          // Columna A
                    sheet.Cells[currentRow, 14].Formula = $"=J{currentRow + 1}*N{currentRow + 1}"; // O
                    sheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder); // O

                    currentRow++;
                }
            }
        }
        private static void WriteKitList(List<KitModel> list, Worksheet sheet, ref int currentRow, TextAlignmentType alignmentType = TextAlignmentType.Center)
        {
          
            TextAlignmentType alignmentTypeB = TextAlignmentType.Center;
            foreach (KitModel item in list)
            {
                var kitCount = GetElementCount(item.Kit);
                if (kitCount > 0)
                {
                    sheet.Cells[currentRow, 0].PutValue(item.Kit);          // Columna A
                    sheet.Cells[currentRow, 0].SetStyle(defaultStyle);          // Columna A
                    sheet.Cells[currentRow, 1].PutValue(item.Description);  // Columna B
                    sheet.Cells[currentRow, 1].SetStyle(defaultStyle);          // Columna A
                    sheet.Cells.Merge(currentRow, 1, 1, 7);                  // Bx:Hx
                    defaultStyle.HorizontalAlignment = alignmentType;
                    sheet.Cells.CreateRange(currentRow, 1, 1, 7).SetStyle(defaultStyle);                  // Bx:Hx

                    defaultStyle.HorizontalAlignment = alignmentTypeB;

                    //sheet.Cells[currentRow, 8].PutValue(item.Count);        // Columna I
                    //sheet.Cells[currentRow, 8].PutValue(GetElementCount(item.Kit));        // Columna I
                    sheet.Cells[currentRow, 8].PutValue(kitCount);        // Columna I
                    sheet.Cells[currentRow, 8].SetStyle(defaultStyle);          // Columna A
                    sheet.Cells[currentRow, 9].Formula = $"=I{currentRow + 1}"; // Jx
                    sheet.Cells[currentRow, 9].SetStyle(defaultStyle);          // Columna A

                    sheet.Cells[currentRow, 13].PutValue(item.InstalationTime); // N
                    sheet.Cells[currentRow, 13].SetStyle(defaultStyle);          // Columna A
                    sheet.Cells[currentRow, 14].Formula = $"=J{currentRow + 1}*N{currentRow + 1}"; // O
                    sheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder); // O

                    currentRow++;
                }
            }
        }
        private static void WriteFooters(Worksheet worksheet, ref int currentRow)
        {
            // Colores en Aspose.Cells (No es necesario usar ColorTranslator)
            //Style greenBackground = worksheet.Workbook.CreateStyle();
            //greenBackground.ForegroundColor = ColorTranslator.FromHtml("#00B0AC");
            //greenBackground.Pattern = BackgroundType.Solid;
            //greenBackground.Font.Color = Color.White;
            //greenBackground.Font.IsBold = true;
            var fontColor = redWordsBlackBorder.Font.Color;
            redWordsBlackBorder.Font.Color = Color.Black;
            // Notas importantes
            worksheet.Cells[currentRow, 0].PutValue("Notas importantes:");
            worksheet.Cells[currentRow, 0].SetStyle(new Style() { Font = { IsBold = true } });

            worksheet.Cells[currentRow, 11].PutValue("Horas de instalación");
            var range = worksheet.Cells.CreateRange(currentRow, 11, 1, 3);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells[currentRow, 14].Formula = "=SUM(O9:O52,O56:O70,O79:O89)";
            worksheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder);

            redWordsBlackBorder.Font.Color = fontColor;

           currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Jornadas de trabajo");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 3);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells[currentRow, 14].Formula = "=O91/8";
            worksheet.Cells[currentRow, 14].SetStyle(redWordsYellowBackground);

            currentRow++;

            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            currentRow++;

            // ACTIVIDADES ADICIONALES DE OBRA
            worksheet.Cells[currentRow, 11].PutValue("ACTIVIDADES ADICIONALES DE OBRA");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 4);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 4).Merge();

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Acarreo y distribución de ducto por nivel");
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).SetStyle(defaultStyle);
            worksheet.Cells[currentRow, 14].Formula = "=(0.4*J8)";
            worksheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder);

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Entrega por nivel, instalación de marco y limpieza de puertas");
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).SetStyle(defaultStyle);
            worksheet.Cells[currentRow, 14].Formula = "=(1*J8+1)";
            worksheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder);

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Horas de instalación");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 3);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells[currentRow, 14].Formula = "=SUM(O95:O96)";
            worksheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder);

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Jornadas de trabajo");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 3);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells[currentRow, 14].Formula = "=O97/8";
            worksheet.Cells[currentRow, 14].SetStyle(redWordsYellowBackground);

            currentRow++;
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();

            currentRow++;
            // ACTIVIDADES ADMINISTRATIVAS
            worksheet.Cells[currentRow, 11].PutValue("ACTIVIDADES ADMINISTRATIVAS");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 4);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 4).Merge();

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Recepción, acopio, almacenamiento de materiales y recorrido en obra");
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).SetStyle(defaultStyle);
            worksheet.Cells[currentRow, 14].PutValue("8");
            worksheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder);

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Traslado de personal (redondo y solo foraneas)");
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).SetStyle(defaultStyle);
            worksheet.Cells[currentRow, 14].Formula = "=O4*8";
            worksheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder);

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Horas de instalación");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 3);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells[currentRow, 14].Formula = "=SUM(O101:O102)";
            worksheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder);

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("Jornadas de trabajo");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 3);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).Merge();
            worksheet.Cells.CreateRange(currentRow, 0, 1, 10).SetStyle(redWordsBlackBorder);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells[currentRow, 14].Formula = "=O103/8";
            worksheet.Cells[currentRow, 14].SetStyle(redWordsYellowBackground);

            currentRow++;
            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("HORAS TOTALES");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 3);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells[currentRow, 14].Formula = "=SUM(O91,O97,O103)";
            worksheet.Cells[currentRow, 14].SetStyle(yellowBackgroundBorder);

            currentRow++;
            worksheet.Cells[currentRow, 11].PutValue("JORNADAS DE TRABAJO");
            range = worksheet.Cells.CreateRange(currentRow, 11, 1, 3);
            range.SetStyle(greenBackground);
            worksheet.Cells.CreateRange(currentRow, 11, 1, 3).Merge();
            worksheet.Cells[currentRow, 14].Formula = "=SUM(O92,O98,O104)";
            worksheet.Cells[currentRow, 14].SetStyle(redWordsYellowBackground);
        }
        private static int GetElementCount(string kit) 
        {
            int count = 0;
            try
            {
                switch (kit)
                {
                    //DUCTS
                    case "B872523":
                    case "B603118": count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.A2).FirstOrDefault()?.Count ?? 0; break;
                    case "B872526":
                    case "B603121": count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.B2).FirstOrDefault()?.Count ?? 0; break;
                    case "B872525":
                    case "B603120": count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.B3).FirstOrDefault()?.Count ?? 0; break;
                    case "B872524":
                    case "B101114": count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.B4).FirstOrDefault()?.Count ?? 0; break;
                    case "B872521":
                    case "B603115": count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.S4).FirstOrDefault()?.Count ?? 0; break;
                    case "B872633":
                    case "B872615": count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.B4F).FirstOrDefault()?.Count ?? 0; break;
                    case "B903058":
                    case "B872614": count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.B3F).FirstOrDefault()?.Count ?? 0; break;
                    case "B903057":
                    case "B872613": count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.B2F).FirstOrDefault()?.Count ?? 0; break;
                    //COMPONENTS
                    case "B603001": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.Sprinkler).FirstOrDefault()?.Count ?? 0; break;
                    case "B601001": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.DisinfectionSystem).FirstOrDefault()?.Count ?? 0; break;
                    case "B101024":
                    case "B1010241": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.XN).FirstOrDefault()?.Count ?? 0; break;
                    case "B101130": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.XNF).FirstOrDefault()?.Count ?? 0; break;
                    case "B603128":
                    case "B701190": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.Chimney).FirstOrDefault()?.Count ?? 0; break;
                    case "B872527":
                    case "B101118": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.TVA).FirstOrDefault()?.Count ?? 0; break;
                    case "B602103": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.Guillotine).FirstOrDefault()?.Count ?? 0; break;
                    case "B602002": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.Discharge).FirstOrDefault()?.Count ?? 0; break;
                    case "B50200054": count = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.Container).FirstOrDefault()?.Count ?? 0; break;
                    case "B601032": count = _state.DuctsVM.DuctDetailList.Count >= 10 ? _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.AntiImpact).FirstOrDefault()?.Count ?? 0 : 0; break;
                    case "B903068": count = _state.DuctsVM.DuctDetailList.Count < 10 ? _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.AntiImpact).FirstOrDefault()?.Count ?? 0 : 0; break;


                }
                var gatesCount = _state.ComponentsVM.ComponentList.Where(component => component.Type == Component.TypeComponent.Gate).FirstOrDefault()?.Count ?? 0;
                var c4Count = _state.DuctsVM.DucList.Where(duct => duct.Type == DuctPiece.TypeDuct.C4).FirstOrDefault()?.Count ?? 0;
                //Default para ropa
                if (_state.CompleteDuctVm.PurposeId == 0)
                {
                    
                    switch (kit) 
                    {
                        //ropa c4
                        case "B872520": count = c4Count; break;
                        ////puerta UL derecha
                        case "B301057": 
                        ////puerta UL izquierda 
                        case "B301055": 
                        ////puerta Inoxidable derecha
                        case "B301080": 
                        ////puerta Inoxidable izquierda
                        case "B301079": count = GetDoorCount(kit); break;

                    }
                }
                //Default para basura
                else 
                {
                    switch (kit)
                    {
                        //basura c4
                        case "B101110": count = c4Count; break;
                        ////puerta Pintada
                        case "B301061":
                        ////puerta Inoxidable
                        case "B301064": 
                        ////puerta UL
                        case "B301033": count = GetDoorCount(kit); break;

                    }
                }
                
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.Message);
            }
            
            return count;
        }
        private static void InsertPicture(Worksheet worksheet)
        {
            var resourceName = "Calculo_ductos_winUi_3.Assets.LogoVertical.png";
            var assembly = Assembly.GetExecutingAssembly();
            using Stream? imageStream = assembly.GetManifestResourceStream(resourceName);
            if (imageStream == null)
                throw new Exception("No se pudo cargar el recurso incrustado.");

            // Insertar imagen en A2
            int pictureIndex = worksheet.Pictures.Add(1, 0, imageStream);
            var picture = worksheet.Pictures[pictureIndex];

            // Establecer la fila y columna de inicio
            picture.UpperLeftRow = 1;    // Fila 2
            picture.UpperLeftColumn = 0; // Columna A

            // Calcular el alto total de las filas 2 a 5
            double totalHeight = 0;
            for (int i = 1; i <= 4; i++)
                totalHeight += worksheet.Cells.GetRowHeightPixel(i);

            
            int columnWidthInPixels = worksheet.Cells.GetColumnWidthPixel(0);
            // Calcular el ancho total de la columna A (solo una columna en este caso)
            //double totalWidth = _widthCoulmnA;

            // Asignar dimensiones
            picture.Width = (int)columnWidthInPixels;
            picture.Height = (int)totalHeight;
            picture.Placement = PlacementType.Move;
        }
        private static int GetDoorCount(string kit)
        {
            var count = 0;
            try
            {
                count = _state.FloorVM.FloorList
                    .Where(floor => floor.TypeDoor.IdSyteLine == kit && floor.NeedGate && floor.Type!=Floor.TypeFloor.discharge)
                    .Sum(floor => floor.FloorCount);
            }
            catch (Exception ex)
            {

                Trace.TraceError(ex.Message);
            }
            return count;
        }

    }
}
