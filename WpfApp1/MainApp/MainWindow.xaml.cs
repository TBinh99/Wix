using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Window = System.Windows.Window;
using Process = System.Diagnostics.Process;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<string> list = new List<string>();
            DTE2 dte = (DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");
            string currentProjectName = GetActiveProjectName(dte);
            if (dte != null && dte.Solution != null)
            {
                Solution solution = dte.Solution;
                foreach (Project project in solution.Projects)
                {
                    if (project.Name != "Miscellaneous Files" && project.Name != "SetupProject1")
                    {
                        list.Add(project.Name);
                    }
                }
            }
            list.Remove(currentProjectName);
            projectListBox.Items.Clear();
            projectListBox.ItemsSource = list;
  
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string command = @"C:\Program Files (x86)\WiX Toolset v3.11\bin\heat.exe";

            if (sender is Button button && button.DataContext is string itemName)
            {
                string arguments = $@"dir ""D:\Binh's Folder\Wix\WpfApp1\{itemName}\bin\Debug"" -dr INSTALLFOLDER -srd -cg MyComponentGroup -dr INSTALLFOLDER -gg -sfrag -var var.{itemName}.TargetDir -out ""D:\Binh's Folder\Wix\WpfApp1\SetupProject1\MyComponentGroup.wxs""";
                Process process = new Process();
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.Start();
                process.WaitForExit();

            // Path to the WiX toolset installation directory
            string wixToolsPath = @"C:\Program Files (x86)\WiX Toolset v3.11\bin\";

            // Path to the WiX source files (e.g., Product.wxs)
            string wixSourcePath = @"D:\Binh's Folder\Wix\WpfApp1\SetupProject1\";

            // Path to the output directory for the MSI file
            string outputDirectory = @"D:\Binh's Folder\Wix\WpfApp1\SetupProject1\bin\Debug\";

            // Build the WiX project using candle.exe and light.exe
            ProcessStartInfo candleStartInfo = new ProcessStartInfo
            {
                FileName = wixToolsPath + "candle.exe",
                Arguments = "\"" + wixSourcePath + "Product.wxs\" -dProductName=\"" + itemName + "\" -out \"" + outputDirectory + "Product.wixobj\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(candleStartInfo).WaitForExit();

            ProcessStartInfo lightStartInfo = new ProcessStartInfo
            {
                FileName = wixToolsPath + "light.exe",
                Arguments = "\"" + outputDirectory + "Product.wixobj\" -dProductName=\"" + itemName + "\" -out \"" + outputDirectory +"SetupProject1.msi\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(lightStartInfo).WaitForExit();

            // Install the MSI using msiexec.exe
            ProcessStartInfo msiexecStartInfo = new ProcessStartInfo
            {
                FileName = "msiexec.exe",
                Arguments = "/i \"" + outputDirectory + "SetupProject1.msi\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(msiexecStartInfo).WaitForExit();

            Console.WriteLine("Installation completed.");
            }
        }

        private static string GetActiveProjectName(DTE2 dte)
        {
            if (dte.ActiveSolutionProjects is Array activeProjects && activeProjects.Length > 0)
            {
                Project activeProject = activeProjects.GetValue(0) as Project;
                return activeProject.Name;
            }
            return null;
        }
    }
}
