using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHO
{
    class helpers
    {
        public string StockName { get; set; }
        public decimal Price { get; set; }
        public decimal TODAYPrice { get; set; }
        public string Series { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Qnt { get; set; }
        public decimal Pivot { get; set; }
        public decimal Preclose { get; set; }
    }
    class PreHelpers
    {
        public string PreStockName { get; set; }
        public decimal Price { get; set; }
    }

    class Program
    {

        public static List<helpers> Listing = new List<helpers>();
        public static List<helpers> PreListing = new List<helpers>();
        public static List<helpers> result = new List<helpers>();
        public static bool LeaveFirstA = false;
        public static bool LeaveFirstB = false;

        static void Main(string[] args)
        {

          
            string csvData = File.ReadAllText(@"C:\Pivot\bhav.csv");

            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {

                    if (LeaveFirstA == true)
                    {
                        var split = row.Split(',');
                        var Open = Convert.ToDecimal(split[2].Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                        var High = Convert.ToDecimal(split[3].Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                        var Low = Convert.ToDecimal(split[4].Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                        var Close = Convert.ToDecimal(split[5].Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                        var Qnt = Convert.ToInt32(split[8].Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));

                        if (split[1] == "EQ")
                        {
                            Listing.Add(new helpers
                            {
                                StockName = split[0],
                                Series = split[1],
                                Open = Open,
                                High = High,
                                Low = Low,
                                Close = Close,
                                Qnt = Qnt,
                                Pivot = Convert.ToDecimal(High + Low + Close) / 3,
                            });
                        }
                    }
                    LeaveFirstA = true;
                }
            }

            result = Listing.Where(p => p.Close > 150 && p.Close < 350 && p.Qnt > 1000000).OrderBy(o => o.Close).ToList();
            GetPre();

        

            //Today();
        }

        public static void Today()
        {
            string csvData = File.ReadAllText(@"D:\Desktop\PHO\TODAY.csv");

            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {

                    if (LeaveFirstB == true)
                    {


                        var split = row.Split(',');

                        helpers result = Listing.Find(x => x.StockName == split[0]);
                        result.TODAYPrice = Convert.ToDecimal(split[1].Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                    }
                    LeaveFirstB = true;
                }
            }

            ShowAll();

        }

        public static void ShowAll()

        {


            foreach (var List in Listing.OrderBy(o => o.TODAYPrice))
            {
                if (List.TODAYPrice != 0)
                {
                    if (List.Price > List.TODAYPrice)
                    {

                        var A1 = 15 - List.StockName.Count();
                        var B1 = List.StockName.Count();
                        var StockName = List.StockName.PadRight(A1 + B1);


                        var Price = Convert.ToString(List.Price);
                        var A2 = 10 - Price.Count();
                        var B2 = Price.Count();
                        var StockPrice = Price.PadRight(A2 + B2);
                        var c = StockPrice.Count();





                        Console.WriteLine(StockName + " - " + StockPrice + " - " + List.TODAYPrice);
                    }
                }
            }
            Console.ReadLine();

        }

        public static void GetPre()
        {
            using (TextReader tr = File.OpenText(@"C:\Pivot\Pre.txt"))
            {
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    string[] items = line.Split('\t');

                    var StockName = items[0];

                    var YES = result.Where(p => p.StockName == StockName).FirstOrDefault();
                    if (YES != null)
                    {
                        var Price = Convert.ToDecimal(items[3].Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                        var preclose= Convert.ToDecimal(items[6].Replace("\r\n", "").Replace("\r", "").Replace("\n", ""));
                        YES.Price = Price;
                        YES.Preclose = preclose;
                    }
                }

                //if PRE CLOSE is > than pivot the go long
                //if PRE CLOSE is < than pivot the go short

                var LONG = 0;
                var SHORT= 0;

                foreach (var res in result)
                {
                    if (res.Preclose > res.Pivot)
                    {
                        var difference = (res.Price - res.Pivot);
                        if (difference < 0)
                        {
                            Console.WriteLine("LONG  " + difference.ToString("0.##") + "---" + res.StockName);
                           
                        }
                        LONG++;
                    }
                  
                }
                Console.WriteLine(LONG);

                foreach (var res in result)
                {
                    if (res.Preclose < res.Pivot)
                    {
                        var difference = (res.Price - res.Pivot);
                        if (difference > 0)
                        {
                            Console.WriteLine("SHORT  " + difference.ToString("0.##") + "---" + res.StockName);
                          
                        }
                        SHORT++;
                    }

                }
                Console.WriteLine(SHORT);



                //foreach (var res in result)
                //{
                //    if (res.Price > res.Pivot)
                //    {
                //        var difference = (res.Price - res.Pivot);
                //        var difference1 = difference.ToString("0.##");
                //        var percent =( (difference / res.Price) * 100).ToString("0.##");
                //        var pre = res.Preclose;


                //        Console.WriteLine(percent+"----"+ difference1 +"-"+res.StockName + "-" + res.Pivot.ToString("0.##") + "-" + res.Price.ToString("0.##")+"----"+ pre.ToString("0.##"));
                //    }
                //}
                //Console.WriteLine("==============================================================");

                //foreach (var res in result)
                //{
                //    if (res.Price < res.Pivot)
                //    {
                //        var difference = (res.Pivot - res.Price);
                //        var difference1 = difference.ToString("0.##");
                //        var percent = ((difference / res.Price) * 100).ToString("0.##");
                //        var pre = res.Preclose;

                //        Console.WriteLine(percent + "-" + difference1 +"-"+res.StockName + "-" + res.Pivot.ToString("0.##") + "-" + res.Price.ToString("0.##") + "----" + pre.ToString("0.##"));
                //    }
                //}
                Console.ReadLine();

            }
        }
    }
}
