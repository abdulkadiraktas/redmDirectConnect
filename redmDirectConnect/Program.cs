using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace FileSelectorExample
{
    class Program
    {
        private const string redMdKey = @"SOFTWARE\Classes\redm";

        static void RegisterProtocol()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(redMdKey))
                {
                    key.SetValue("", "URL:RedM Connect Protocol");
                    key.SetValue("URL Protocol", "");

                    using (RegistryKey shellKey = key.CreateSubKey("shell"))
                    using (RegistryKey openKey = shellKey.CreateSubKey("open"))
                    using (RegistryKey commandKey = openKey.CreateSubKey("command"))
                    {
                        string applicationLocation = Process.GetCurrentProcess().MainModule.FileName;
                        commandKey.SetValue("", $"\"{applicationLocation}\" \"%1\"");
                    }
                }
                Console.WriteLine("Protocol registered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering protocol: {ex.Message}");
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "setup")
            {
                RegisterProtocol();
                Console.WriteLine("Protocol registered. You can now use redmconnect://id to start RedM.");
                return;
            }
            if (args.Length > 0)
            {
                string url = args[0];
                Uri uri = new Uri(url);
                string id = uri.Host;
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string redmLocation = Path.Combine(appDataPath, "RedM\\RedM.exe");
                if (File.Exists(redmLocation))
                {
                    Process.Start(redmLocation, $"fivem://connect/{id}");
                }
            }
            else
            {
                Console.WriteLine("No URL provided");
            }
        }
    }
}
