using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

using DraggableApp.Controls.ViewModels;

namespace DraggableApp.ViewModels
{
    public class ConnectorSampleViewModel : BindableBase
    {
        public ConnectorViewModel Connector { get; set;  }
        public ConnectorSampleViewModel()
        {
            Connector = new ConnectorViewModel(new List<System.Windows.Point>(){
                new System.Windows.Point(100,100),
                new System.Windows.Point(100,200),
                new System.Windows.Point(200,200),
                new System.Windows.Point(200,300),
                new System.Windows.Point(300,300),
            }) ;
            
            // continue
        }
    }
}
