using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace GeradorCSV
{
    /// <summary>
    /// var listPeople = new List<People>()
    /// {
    ///     new People{ IdPeople = 1, FirtName = "Rafael", LastName = "Poveda" }
    /// };
    /// var bytes = listPeople.GetCSVBytes();
    /// listPeople.SaveCSVPath(@"c:/temp/","mycsv.csv")
    /// listPeople.SaveCSVPath(@"c:/temp/","mycsv2")
    /// </summary>

    static class CreateCSV
    {
        public static byte[] GetCSVBytes<T>(this IList<T> list, string separator = ";")
        {
            StringBuilder csv = new StringBuilder();

            //Set header csv
            csv.AppendLine(SetHeaderCSV<T>(separator));

            //Set body csv
            csv.AppendLine(SetBodyCSV<T>(list, separator));

            return new System.Text.UTF8Encoding().GetBytes(csv.ToString());
        }

        public static void SaveCSVPath<T>(this IList<T> list, string path, string fileName, string separator = ";")
        {
            StringBuilder csv = new StringBuilder();

            //Set header csv
            csv.AppendLine(SetHeaderCSV<T>(separator));

            //Set body csv
            csv.AppendLine(SetBodyCSV<T>(list, separator));

            //Check if file has extension
            fileName = (fileName.Contains(".csv") ? fileName : string.Format("{0}.csv", fileName));

            //Combine Path and FileName
            var pathComplete = Path.Combine(path, fileName);

            using (StreamWriter sw = new StreamWriter(pathComplete))
            {
                sw.Write(csv.ToString());
            }
        }

        private static string SetHeaderCSV<T>(string separator)
        {
            var objGeneric = Activator.CreateInstance<T>();
            var properties = objGeneric.GetType().GetProperties();
            StringBuilder header = new StringBuilder();

            foreach (PropertyInfo property in properties)
            {
                header.AppendFormat("{0}{1}", property.Name, separator);
            }

            return header.ToString();
        }

        private static string SetBodyCSV<T>(IList<T> list, string separator)
        {
            StringBuilder body = new StringBuilder();

            foreach (T item in list)
            {
                var properties = item.GetType().GetProperties();
                StringBuilder content = new StringBuilder();
                foreach (var property in properties)
                {
                    content.AppendFormat("{0}{1}", property.GetValue(item, null), separator);
                }
                body.AppendLine(content.ToString());
            }

            return body.ToString();
        }
    }
}
