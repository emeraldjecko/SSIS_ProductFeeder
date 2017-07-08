using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Data.SqlClient;
using System.Globalization;
using ProductsFeeder.Models;
using System.IO;
using System.Net;
using System.Configuration;
namespace ProductsFeeder
{
    static class Program
    {
        static void Main(string[] args)
        {
            var CSVData = GetProductsFromRemote();
            var CSVTeapplixData = GetProductsFromTeapplixRemote();
           // string csvFilePath = ConfigurationManager.AppSettings["CSVFile"];
            var products = LoadProductsFromFile(CSVData);
            var product2 =  LoadProductsFromTeapplixFile(CSVTeapplixData);
            //var l = from p in product2
            //        join ps in products on p.SKU equals ps.RealSKU
            //        select p;
             products = MergeTwoLists(products, product2);
            SaveProducts(products);
        }
        private static List<Product>  MergeTwoLists(List<Product> products, List<Product> product2) {
            foreach (var p in products)
            {
                var product = product2.FirstOrDefault(fb => p.RealSKU.Contains(fb.SKU));
              if (product != null) 
              {
                  p.Cost= product.Cost;
              }
            }
               return products;
        }
        private static List<Product> LoadProductsFromFile(string CSVData)
        {
           // DataTable csvData = new DataTable();
            List<Product> listProducts = new List<Product>();
           
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(new StringReader(CSVData)))
                {
                   
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    csvReader.Delimiters = new string[] { "," };
                    //Read column names 
                    string[] colFields = csvReader.ReadFields();
                    //foreach (string column in colFields)
                    //{
                    //    //add each column to datatable
                    //    DataColumn datacolumn = new DataColumn(column);
                    //    datacolumn.AllowDBNull = true;
                    //    csvData.Columns.Add(datacolumn);
                    //}
                    while (!csvReader.EndOfData)
                    {
                        try
                        {

                            // read datafileds and add rows
                            string[] dataRow = csvReader.ReadFields();
                            var product = new Product();

                            for (int i = 0; i < dataRow.Length; i++)
                            {
                                product.Brand = dataRow[0].ToString();
                                product.SKU = dataRow[1].ToString();
                                product.Name = dataRow[2].ToString();
                                product.Active = dataRow[3].ToString();
                                product.RealSKU = dataRow[4].ToString();
                                product.SellerId = dataRow[5].ToString();
                                product.CostPrice = Convert.ToDouble(dataRow[6]);
                                product.PriceDefault = Convert.ToDouble(dataRow[7]);
                                product.DateCreated = DateTime.UtcNow;
                                product.eBayItemID= dataRow[8].ToString();
                            }

                            listProducts.Add(product);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to read a csv record: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return listProducts;
        }
        private static List<Product> LoadProductsFromTeapplixFile(string CSVteaplixData)
        {
            //DataTable csvData = new DataTable();
            List<Product> listProducts = new List<Product>();

            int colSKU = 0;
            int colCost = 0;
            int counter = 0;

            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(new StringReader(CSVteaplixData)))
                {

                    csvReader.HasFieldsEnclosedInQuotes = true;
                    csvReader.Delimiters = new string[] { "," };
                    //Read column names 
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        ////add each column to datatable
                        //DataColumn datacolumn = new DataColumn(column);
                        //datacolumn.AllowDBNull = true;
                        //csvData.Columns.Add(datacolumn);
                        if(column == "Teapplix SKU")
                        {
                            colSKU = counter;
                        }
                        if (column == "Cost")
                        {
                            colCost = counter;
                        }
                        counter++;
                    }
                    while (!csvReader.EndOfData)
                    {
                        try
                        {

                            // read datafileds and add rows
                            string[] dataRow = csvReader.ReadFields();
                            var product = new Product();

                            //for (int i = 0; i < dataRow.Length; i++)
                            //{
                                
                                //product.SKU = dataRow[2].ToString();

                                //product.Cost = string.IsNullOrEmpty(dataRow[17].ToString())?0:Convert.ToDouble(dataRow[17 ]);

                            product.SKU = dataRow[colSKU].ToString();

                            product.Cost = string.IsNullOrEmpty(dataRow[colCost].ToString()) ? 0 : Convert.ToDouble(dataRow[colCost]);
                            //}

                            listProducts.Add(product); 
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to read a csv record: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return listProducts;
        }

        private static void SaveProducts(List<Product> newProductsList)
        {
            ProductEntities db = new ProductEntities();

            //Remove the exiting records.
            var allProducts = db.Products.ToList();
            if (allProducts.Count > 0)
            {
                db.Products.RemoveRange(allProducts);
                db.SaveChanges();
            }

            db.Products.AddRange(newProductsList);

            db.SaveChanges();
           
      }

        public static string GetProductsFromRemote()
        {
            
            string url = ConfigurationManager.AppSettings["DailyExport"].ToString();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream());
            string csvResults = sr.ReadToEnd();
            sr.Close();

            return csvResults;
        }
        public static string GetProductsFromTeapplixRemote()
        {
            try
            {

                string url = ConfigurationManager.AppSettings["TeapplixRemote"].ToString(); 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream());
                string csvResults = sr.ReadToEnd();
                sr.Close();

                return csvResults;
            }
            catch (Exception e) 
            {
                return string.Empty;
            }
        }
    }
}


