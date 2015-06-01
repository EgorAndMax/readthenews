using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp;
using iTextSharp.text.pdf;
using AODL;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;
using AODL.Document.Styles;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using AODL.Document.TextDocuments;

namespace ReadTheNews.Models
{
    public class Report
    {
        //Формирование pdf-файла с краткими описаниями и иллюстрациями новостей за заданный период времени
        public void CreatePdf(string path, DateTime start, DateTime finish, RssNewsContext db, string user_id)
        {
            var doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.Open();
            BaseFont baseFont = BaseFont.CreateFont(@"E:\Projects\read_the_news\ReadTheNews\Content\Fonts\arial.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var favoriteNews = GetFavoriteNews(db, user_id, start, finish);
            
            foreach (var news in favoriteNews)
            {
                //title
                Phrase j = new Phrase(news.Title, new Font(baseFont, 15, Font.NORMAL,
                    new BaseColor(System.Drawing.Color.Black)));
                iTextSharp.text.Paragraph a1 = new iTextSharp.text.Paragraph(j);
                doc.Add(a1);
                //description
                j = new Phrase(news.Description, new Font(baseFont, 11, Font.NORMAL,
                    new BaseColor(System.Drawing.Color.Black)));
                a1 = new iTextSharp.text.Paragraph(j);
                doc.Add(a1);
                //link
                j = new Phrase("Ссылка: " + news.Link, new Font(baseFont, 11, Font.BOLDITALIC,
                    new BaseColor(System.Drawing.Color.Black)));
                a1 = new iTextSharp.text.Paragraph(j);
                doc.Add(a1);
                //date
                j = new Phrase(news.Date.ToString(), new Font(baseFont, 11, Font.NORMAL,
                    new BaseColor(System.Drawing.Color.Black)));
                a1 = new iTextSharp.text.Paragraph(j);
                doc.Add(a1);
                //image
                if (news.ImageSrc != "no")
                {
                    System.Net.WebClient w = new System.Net.WebClient();
                    string name = @"E:\Projects\read_the_news\ReadTheNews\Content\Images\" + news.ImageSrc.Replace(":", "").Replace(@"/", "");
                    try
                    {
                        w.DownloadFile(news.ImageSrc, name);
                        Image image = Image.GetInstance(name);
                        image.Alignment = Element.ALIGN_CENTER;
                        doc.Add(image);
                    }
                    catch (Exception ex){ }
                }
                //border
                j = new Phrase("============================", new Font(baseFont, 8, Font.BOLDITALIC,
                    new BaseColor(System.Drawing.Color.Black)));
                a1 = new iTextSharp.text.Paragraph(j);
                doc.Add(a1);
            }

            if (favoriteNews.Count == 0)
            {
                Phrase j = new Phrase("Новостей за данный период нет", new Font(baseFont, 8, Font.BOLDITALIC,
                   new BaseColor(System.Drawing.Color.Black)));
                iTextSharp.text.Paragraph a1 = new iTextSharp.text.Paragraph(j);
                doc.Add(a1);
            }
       

            doc.Close();
        }


        //Текстовый отчет в writter
        public void CreateCalc(string path, RssNewsContext db, string user_id)
        {
            #region reflection
            Type iServiceManager = Type.GetTypeFromProgID("com.sun.star.ServiceManager", true);
            object oServiceManager = System.Activator.CreateInstance(iServiceManager);
            object oDesktop = InvokeObj(oServiceManager, "createInstance", BindingFlags.InvokeMethod, "com.sun.star.frame.Desktop");
            Object[] arg = new Object[4];
            arg[0] = "private:factory/scalc";
            arg[1] = "_blank";
            arg[2] = 0;
            arg[3] = new Object[] { };
            object oComponent = InvokeObj(oDesktop, "loadComponentFromUrl", BindingFlags.InvokeMethod, arg);
            arg = new Object[0];
            Object oText = InvokeObj(oComponent, "getSheets", BindingFlags.InvokeMethod, arg);
            oText = InvokeObj(oText, "getByName", BindingFlags.InvokeMethod, new Object[1] { "Лист1" });


            List<CountNewsOfCategory> query = GetCountNewsOfCategory(db, user_id);

            if (query.Count == 0)
                throw new Exception();

            Object oText1, oText2;
            oText1 = InvokeObj(oText, "getCellByPosition", BindingFlags.InvokeMethod, new Object[2] { 0, 0 });
            oText2 = InvokeObj(oText1, "setString", BindingFlags.InvokeMethod, new Object[1] { "Название категории" });
            oText1 = InvokeObj(oText, "getCellByPosition", BindingFlags.InvokeMethod, new Object[2] { 1, 0 });
            oText2 = InvokeObj(oText1, "setString", BindingFlags.InvokeMethod, new Object[1] { "Количество новостей" });
            int i = 1;
            foreach (var item in query)
            {
                oText1 = InvokeObj(oText, "getCellByPosition", BindingFlags.InvokeMethod, new Object[2] { 0, i });
                oText2 = InvokeObj(oText1, "setString", BindingFlags.InvokeMethod, new Object[1] { item.Name });
                oText1 = InvokeObj(oText, "getCellByPosition", BindingFlags.InvokeMethod, new Object[2] { 1, i });
                oText2 = InvokeObj(oText1, "setString", BindingFlags.InvokeMethod, new Object[1] { item.Count });
                i++;
            }

            arg = new Object[2];
            arg[0] = "file:///" + path.Replace(@"\", "/");
            arg[1] = new Object[0] { };
            InvokeObj(oComponent, "storeToURL", BindingFlags.InvokeMethod, arg);
            InvokeObj(oComponent, "close", BindingFlags.InvokeMethod, new object[1] { "true" });
            #endregion

            #region AODL
            //SpreadsheetDocument spd = new SpreadsheetDocument();
            //spd.New();
            //TextDocument doc = new TextDocument();
            //doc.New();
            //Table table = new Table(spd, "First", "tablefirst");
            //for (int i = 0; i < 10; i++)
            //{
            //    Cell cell = table.CreateCell("cell {0}");
            //    cell.OfficeValueType = "string";
            //    cell.CellStyle.CellProperties.Border = Border.NormalSolid;
            //    AODL.Document.Content.Text.Paragraph p = ParagraphBuilder.CreateSpreadsheetParagraph(spd);
            //    p.TextContent.Add(new SimpleText(spd, i.ToString() + "   Цена: " + i.ToString() + "руб.   Продолжительность: "));
            //    cell.Content.Add(p);
            //    table.InsertCellAt(i + 2, 4, cell);
            //}
            //spd.TableCollection.Add(table);

            //spd.SaveTo(@"E:\Projects\Report.ods");
            #endregion
        }

        private static object InvokeObj(object obj, string method, BindingFlags binding, params object[] par)
        {
            return obj.GetType().InvokeMember(method, binding, null, obj, par);
        }


        //Графический отчет в exel
        public void CreateExcel(string path, RssNewsContext rss_news_context, string user_id)
        {
            Excel.Application xlapp = new Excel.Application();
            xlapp.Visible = false;
            xlapp.DisplayAlerts = true;
            Excel.Workbook xlappwb = xlapp.Workbooks.Add(Type.Missing);
            Excel.Worksheet xlappwsh = (Excel.Worksheet)xlappwb.Worksheets.get_Item(1);
            xlappwsh.Cells[1, 1] = "Название категории";
            xlappwsh.Cells[1, 2] = "Количество новостей";

            List<CountNewsOfCategory> query = GetCountNewsOfCategory(rss_news_context, user_id);

            if (query.Count == 0)
                throw new Exception();

            int i = 2;
            foreach(var item in query)
            {
                xlappwsh.Cells[i, 1] = item.Name;
                xlappwsh.Cells[i, 2] = item.Count;
                i += 1;
            }
            
            xlapp.Cells.ColumnWidth = 10;
            Excel.Range chartRange;
            Excel.ChartObjects xlCharts = (Excel.ChartObjects)xlappwsh.ChartObjects(Type.Missing);
            Excel.ChartObject myChart = (Excel.ChartObject)xlCharts.Add(150, 10, 510, 293);
            Excel.Chart chartPage = myChart.Chart;
            chartRange = xlappwsh.get_Range("A1", "B" + query.Count);
            chartPage.ChartType = Excel.XlChartType.xlColumnStacked;
            //chartPage.Legend.Clear();
            object misValue = Missing.Value;
            chartPage.SetSourceData(chartRange, misValue);
            path = @"E:\Projects\read_the_news\ReadTheNews\Content\Images\graph.bmp";
            chartPage.Export(path, "BMP", misValue);
            
            xlappwb.Saved = true;
            xlappwb.Save();
            xlappwb.SaveCopyAs(@"E:\Projects\read_the_news\ReadTheNews\Content\Reports\Report.xls");
            //           xlappwb.SaveAs(path + "Reports.xls", Excel.XlFileFormat.xlExcel9795, "WWWWW", "WWWWW", Type.Missing, Type.Missing,
            // Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing,
            //Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            //         xlappwb.Close(true, Type.Missing, false);
            xlappwb.Close();
            xlapp.Quit();
        }


        private List<RssItem> GetFavoriteNews(RssNewsContext db, string user_id, DateTime start, DateTime finish)
        {
            var news = (from rss in db.RssItems
                        where rss.Date >= start && rss.Date <= finish
                        select rss).ToList(); ;

            return news;
        }

        private List<CountNewsOfCategory> GetCountNewsOfCategory(RssNewsContext db, string user_id)
        {
            var CountsCategories = (from rc in db.RssCategories.Include("RssItems")
                                        let deletedNews = (from d in db.DeletedNews
                                                           where d.UserId == user_id
                                                           join ri in db.RssItems on d.RssItemId equals ri.Id
                                                           select ri)
                                        select new CountNewsOfCategory
                                        {
                                            Name = rc.Name,
                                            Count = rc.RssItems.Except(deletedNews).Count()
                                        }).OrderByDescending(cnc => cnc.Count).Take(20).ToList();

            return CountsCategories;
        }

    }
}