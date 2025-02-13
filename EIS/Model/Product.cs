using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace EIS.Model
{
    public class Product
    {
        public string Name { get; set; } 
        public string KTRU { get; set; } 
        public double Price { get; set; } 
        public double TotPrice { get; set; } 
        public string Currency { get; set; } 
        public double Count { get; set; } 
        public string UoM { get; set; }
        public string Month { get; set; }
        public DateTime? Date { get; set; } = null;
        public string URL { get; set; }
        public Product() { }

        public string GetStr()
        {
            return
                "Контракт: №" + this.URL.Replace("https://zakupki.gov.ru/epz/contract/contractCard/payment-info-and-target-of-order.html?reestrNumber=", "") +
                "КТРУ: " + this.KTRU + "\n" +
                "Количество: " + this.Count + " " + this.UoM + "\n" +
                "Цена за еденицу измерения: " + this.Price + " " + this.Currency + "\n" +
                "Сумма: " + this.TotPrice + "\n" +
                "Дата размещения заказа: " + Date + "\n" +
                "URL: " + URL + "\n";
        }
        public static List<Product> ParseCategory(string categoryKTRU)
        {
            List<Product> category = new List<Product>();

            string targURL = "https://zakupki.gov.ru/epz/ktru/search/results.html?searchString=" + categoryKTRU + "&morphology=on&search-filter=Дате+размещения&active=on&ktruCharselectedTemplateItem=0&sortBy=ITEM_CODE&pageNumber=1&sortDirection=true&recordsPerPage=_100&showLotsInfoHidden=false&rubricatorIdSelected=369";
            Regex regexKTRU = new Regex(@"\d\d\.\d\d\.\d\d\.\d\d\d\-\d\d\d\d\d\d\d\d");

            HtmlWeb web = new HtmlWeb();
            Console.WriteLine("Категория: " + categoryKTRU);

            HtmlDocument doc = web.Load(targURL);
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='registry-entry__body-caption float-right']");

            if (nodes == null) 
            {
                return category; 
            }
            targURL = "https://zakupki.gov.ru" + nodes[1].ChildNodes[1].OuterHtml.ToString().Trim().Replace("<a href=\"", "").Replace("\">Перейти к позициям<img class=\"ml-3\" src=\"/epz/static/img/icons/arrow-prime.svg\" alt=\"\"></a>", "");
            
            doc = web.Load(targURL);
            nodes = doc.DocumentNode.SelectNodes("//a[@target='_blank']");
            try
            {
                if (nodes == null)
                {
                    return category;
                }
                foreach (var node in nodes)
                    {

                        if (regexKTRU.Match(node.InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value != null && regexKTRU.Match(node.InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value.Length > 0)
                        {

                            category.AddRange(Parsed(regexKTRU.Match(node.InnerText).Value.ToString()));
                            //Console.WriteLine(regexKTRU.Match(node.InnerText).Value.ToString());
                        }
                    }
            }
            catch
            {
                Console.WriteLine("Возникло исключение ParseCategory! ");
            }
            return category;
        }
        public static List<string> CategoryKTRUList(string categoryKTRU)
        {
            List<string> category = new List<string>();

            string targURL = "https://zakupki.gov.ru/epz/ktru/search/results.html?searchString=" + categoryKTRU + "&morphology=on&search-filter=Дате+размещения&active=on&ktruCharselectedTemplateItem=0&sortBy=ITEM_CODE&pageNumber=1&sortDirection=true&recordsPerPage=_100&showLotsInfoHidden=false&rubricatorIdSelected=369";
            Regex regexKTRU = new Regex(@"\d\d\.\d\d\.\d\d\.\d\d\d\-\d\d\d\d\d\d\d\d");

            HtmlWeb web = new HtmlWeb();
            //Console.WriteLine("Категория: " + categoryKTRU);

            HtmlDocument doc = web.Load(targURL);
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='registry-entry__body-caption float-right']");
            //var nodes = doc.DocumentNode.SelectNodes("//a[@target='_blank']");
            if (nodes == null)
            {
                return category;
            }
            targURL = "https://zakupki.gov.ru" + nodes[1].ChildNodes[1].OuterHtml.ToString().Trim().Replace("<a href=\"", "").Replace("\">Перейти к позициям<img class=\"ml-3\" src=\"/epz/static/img/icons/arrow-prime.svg\" alt=\"\"></a>", "");
            doc = web.Load(targURL);
            nodes = doc.DocumentNode.SelectNodes("//a[@target='_blank']");
            try
            {
                foreach (var node in nodes)
                {

                    if (regexKTRU.Match(node.InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value != null && regexKTRU.Match(node.InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value.Length > 0)
                    {

                        category.Add(regexKTRU.Match(node.InnerText).Value.ToString());
                    }
                }
            }
            catch
            {
                Console.WriteLine("Возникло исключение ParseCategory! ");
            }
            return category;
        }
        public static List<String> ParsePosition(String targKTRU)
        {
            List<String> ReestNunber = new List<String>();

            HtmlWeb web = new HtmlWeb();

            //Устаревший вариант 
            //String targURL = "https://zakupki.gov.ru/epz/contract/search/results.html?morphology=on&search-filter=Дате+размещения&fz44=on&budgetLevelsIdNameHidden=%7B%7D&ktruCodeNameList=" + targKTRU + "&sortBy=PUBLISH_DATE&pageNumber=1&sortDirection=false&recordsPerPage=_50&showLotsInfoHidden=false";
           
            String targURL = "https://zakupki.gov.ru/epz/contract/search/results.html?fz44=on&sortBy=UPDATE_DATE&ktruCodeNameList=" + targKTRU;

            Console.WriteLine("KTRU: " + targKTRU + "\n" + "Searh here: " + targURL);

            HtmlDocument doc = web.Load(targURL);
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='registry-entry__header-mid__number']");
            try
            {
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        ReestNunber.Add("https://zakupki.gov.ru/epz/contract/contractCard/payment-info-and-target-of-order.html?reestrNumber=" + node.ChildNodes[1].InnerHtml.Trim().Replace("№ ", ""));
                        Console.WriteLine("https://zakupki.gov.ru/epz/contract/contractCard/payment-info-and-target-of-order.html?reestrNumber=" + node.ChildNodes[1].InnerHtml.Trim().Replace("№ ", ""));
                    }
                }
                else
                {
                    Console.WriteLine("Не найдено КТРУ " + targKTRU);
                }
            }
            catch
            {
                Console.WriteLine("Возникло исключение ParsePosition! ");
            }
            return ReestNunber;
        }
        //tableBlock__row
        public static List<Product> ParseKTRU(String targKTRU, String targUrl)
        {
            Regex regex = new Regex(@"-?\d+(?:\,\d+)?");
            Regex regexKTRU = new Regex(@"\d\d\.\d\d\.\d\d\.\d\d\d\-\d\d\d\d\d\d\d\d");

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(targUrl);

            var time = doc.DocumentNode.SelectNodes("//span[@class='cardMainInfo__content']");
            var nodes = doc.DocumentNode.SelectNodes("//tbody");

            List<Product> products = new List<Product>();
            bool i = false;
            if (nodes != null)
            {
                try
                {
                    if (nodes.Count == 0)
                    {
                        Console.WriteLine("Попробуйте позже");
                        return products;
                    }

                    foreach (var node in nodes[1].ChildNodes)
                    {
                        if (node.ChildNodes.Count >= 6)
                        {
                            int j = 0;
                            //foreach (var child in node.ChildNodes)
                            {
                                //Console.WriteLine( j++ + "......................................\n "+ child.InnerHtml+ "\n ......................................");
                            }

                            if (regexKTRU.Match(node.ChildNodes[5].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value == targKTRU)
                            {

                                Product product = new Product();
                                //Название
                                //product.Name = node.ChildNodes[3].ChildNodes[1].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "");
                                product.Name = Regex.Replace(node.ChildNodes[3].ChildNodes[1].InnerText, @"-?\d+.", " ").Trim();
                                Console.WriteLine("Название: " + product.Name);

                                //КТРУ
                                product.KTRU = regexKTRU.Match(node.ChildNodes[5].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value;
                                Console.WriteLine("КТРУ: " + product.KTRU);

                                //Количество
                                String str = regex.Match(node.ChildNodes[9].ChildNodes[1].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value;
                                if (str.Length == 0)
                                {
                                    str = null;
                                }
                                product.Count = 0;
                                try
                                {
                                    product.Count = Convert.ToDouble(str);
                                }
                                catch
                                {
                                }
                                Console.WriteLine("Количество: " + product.Count);

                                //Единица измерения
                                product.UoM = Regex.Replace(node.ChildNodes[9].ChildNodes[1].InnerText, @"-?\d+(?:\,\d+)?", " ").Trim();
                                Console.WriteLine("Единицы измерения: " + product.UoM);

                                //Цена
                                str = regex.Match(node.ChildNodes[11].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value;
                                if (str.Length == 0)
                                {
                                    str = null;
                                }
                                product.Price = Convert.ToDouble(str);
                                Console.WriteLine("Цена: " + product.Price);

                                //Валюта
                                try
                                {
                                    product.Currency = Regex.Replace(node.ChildNodes[11].InnerText, @"-?\d+(?:\,\d+)?", " ").Trim();
                                }
                                catch
                                {
                                }
                                if (product.Currency == null || product.Currency == "") product.Currency = "₽";
                                Console.WriteLine("Валюта: " + product.Currency);

                                //Сумма
                                str = regex.Match(node.ChildNodes[13].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value;
                                if (str.Length == 0)
                                {
                                    str = null;
                                }
                                product.TotPrice = product.Price * product.Count;
                                try
                                {
                                    product.TotPrice = Convert.ToDouble(str);
                                }
                                catch
                                {
                                }
                                Console.WriteLine("Сумма: " + product.TotPrice);

                                product.URL = targUrl;
                                Console.WriteLine("Искомый URL: " + product.URL);

                                //Дата
                                product.Date = DateTime.Parse(time[4].InnerText);
                                Console.WriteLine("Дата: " + product.Date);

                                if (product.Count > 0 && product.Price > 0)
                                {
                                    products.Add(product);
                                }
                                break;
                            }
                        }

                    }
                }
                catch
                {
                    Console.WriteLine("Возникло исключение ParseKTRU! ");
                }
            }
            
            return products;
        }

        public static List<Product> ParseKTRU_v0(String targKTRU, String targUrl)
        {
            Regex regex = new Regex(@"-?\d+(?:\,\d+)?");
            Regex regexKTRU = new Regex(@"\d\d\.\d\d\.\d\d\.\d\d\d\-\d\d\d\d\d\d\d\d");

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(targUrl);

            var time = doc.DocumentNode.SelectNodes("//span[@class='cardMainInfo__content']");
            var nodes = doc.DocumentNode.SelectNodes("//tr");


            List<Product> products = new List<Product>();
            bool i = false;
            try
            {
                if (nodes.Count == 0)
                {
                    Console.WriteLine("Попробуйте позже");
                    return products;
                }
                foreach (var node in nodes)
                {

                    if (node.ChildNodes.Count > 7)
                    {
                        if (i)
                        {
                            if (regexKTRU.Match(node.ChildNodes[5].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value == targKTRU)
                            {
                                Product product = new Product();

                                product.KTRU = regexKTRU.Match(node.ChildNodes[5].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value;


                                String str = regex.Match(node.ChildNodes[9].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value;
                                if (str.Length == 0)
                                {
                                    str = null;
                                }
                                product.Count = Convert.ToDouble(str);
                                product.UoM = Regex.Replace(node.ChildNodes[9].InnerText, @"-?\d+(?:\,\d+)?", " ").Trim();


                                str = regex.Match(node.ChildNodes[11].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value;
                                if (str.Length == 0)
                                {
                                    str = null;
                                }
                                product.Price = Convert.ToDouble(str);
                                product.Currency = Regex.Replace(node.ChildNodes[11].InnerText, @"-?\d+(?:\,\d+)?", " ").Trim();



                                str = regex.Match(node.ChildNodes[15].InnerText.Replace(" ", "").Replace("\n", "").Replace(" ", "")).Value;
                                if (str.Length == 0)
                                {
                                    str = null;
                                }
                                product.TotPrice = Convert.ToDouble(str);

                                product.URL = targUrl;
                                product.Date = DateTime.Parse(time[4].InnerText);
                                if (product.Count > 0 && product.Price > 0)
                                {
                                    products.Add(product);
                                }

                            }
                        }
                        else
                        {
                            i = true;
                        }
                    }

                }
            }
            catch
            {
                Console.WriteLine("Возникло исключение ParseKTRU! ");
            }
            Console.WriteLine();
            return products;
        }
        public static double ProductAvrPrice(List<Product> products)
        {
            double totcount = 0;
            double totprice = 0;
            foreach (Product product in products)
            {
                totprice += product.TotPrice;
                totcount += product.Count;
            }
            return Math.Round(totprice / totcount, 2);
        }
        public static List<Product> Parsed(String targKTRU)
        {
            List<Product> products = new List<Product>();


            foreach (String targurl in ParsePosition(targKTRU))
            {

                foreach (Product prod in ParseKTRU(targKTRU, targurl))
                {
                    products.Add(prod);
                }
            }



            foreach (Product prod in products)
            {
                Console.WriteLine(prod.GetStr());
            }
            Console.WriteLine(products.Count);
            Console.WriteLine("Average Price: " + ProductAvrPrice(products));

            products.Reverse();
            return products;
        }


    }
}
