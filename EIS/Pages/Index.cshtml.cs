using Microsoft.AspNetCore.Mvc.RazorPages;
using FusionCharts.DataEngine;
using FusionCharts.Visualization;
using System.Data;
using EIS.Model;
using Microsoft.AspNetCore.Mvc;
using Aspose.Cells;
using Telerik.SvgIcons;

namespace EIS.Pages
{
    public class KTRUPositionSearchModel : PageModel
    {
        public string ChartPriceJson { get; internal set; }
        public string ChartCountJson { get; internal set; } 
        public string Message { get; private set; } = "";
        public bool IsPost { get; private set; } = false;
        public static bool GroupedBM { get; private set; } = false;
        public static bool Filtred { get; private set; } = false;
        public static string KTRUPosition { get; private set; }
        public static double SumCount { get; private set; }
        public static List<Product> productsFiltred { get; private set; }

        [BindProperty]
        public DateTime dateStart { get; set; }
        [BindProperty]
        public DateTime dateEnd { get; set; }
        public static List<Product> productsSerched { get; private set; }

        public void BarChartDatePrice(List<Product> products)
        {
            DataTable ChartData = new DataTable();
            ChartData.Columns.Add("Цена за единицу измерения, ₽" + products[1].Currency, typeof(double));
            ChartData.Columns.Add("Дата контракта", typeof(string));
            
            foreach (Product product in products)
            {
                ChartData.Rows.Add(product.Price.ToString(), product.Date.ToString().Replace(" 0:00:00", ""));
            }
            
            StaticSource source = new StaticSource(ChartData);
            DataModel model = new DataModel();
            model.DataSources.Add(source);
            Charts.LineChart column = new Charts.LineChart("second_chart");
            column.Events.AttachGenericEvents(FusionChartsEvents.GenericEvents.DATAPLOTCLICK,"wrt");
            column.Width.Pixel(600);
            column.Height.Pixel(400);
            column.Data.Source = model;
            column.Caption.Text = "Цена на позицию КТРУ № "+KTRUPosition;
            column.SubCaption.Text = products.First().Date.ToString().Replace(" 0:00:00", "")+" - "+products.Last().Date.ToString().Replace(" 0:00:00", "");
            column.Legend.Show = false;
            column.XAxis.Text = "Дата контракта";
            column.YAxis.Text = "Цена за единицу измерения, ₽";
            column.ThemeName = FusionChartsTheme.ThemeName.FUSION;
            
            ChartPriceJson = column.Render();
        }
        public void BarChartDatePriceGropedByMounth(List<Product> products)
        {
            products = GroupByMonths(products);

            DataTable ChartData = new DataTable();
            ChartData.Columns.Add("Цена за единицу измерения, ₽" + products[1].Currency, typeof(double));
            ChartData.Columns.Add("Дата контракта", typeof(string));

            foreach (Product product in products)
            {
                ChartData.Rows.Add(product.Price.ToString(), product.Month);
            }

            StaticSource source = new StaticSource(ChartData);
            DataModel model = new DataModel();
            model.DataSources.Add(source);
            Charts.LineChart column = new Charts.LineChart("second_chart");
            column.Events.AttachGenericEvents(FusionChartsEvents.GenericEvents.DATAPLOTCLICK, "wrt");
            column.Width.Pixel(600);
            column.Height.Pixel(400);
            column.Data.Source = model;
            column.Caption.Text = "Цена на позицию КТРУ № " + KTRUPosition;
            column.SubCaption.Text = products.First().Month + " - " + products.Last().Month;
            column.Legend.Show = false;
            column.XAxis.Text = "Дата контракта";
            column.YAxis.Text = "Цена за единицу измерения, ₽";
            column.ThemeName = FusionChartsTheme.ThemeName.FUSION;

            ChartPriceJson = column.Render();
        }
        public void BarChartDateCount(List<Product> products)
        {

            DataTable ChartData = new DataTable();
            ChartData.Columns.Add("Количество(" + products[1].UoM + ")", typeof(double));
            ChartData.Columns.Add("Дата контракта", typeof(string));

            foreach (Product product in products)
            {
                ChartData.Rows.Add(product.Count.ToString(), product.Date.ToString().Replace(" 0:00:00", ""));
            }

            StaticSource source = new StaticSource(ChartData);
            DataModel model = new DataModel();
            model.DataSources.Add(source);
            Charts.ColumnChart column = new Charts.ColumnChart("first_chart");

            column.Width.Pixel(700);
            column.Height.Pixel(400);
            column.Data.Source = model;
            column.Caption.Text = "Количество купленных едениц позиции КТРУ № " + KTRUPosition;
            column.SubCaption.Text = products.First().Date.ToString().Replace(" 0:00:00", "") + " - " + products.Last().Date.ToString().Replace(" 0:00:00", "");
            column.Legend.Show = false;
            column.XAxis.Text = "Дата контракта";
            column.YAxis.Text = "Количество(" + products[1].UoM + ")";
            column.ThemeName = FusionChartsTheme.ThemeName.FUSION;


            ChartCountJson = column.Render();
        }
        public void BarChartDateCountGropedByMounth(List<Product> products)
        {

            products = GroupByMonths(products);

            DataTable ChartData = new DataTable();
            ChartData.Columns.Add("Количество(" + products[1].UoM + ")", typeof(double));
            ChartData.Columns.Add("Дата контракта", typeof(string));

            foreach (Product product in products)
            {
                ChartData.Rows.Add(product.Count.ToString(), product.Month);
            }

            StaticSource source = new StaticSource(ChartData);
            DataModel model = new DataModel();
            model.DataSources.Add(source);
            Charts.ColumnChart column = new Charts.ColumnChart("first_chart");

            column.Width.Pixel(700);
            column.Height.Pixel(400);
            column.Data.Source = model;
            column.Caption.Text = "Количество купленных едениц позиции КТРУ № " + KTRUPosition;
            column.SubCaption.Text = products.First().Month + " - " + products.Last().Month;
            column.Legend.Show = false;
            column.XAxis.Text = "Дата контракта";
            column.YAxis.Text = "Количество(" + products[1].UoM + ")";
            column.ThemeName = FusionChartsTheme.ThemeName.FUSION;


            ChartCountJson = column.Render();
        }
        public void OnGet()
        {
            KTRUPosition = "";
            Message = "Введите код позиции КТРУ";
        }
        public void OnPostSearch(string _KTRUPosition)
        {
            if (_KTRUPosition == null || _KTRUPosition.Length == 0)
            {
                Message = $"Введите корректный код позиции КТРУ";
            }
            else
            {
                KTRUPosition = _KTRUPosition.Trim();
                Message = $"Код позиции КТРУ: {KTRUPosition}";

                productsSerched = Product.Parsed(KTRUPosition);
                if (productsSerched != null && productsSerched.Count > 0)
                {
                    productsFiltred = productsSerched;

                    dateStart = (DateTime)productsFiltred.First().Date;
                    dateEnd = (DateTime)productsFiltred.Last().Date;
                    BarChartDatePrice(productsFiltred);
                    BarChartDateCount(productsFiltred);
                    IsPost = true;
                    foreach (Product product in productsFiltred)
                    {
                        SumCount += product.Count;
                    }
                    IsPost = true;
                }
                else
                {
                    Message = $"Позиция КТРУ не найдена. Введите корректный код позиции КТРУ";
                }
                
            }

        }
        public void MakeReport(List<Product> products,string KTRU)
        {
            Workbook wb = new Workbook();

            Worksheet sheet = wb.Worksheets[0];



            sheet.Cells[0, 0].Value = "Наименование товара";
            sheet.Cells[0, 0 + 1].Value = "Номер контракта";
            sheet.Cells[0, 0 + 2].Value = "Цена";
            sheet.Cells[0, 0 + 3].Value = "Количество";
            sheet.Cells[0, 0 + 4].Value = "Единицы измерения";
            sheet.Cells[0, 0 + 5].Value = "Сумма";
            sheet.Cells[0, 0 + 6].Value = "Дата";
            sheet.Cells[0, 0 + 7].Value = "Ссылка на контракт";
            sheet.Cells[0, 0 + 8].Value = "КТРУ";
            int row = 1;
            int column = 0;
            products.Reverse();
            foreach (Product product in products)
            {
                sheet.Cells[row, column].Value = product.Name;
                sheet.Cells[row, column + 1].Value = product.URL.Replace("https://zakupki.gov.ru/epz/contract/contractCard/payment-info-and-target-of-order.html?reestrNumber=", "");
                sheet.Cells[row, column + 2].Value = product.Price;
                sheet.Cells[row, column + 3].Value = product.Count;
                sheet.Cells[row, column + 4].Value = product.UoM;
                sheet.Cells[row, column + 5].Value = product.TotPrice;
                sheet.Cells[row, column + 6].Value = product.Date.ToString();
                sheet.Cells[row, column + 7].Value = product.URL;
                sheet.Cells[row, column + 8].Value = KTRU;
                row++;
            }
            products.Reverse();
            Console.WriteLine("Reported");
            wb.Save("C:\\Users\\Егор\\source\\repos\\EIS\\EIS\\wwwroot\\Data\\"+KTRU +".xlsx", SaveFormat.Xlsx);
            

        }
        public void OnPostGroupByMounth()
        {
            Message = $"Код позиции КТРУ: {KTRUPosition}";
            BarChartDatePriceGropedByMounth(productsFiltred);
            BarChartDateCountGropedByMounth(productsFiltred);
            IsPost = true;
            GroupedBM = true;
        }
        public void OnPostUnGroupByMounth()
        {
            Message = $"Код позиции КТРУ: {KTRUPosition}";
            BarChartDatePrice(productsFiltred);
            BarChartDateCount(productsFiltred);
            IsPost = true;
            GroupedBM = false;
        }
        public void OnPostFiltrByTime()
        {
            Message = $"Код позиции КТРУ: {KTRUPosition}";
            productsFiltred = productsSerched.Where(p => p.Date >= dateStart).Where(p => p.Date <= dateEnd).ToList();
            Filtred = true;
            if (GroupedBM)
            {
                BarChartDatePriceGropedByMounth(productsFiltred);
                BarChartDateCountGropedByMounth(productsFiltred);
            }
            else
            {
                BarChartDatePrice(productsFiltred);
                BarChartDateCount(productsFiltred);
            }
            IsPost = true;

        }
        public void OnPostUnFilter()
        {
            Message = $"Код позиции КТРУ: {KTRUPosition}";
            productsFiltred = productsSerched;
            dateStart = (DateTime)productsFiltred.First().Date;
            dateEnd = (DateTime)productsFiltred.Last().Date;

            Filtred = false;
            if (GroupedBM)
            {
                BarChartDatePriceGropedByMounth(productsFiltred);
                BarChartDateCountGropedByMounth(productsFiltred);
            }
            else
            {
                BarChartDatePrice(productsFiltred);
                BarChartDateCount(productsFiltred);
            }
            IsPost = true;

        }
        public IActionResult OnPostReport()
        {
            Console.WriteLine("Preparing report");
            MakeReport(productsFiltred, KTRUPosition);
            return File("Data\\" + KTRUPosition + ".xlsx", "text/plain", KTRUPosition + ".xlsx");
        }
        public List<Product> GroupByMonths(List<Product> products)
        {
            List<Product> result = new List<Product>();
            int i = -1;
            int k= 0;
            double sum = 0;
            foreach (Product product in products)
            {
                product.Month = product.Date.ToString().Replace(" 0:00:00", "").Remove(0,3);
                Console.WriteLine(product.Month);
                if (i!=-1)
                {
                    if (result[i].Month != product.Month)
                    {
                        
                        result.Add(product);
                        sum = 0;
                        i++;
                    }
                    else
                    {
                        result[i].Count += product.Count;
                        sum+= product.TotPrice;
                        product.Price = sum / result[i].Count;
                    }
                }
                else
                {
                    result.Add(product);
                    k++;
                    i++;
                }
            }
            return result;
        }
    }
}
