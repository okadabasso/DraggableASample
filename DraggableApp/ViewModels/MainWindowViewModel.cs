using DraggableApp.Views;
using Prism.Mvvm;
using Prism.Regions;

namespace DraggableApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Draggable Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        IRegionManager _regionManager;
        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
    }
}
