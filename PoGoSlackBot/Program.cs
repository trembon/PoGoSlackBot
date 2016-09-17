using NLog;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot
{
    public class Program : ServiceBase
    {
        public const string SERVICE_NAME = "Pokemon GO - Slack Bot";
        private static readonly Logger log = LogManager.GetLogger("Service");

        private static Program service;
        private PokemonGoService pokemonGoService;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // create the new base service
            service = new Program();

            POGOLib.Logging.LoggerConfiguration.MinimumLogLevel = POGOLib.Logging.LogLevel.Error;

            // check mode on the application, cmd or service
            if (Environment.UserInteractive)
            {
                HandleConsoleInput(args);
            }
            else
            {
                // run the service as a windows service
                ServiceBase.Run(service);
            }
        }

        private static void HandleConsoleInput(string[] args)
        {
            bool keepAskingForInput = true;
            while (keepAskingForInput)
            {
                Console.Clear();

                bool isInstalled = false;
                ServiceController windowsService = new ServiceController(SERVICE_NAME);
                try { isInstalled = windowsService.DisplayName == SERVICE_NAME; } catch { }

                Console.WriteLine("Select your choice:");
                Console.WriteLine($"[s] Start '{SERVICE_NAME}' as a console application.");
                Console.WriteLine("");

                if (isInstalled)
                {
                    Console.WriteLine($"[u] Uninstall the '{SERVICE_NAME}' service.");
                    if (windowsService.Status == ServiceControllerStatus.Running)
                    {
                        Console.WriteLine($"[2] Stop the '{SERVICE_NAME}' service.");
                        Console.WriteLine($"[3] Restart the '{SERVICE_NAME}' service.");
                    }
                    else
                    {
                        Console.WriteLine($"[1] Start the '{SERVICE_NAME}' service.");
                    }
                }
                else
                {
                    Console.WriteLine($"[i] Install '{SERVICE_NAME}' as a windows service.");
                }

                Console.WriteLine("");
                Console.WriteLine("[q] Exit.");
                Console.WriteLine();

                Console.Write("Your input: ");
                ConsoleKeyInfo input = Console.ReadKey();

                Console.WriteLine("");
                Console.WriteLine("");

                switch (input.KeyChar)
                {
                    case 's':
                        Console.Clear();
                        keepAskingForInput = false;

                        // start the service as a console application
                        service.OnStart(args);
                        Console.WriteLine("Press any key to stop the program.");
                        Console.Read();
                        service.OnStop();
                        break;

                    case 'q':
                        keepAskingForInput = false;
                        break;

                    case 'i':
                        Console.WriteLine("Installing the service.");

                        var iOriginal = Console.Out;
                        try
                        {
                            Console.SetOut(TextWriter.Null);

                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });

                            Console.SetOut(iOriginal);
                            Console.WriteLine("Installing the service - complete.");
                        }
                        catch
                        {
                            Console.SetOut(iOriginal);
                            Console.WriteLine("An error occured, verify that you are running as an administrator");
                        }
                        break;

                    case 'u':
                        Console.WriteLine("Uninstalling the service.");

                        var uOriginal = Console.Out;
                        try
                        {
                            Console.SetOut(TextWriter.Null);
                            
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });

                            Console.SetOut(uOriginal);
                            Console.WriteLine("Uninstalling the service - complete.");
                        }
                        catch
                        {
                            Console.SetOut(uOriginal);
                            Console.WriteLine("An error occured, verify that you are running as an administrator");
                        }
                        break;

                    case '1':
                        try
                        {
                            Console.WriteLine("Starting the service.");
                            windowsService.Start();
                            windowsService.WaitForStatus(ServiceControllerStatus.Running);
                            Console.WriteLine("The service has now started.");
                        }
                        catch
                        {
                            Console.WriteLine("An error occured, verify that you are running as an administrator");
                        }
                        break;

                    case '2':
                        try
                        {
                            Console.WriteLine("Stopping the service.");
                            windowsService.Stop();
                            windowsService.WaitForStatus(ServiceControllerStatus.Stopped);
                            Console.WriteLine("The service has now stopped.");
                        }
                        catch
                        {
                            Console.WriteLine("An error occured, verify that you are running as an administrator");
                        }
                        break;

                    case '3':
                        try
                        {
                            Console.WriteLine("Stopping the service.");
                            windowsService.Stop();
                            windowsService.WaitForStatus(ServiceControllerStatus.Stopped);
                            Console.WriteLine("Startig the service.");
                            windowsService.Start();
                            windowsService.WaitForStatus(ServiceControllerStatus.Running);
                            Console.WriteLine("The service has now restarted.");
                        }
                        catch
                        {
                            Console.WriteLine("An error occured, verify that you are running as an administrator");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }

                if (keepAskingForInput)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject as Exception, "Uncought exception");
            Environment.Exit(1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        public Program()
        {
            ServiceName = SERVICE_NAME;
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            // log
            log.Info("Service is starting.");

            // clear the old service
            if (pokemonGoService != null)
                pokemonGoService.Stop();

            // create a new service to watch from
            pokemonGoService = new PokemonGoService();

            // start to watch on the configured cameras
            pokemonGoService.Start();

            // log
            log.Info("Service has started.");
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            // log
            log.Info("Service is stopping.");

            // stop and empty the service
            if (pokemonGoService != null)
            {
                pokemonGoService.Stop();
                pokemonGoService = null;
            }

            // log
            log.Info("Service has stopped.");
        }
    }
}
