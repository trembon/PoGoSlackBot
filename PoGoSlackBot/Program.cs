using NLog;
using System;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace PoGoSlackBot
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetLogger("Program");

        private static Service service;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // create the new base service
            service = new Service();

            // check mode on the application
            if (Environment.UserInteractive)
            {
                if (args != null && args.Length >= 1 && args[0] == "-console")
                {
                    // run the service as a console app, if -console argument is specified
                    RunAsConsole(args.Skip(1).ToArray());
                }
                else
                {
                    // handle input of the user
                    HandleConsoleInput(args);
                }
            }
            else
            {
                // run the service as a windows service
                ServiceBase.Run(service);
            }
        }

        private static void HandleConsoleInput(string[] args)
        {
            while (true)
            {
                bool isInstalled = false;
                ServiceController windowsService = new ServiceController(Service.SERVICE_NAME);
                try { isInstalled = windowsService.DisplayName == Service.SERVICE_NAME; } catch { }

                Console.WriteLine("Select your choice:");
                Console.WriteLine($"[s] Start '{Service.SERVICE_NAME}' as a console application.");
                Console.WriteLine("");

                if (isInstalled)
                {
                    Console.WriteLine($"[u] Uninstall the '{Service.SERVICE_NAME}' service.");
                    if (windowsService.Status == ServiceControllerStatus.Running)
                    {
                        Console.WriteLine($"[2] Stop the '{Service.SERVICE_NAME}' service.");
                        Console.WriteLine($"[3] Restart the '{Service.SERVICE_NAME}' service.");
                    }
                    else
                    {
                        Console.WriteLine($"[1] Start the '{Service.SERVICE_NAME}' service.");
                    }
                }
                else
                {
                    Console.WriteLine($"[i] Install '{Service.SERVICE_NAME}' as a windows service.");
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
                        RunAsConsole(args);
                        break;

                    case 'q':
                        Environment.Exit(0);
                        break;

                    case 'i':
                        Console.WriteLine("Installing the service.");

                        HideOutput(() =>
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        },
                        () => Console.WriteLine("Installing the service - complete."),
                        () => Console.WriteLine("An error occured, verify that you are running as an administrator"));
                        break;

                    case 'u':
                        Console.WriteLine("Uninstalling the service.");

                        HideOutput(() =>
                        {
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        },
                        () => Console.WriteLine("Uninstalling the service - complete."),
                        () => Console.WriteLine("An error occured, verify that you are running as an administrator"));
                        break;

                    case '1':
                        StartService(windowsService);
                        break;

                    case '2':
                        StopService(windowsService);
                        break;

                    case '3':
                        StopService(windowsService);
                        StartService(windowsService);
                        break;

                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }

                Console.WriteLine("");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void HideOutput(Action action, Action onSuccess = null, Action onError = null)
        {
            var originalOutput = Console.Out;
            try
            {
                Console.SetOut(TextWriter.Null);
                action();
                Console.SetOut(originalOutput);

                onSuccess?.Invoke();
            }
            catch
            {
                Console.SetOut(originalOutput);
                onError?.Invoke();
            }
        }

        private static void StartService(ServiceController service)
        {
            try
            {
                Console.WriteLine("Starting the service.");

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);

                Console.WriteLine("The service has now started.");
            }
            catch
            {
                Console.WriteLine("An error occured, verify that you are running as an administrator");
            }
        }

        private static void StopService(ServiceController service)
        {
            try
            {
                Console.WriteLine("Stopping the service.");

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);

                Console.WriteLine("The service has now stopped.");
            }
            catch
            {
                Console.WriteLine("An error occured, verify that you are running as an administrator");
            }
        }

        private static void RunAsConsole(string[] args)
        {
            // clear the console output, just in case
            Console.Clear();

            // start the service as a console application
            service.Start(args);
            Console.WriteLine("Press any key to stop the program.");
            Console.Read();
            service.Stop();

            // exit the application when done
            Environment.Exit(0);
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject as Exception, "Uncought exception.");
        }
    }
}
