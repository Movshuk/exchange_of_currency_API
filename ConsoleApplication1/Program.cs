using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;

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

      // запрос курса на дату:
      // http://www.nbrb.by/API/ExRates/Rates?onDate=2018-3-29&Periodicity=0

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
            reader.Close();
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


            // извлекаем пропорцию соотношения 1:М
            string patternHowMany = @"Cur_Scale"":\d*";
            Regex reg3 = new Regex(patternHowMany); // здесь паттерн поиска
            Match parseHowMany = reg3.Match(arrObj[i]); // выбранная подстрока
            string newStr = parseHowMany.ToString();
            //Console.WriteLine(">> " + newStr); // отработала


            //string newStr2 = @"Cur_Scale"":100";
            string patternHowMany2 = @"[0-9]{1,}";
            Regex reg4 = new Regex(patternHowMany2);
            Match parseHowMany2 = reg4.Match(parseHowMany.ToString());
            tm.HowMany = Convert.ToInt32(parseHowMany2.ToString());

            /*
            if (reg4.IsMatch(newStr))
               Console.WriteLine("В исходной строке: \"{0}\" есть совпадения [[{1}]]!", newStr, (Convert.ToInt32(reg4.Match(newStr).ToString())).ToString());
            else
               Console.WriteLine("ПОЧЕМУ ТО НЕ НАШЕЛ");
            */

            // извлекаем имя валюты
            string patternCuName = @"([а-я]+\W\([а-я]+\W[а-я]+\W[а-я]+\))|([а-я]+\W[а-я]+)|([а-я]+)";
            Regex reg5 = new Regex(patternCuName, RegexOptions.IgnoreCase);
            Match parseCurName = reg5.Match(arrObj[i]);

            tm.CurName = parseCurName.ToString();

            // извлечение актуального курса
            string patternCurRate = @"(\d+\.{1}\d+)";
            Regex reg6 = new Regex(patternCurRate);
            Match parseCurRate = reg6.Match(arrObj[i]);
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            tm.CurRate = double.Parse(parseCurRate.ToString());


            listTm.Add(tm); // добавляю
         }

         Console.WriteLine("Официальные курсы белорусского рубля по отношению к иностранным валютам, на сегодня ");

         // вывод на экран
         foreach (TypeMoney t in listTm)
         {
            Console.WriteLine(t.Date + " " + t.CurAbr + " " + t.HowMany + " " + t.CurName + " " + t.CurRate);
         }

         Console.WriteLine("Press Enter....");
         Console.ReadKey();
      }
   }
}
