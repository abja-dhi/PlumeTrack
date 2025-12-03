using Plume_Track;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Plume_Track
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var splash = new __SplashPage())
            {
                string appFolder = Path.Combine(_Globals.dataPath, "PlumeTrack");
                if (!Directory.Exists(appFolder))
                {
                    Directory.CreateDirectory(appFolder);
                }
                splash.Show();
                splash.Refresh();
                Dictionary<string, string> inputs = new()
                {
                        { "Task", "HelloBackend" },
                        { "Path", Path.Combine(_Globals.dataPath, "PlumeTrack", "load_data_message.html").ToString() },
                    };
                string xmlInput = _Tools.GenerateInput(inputs);
                XmlDocument result = _Tools.CallPython(xmlInput);
                Dictionary<string, string> outputs = _Tools.ParseOutput(result);
                if (outputs.TryGetValue("Error", out string? value))
                {
                    MessageBox.Show("Backend Error: " + value);
                    Application.Exit();
                }
                splash.Close();
            }
            Application.Run(new __PlumeTrack(args));
        }
    }
}