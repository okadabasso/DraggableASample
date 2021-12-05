using Prism.Regions;
using System.Windows;

namespace DraggableApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(LineDragView));
        }
    }
}
