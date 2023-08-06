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
using EnvDTE;
using EnvDTE80;
using Window = System.Windows.Window;

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
