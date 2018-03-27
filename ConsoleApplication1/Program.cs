using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

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

         string line = null;
         
         // чтение результата запроса в строку
         using (Stream stream = response.GetResponseStream())
         {

            //using (StreamReader reader = new StreamReader(stream))
            //{
            //line = "";
               StreamReader reader = new StreamReader(stream);
               line = reader.ReadToEnd();
            //Console.WriteLine(line);
            //Console.WriteLine(line.Length + "*********!!!!!!!!!!!!!!!********");
            /*
               while ((line = reader.ReadLine()) != null)
               {
                  Console.WriteLine(line);
                  Console.WriteLine(line.Length + "*********!!!!!!!!!!!!!!!********");
               }
               */
            //}
            // здесь строки уже нет
         }

         
         response.Close();
         Console.WriteLine("Запрос выполнен");
         Console.Read();

         // парс ответа
         List<string> arrObj = new List<string>();

         for (int i = 0; i < line.Length; i++)
         {
            if (line[i] == '{')
            {
               string strTmp = null;
               i++;
               for (int j = 0; line[i] != '}'; i++, j++)
               {
                  strTmp += line[i].ToString();
               }
               arrObj.Add(strTmp);
            }
         }

         /*
         int c = 1;
         foreach (var l in arrObj)
         {
            Console.WriteLine(c.ToString() + l);
            c++;
         }
         */

         // формирование коллекции объектов
         
         TypeMoney tm;
         List<TypeMoney> listTm = new List<TypeMoney>();

         for (int i = 0; i < arrObj.Count; i++)
         {
            // создаем объект деньги
            tm = new TypeMoney();

            // извлекаем дату
            string patternDate = @"\d{4}-\d{2}-\d{2}";
            Regex reg = new Regex(patternDate);
            Match parseDate = reg.Match(arrObj[i]);

            tm.Date = parseDate.ToString();

            // извлекаем абревеатуру валюты
            string patternCurAbr = @"([A-Z]){3}";
            Regex reg2 = new Regex(patternCurAbr);
            Match parseCurAbr = reg2.Match(arrObj[i]);

            tm.CurAbr = parseCurAbr.ToString();


            listTm.Add(tm); // добавляю
         }


         foreach (TypeMoney t in listTm)
         {
            Console.WriteLine(t.Date + " " + t.CurAbr);
         }


      }
   }
}
