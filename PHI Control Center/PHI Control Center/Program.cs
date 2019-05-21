using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PHI_Control_Center.Classes;
using Newtonsoft.Json;

namespace PHI_Control_Center
{
    static class Program
    {
        public static List<Device> Devices;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoadDevices();
            Configure configure = new Configure();

            Application.Run();
        }

        private static void LoadDevices()
        {
            Devices = new List<Device>();
            foreach (string file in Directory.GetFiles("Devices"))
            {
                if (file.EndsWith(".json"))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        Devices.Add(JsonConvert.DeserializeObject<Device>(sr.ReadToEnd()));
                    }
                }
            }
            Devices.Sort();
        }
    }
}
