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
    internal class SideMenuViewModel
    {
        private readonly IRegionManager _regionManager; 
        public ReactiveCommand ReturnToHomeCommand { get; set; } = new ReactiveCommand();


        public SideMenuViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ReturnToHomeCommand.Subscribe(() => ReturnToHome());
        }
        public void ReturnToHome()
        {
            _regionManager.RequestNavigate("ContentRegion", typeof(MenuView).FullName);
        }
    }
}
