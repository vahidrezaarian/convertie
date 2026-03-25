using System.Windows;

namespace Convertie;

public partial class App : Application
{
    [System.STAThreadAttribute()]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    static void Main(string[] args)
    {
        App app = new App();
        app.MainWindow = new MainWindow();
        app.MainWindow.Activate();
        app.MainWindow.Focus();
        app.MainWindow.Show();
        app.Run();
    }
}