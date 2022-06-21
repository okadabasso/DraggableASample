using Prism.Ioc;
using DraggableApp.Views;
using System.Windows;
using System.Linq;
using System.Windows.Controls;

namespace DraggableApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var views = this.GetType().Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(UserControl)) && t.Namespace.StartsWith("DraggableApp.Views"))
                ;
            foreach(var view in views)
            {
                containerRegistry.RegisterForNavigation(view, view.FullName);
            }
        }
    }
}
