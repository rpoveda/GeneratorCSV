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

    public static class CreateCSV
    {
        public static byte[] GetCSVBytes<T>(this IList<T> list, string separator = ";")
        {
            StringBuilder csv = new StringBuilder();
            //Cria o cabecalho do csv
            csv.AppendLine(SetHeaderCSV<T>(separator));
            //Cria o corpo do csv
            csv.AppendLine(SetBodyCSV<T>(list, separator));

            using (var stream = new MemoryStream())
            using (var sw = new StreamWriter(stream, Encoding.UTF8))
            {
                sw.Write(csv.ToString());
                sw.Flush();
                return stream.ToArray();
            }
        }
        public static void SaveCSVPath<T>(this IList<T> list, string path, string fileName, string separator = ";")
        {
            StringBuilder csv = new StringBuilder();
            //Cria o cabecalho do csv
            csv.AppendLine(SetHeaderCSV<T>(separator));
            //Cria o corpo do csv
            csv.AppendLine(SetBodyCSV<T>(list, separator));
            //Verifica se o nome do arquivo contem .csv
            //caso nao contenha é adicionado
            fileName = (fileName.Contains(".csv") ? fileName : string.Format("{0}.csv", fileName));

            //Combina path com filename
            var pathComplete = Path.Combine(path, fileName);

            using (FileStream fs = new FileStream(pathComplete, FileMode.Truncate))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(csv.ToString());
                }
            }
        }

        private static string SetHeaderCSV<T>(string separator)
        {
            var properties = typeof(T).GetProperties();
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
