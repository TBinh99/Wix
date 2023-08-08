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
using WixSharp;
using System.IO;
using Window = System.Windows.Window;
using Process = System.Diagnostics.Process;
using Project = EnvDTE.Project;
using Microsoft.Win32;
using System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Button = System.Windows.Controls.Button;
using File = WixSharp.File;
using System.Collections.ObjectModel;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> listBoxItems = new ObservableCollection<string>();
        public MainWindow()
        {
            InitializeComponent();
            //List<string> list = new List<string>();
            //DTE2 dte = (DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");
            //string currentProjectName = GetActiveProjectName(dte);
            //if (dte != null && dte.Solution != null)
            //{
            //    Solution solution = dte.Solution;
            //    foreach (Project project in solution.Projects)
            //    {
            //        if (project.Name != "Miscellaneous Files" && project.Name != "SetupProject1")
            //        {
            //            list.Add(project.Name);
            //        }
            //    }
            //}
            //list.Remove(currentProjectName);
            //projectListBox.Items.Clear();
            //projectListBox.ItemsSource = list;
            projectListBox.ItemsSource = listBoxItems;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is string itemName)
            {
                listBoxItems.Remove(itemName);
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
        static void RunMsi(string msiPath)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "msiexec.exe";
                process.StartInfo.Arguments = "/i \"" + msiPath + "\""; // /i for installation
                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine("Installation successful.");
                }
                else
                {
                    Console.WriteLine("Installation failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void GetFiles(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; // Allow multiple file selection

            DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string[] selectedFilePaths = openFileDialog.FileNames;
                //projectListBox.Items.Clear();
                //projectListBox.ItemsSource = selectedFilePaths;

                foreach (string filePath in selectedFilePaths)
                {
                    listBoxItems.Add(filePath);
                }

            }
        }

        private void Install(object sender, RoutedEventArgs e)
        {
            string itemName = AppName.Text;
            List<File> listBoxFiles = new List<File>();
            foreach (string path in listBoxItems) {
                listBoxFiles.Add(new File(path));
            }
            var project = new WixSharp.Project(itemName,
                      new Dir($@"%ProgramFiles%\{itemName}",
                      listBoxFiles.ToArray()));
                //project.GUID = new Guid("6f330b47-2577-43ad-9095-1861ba25889b");
                Compiler.BuildMsi(project);
                //RunMsi(System.IO.Path.Combine(project.OutDir, project.Name));
        }
    }
}
