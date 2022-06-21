using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using DraggableApp.Views;
using Prism.Regions;

namespace DraggableApp.ViewModels
{
    internal class MenuViewModel
    {
        public ReactiveCommand<string> ExecuteMenuCommand { get; set; } = new ReactiveCommand<string>();
        public ObservableCollection<string> Menus { get; set; }


        private readonly IRegionManager _regionManager;

        public MenuViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ExecuteMenuCommand.Subscribe( parameter => ExecuteMenu(parameter));

            Menus = new ObservableCollection<string> { 
                typeof(CanvasView).FullName,
                typeof(ConnectorSampleView).FullName,
                typeof(LineDragView).FullName,
            };
        }
        public void ExecuteMenu(string parameter)
        {
            _regionManager.RequestNavigate("ContentRegion", parameter);
        }
    }
}
