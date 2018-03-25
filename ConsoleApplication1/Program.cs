using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ConsoleApplication1
{
   class Program
   {
      // запрос API на сайт банка
      // получение ответа
      // - распарсить ответ, вывести результат 
      static void Main(string[] args)
      {
         WebRequest request = WebRequest.Create("http://www.nbrb.by/API/ExRates/Rates?Periodicity=0");

         WebResponse response = request.GetResponse();

         
         using (Stream stream = response.GetResponseStream())
         {
            using (StreamReader reader = new StreamReader(stream))
            {
               
               string line = "";
               while ((line = reader.ReadLine()) != null)
               {
                  Console.WriteLine(line);
               }
               
            }
         }
         

         response.Close();
         Console.WriteLine("Запрос выполнен");
         Console.Read();


      }
   }
}
