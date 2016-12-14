using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace BotConsultant
{
    public static class ResponsesParser
    {
        private static readonly Random Rnd = new Random();
        
        public static string GetResponse(string intent)
        {
            string res = "Что-то мне не хорошо, дайте бревно погрызть...";

            var assembly = Assembly.GetExecutingAssembly();
            var fileStream = assembly.GetManifestResourceStream("BotConsultant.responses.json");
            if (fileStream != null)
            {
                JObject responses = JObject.Parse(new StreamReader(fileStream).ReadToEnd());

                JToken intentArray;
                if (!responses.TryGetValue(intent, StringComparison.InvariantCultureIgnoreCase, out intentArray))
                {
                    responses.TryGetValue("DoNotUnderstand", StringComparison.InvariantCultureIgnoreCase, out intentArray);
                }

                if (intentArray != null)
                {
                    res = intentArray.ElementAt(Rnd.Next(intentArray.Count())).ToString();
                }
            }
            
            return res;
        }
    }
}