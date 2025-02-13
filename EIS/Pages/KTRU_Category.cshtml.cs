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
    public class Pages_KTRUModel : PageModel
    {
        public string ChartPriceJson { get; internal set; }
        public string ChartCountJson { get; internal set; }
        public string PieChartJson { get; internal set; }
        public string Message { get; private set; } = "";
        public bool IsPost { get; private set; } = false;
        public static bool GroupedBM { get; private set; } = false;
        public static bool Filtred { get; private set; } = false;
        public static string KTRUCategory { get; private set; }
        public static double SumCount { get; private set; }
        public static List<string> CategoryList { get; private set; }
        public static List<Product> productsFiltred { get; private set; }
        [BindProperty]
        public static DateTime dateStart { get; set; }
        [BindProperty]
        public static DateTime dateEnd { get; set; }
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
            column.Caption.Text = "Цена на позицию в категории КТРУ № "+KTRUCategory;
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
            column.Caption.Text = "Цена на позицию в категории КТРУ № " + KTRUCategory;
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
            column.Caption.Text = "Количество купленных едениц товара в категориии КТРУ № " + KTRUCategory;
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
            column.Caption.Text = "Количество купленных едениц товара в категории КТРУ № " + KTRUCategory;
            column.SubCaption.Text = products.First().Month + " - " + products.Last().Month;
            column.Legend.Show = false;
            column.XAxis.Text = "Дата контракта";
            column.YAxis.Text = "Количество(" + products[1].UoM + ")";
            column.ThemeName = FusionChartsTheme.ThemeName.FUSION;


            ChartCountJson = column.Render();
        }
        public void PieChart(List<Product> products)
        {
            DataTable ChartData = new DataTable();
            ChartData.Columns.Add("Позиция КТРУ №" + KTRUCategory, typeof(string));
            ChartData.Columns.Add("Количество проданного товара" + KTRUCategory, typeof(double));
            foreach (string ktru in CategoryList)
            {
                double sum = 0;
                foreach (Product product in products)
                {
                    if(product.KTRU == ktru) sum += product.Count;
                    
                }
                ChartData.Rows.Add(ktru, sum);

            }

            StaticSource source = new StaticSource(ChartData);
            DataModel model = new DataModel();


            model.DataSources.Add(source);
            Charts.PieChart column = new Charts.PieChart("pie_chart");


            column.Width.Pixel(600);
            column.Height.Pixel(400);
            column.Caption.Text = "Количество купленных товаров каждой позиции в категории КТРУ №" + KTRUCategory;
            column.SubCaption.Text = products.First().Date.ToString().Replace(" 0:00:00", "") + " - " + products.Last().Date.ToString().Replace(" 0:00:00", "");
            column.Legend.Show = false;

            column.Data.Source = model;
            column.ThemeName = FusionChartsTheme.ThemeName.FUSION;
            PieChartJson = column.Render();
            Console.WriteLine(PieChartJson);

        }
        public void OnGet()
        {
            KTRUCategory = "";
            Message = "Введите код категории КТРУ";
        }
        public void OnPostSearch(string _KTRUPosition)
        {
            if (_KTRUPosition == null || _KTRUPosition.Length == 0)
            {
                Message = $"Введите корректный код категории КТРУ";
            }
            else
            {
                KTRUCategory = _KTRUPosition.Trim();
                Message = $"Код категории КТРУ: {KTRUCategory}";

                productsSerched = Product.ParseCategory(KTRUCategory);
                productsFiltred = productsSerched.OrderBy(a => a.Date).ToList();
                CategoryList = Product.CategoryKTRUList(KTRUCategory);
                if (productsSerched!=null && productsSerched.Count>0)
                {
                    Console.WriteLine("Категория КТРУ найдена");
                    dateStart = (DateTime)productsFiltred.First().Date;
                    dateEnd = (DateTime)productsFiltred.Last().Date;
                    BarChartDatePrice(productsFiltred);
                    BarChartDateCount(productsFiltred);
                    Console.WriteLine("PieChartNext");
                    PieChart(productsFiltred);
                    
                    IsPost = true;
                    foreach (Product product in productsFiltred)
                    {
                        SumCount += product.Count;
                    }
                    SumCount = Math.Round(SumCount, 2);
                }
                else
                {
                    Message = $"Категория КТРУ не найдена. Введите корректный код категории КТРУ";
                }
            }

        }
        public void MakeReport(List<Product> products,string KTRU)
        {
            Workbook wb = new Workbook();

            Worksheet sheet = wb.Worksheets[0];


            sheet.Cells[0, 0].Value = "Наименование товара";
            sheet.Cells[0, 0 + 1].Value = "Цена";
            sheet.Cells[0, 0 + 2].Value = "Количество";
            sheet.Cells[0, 0 + 3].Value = "Единицы измерения";
            sheet.Cells[0, 0 + 4].Value = "Сумма";
            sheet.Cells[0, 0 + 5].Value = "Дата";
            sheet.Cells[0, 0 + 6].Value = "Ссылка на контракт";
            sheet.Cells[0, 0 + 7].Value = "КТРУ";
            sheet.Cells[0, 0 + 8].Value = "Номер контракта";
            int row = 1;
            int column = 0;
            products.Reverse(); 
            foreach (Product product in products)
            {
                sheet.Cells[row, column].Value = product.Name;
                sheet.Cells[row, column + 1].Value = product.Price;
                sheet.Cells[row, column + 2].Value = product.Count;
                sheet.Cells[row, column + 3].Value = product.UoM;
                sheet.Cells[row, column + 4].Value = product.TotPrice;
                sheet.Cells[row, column + 5].Value = product.Date.ToString();
                sheet.Cells[row, column + 6].Value = product.URL;
                sheet.Cells[row, column + 7].Value = KTRU;
                sheet.Cells[row, column + 8].Value = product.URL.Replace("https://zakupki.gov.ru/epz/contract/contractCard/payment-info-and-target-of-order.html?reestrNumber=", "");
                row++;
            }
            products.Reverse();
            Console.WriteLine("Reported");
            wb.Save("C:\\Users\\Егор\\source\\repos\\EIS\\EIS\\wwwroot\\Data\\"+KTRU +".xlsx", SaveFormat.Xlsx);
            

        }
        public void OnPostGroupByMounth()
        {
            Message = $"Код категории КТРУ: {KTRUCategory}";
            BarChartDatePriceGropedByMounth(productsFiltred);
            BarChartDateCountGropedByMounth(productsFiltred);
            PieChart(productsFiltred);
            dateStart = (DateTime)productsFiltred.First().Date;
            dateEnd = (DateTime)productsFiltred.Last().Date;
            IsPost = true;
            GroupedBM = true;
        }
        public void OnPostUnGroupByMounth()
        {
            Message = $"Код категории КТРУ: {KTRUCategory}";
            BarChartDatePrice(productsFiltred);
            BarChartDateCount(productsFiltred);
            PieChart(productsFiltred);
            IsPost = true;
            GroupedBM = false;
            dateStart = (DateTime)productsFiltred.First().Date;
            dateEnd = (DateTime)productsFiltred.Last().Date;
        }
        public void OnPostFiltrByTime()
        {
            Message = $"Код категории КТРУ: {KTRUCategory}";
            productsFiltred = productsSerched.Where(p => p.Date >= dateStart).Where(p => p.Date <= dateEnd).ToList();
            Filtred = true;
            dateStart = (DateTime)productsFiltred.First().Date;
            dateEnd = (DateTime)productsFiltred.Last().Date;
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
            PieChart(productsFiltred);
            IsPost = true;
           
        }
        public void OnPostUnFilter()
        {
            Message = $"Код категории КТРУ: {KTRUCategory}";
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
            PieChart(productsFiltred);  
            IsPost = true;
            dateStart = (DateTime)productsFiltred.First().Date;
            dateEnd = (DateTime)productsFiltred.Last().Date;
        }
        public IActionResult OnPostReport()
        {
            Console.WriteLine("Preparing report");
            MakeReport(productsFiltred, KTRUCategory);
            return File("Data\\" + KTRUCategory + ".xlsx", "text/plain", KTRUCategory + ".xlsx");
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
