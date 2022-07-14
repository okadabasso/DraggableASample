using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using DraggableApp.Controls.ViewModels;
using System.Reactive.Linq;
using System.Windows;

namespace DraggableApp.ViewModels
{
    public class ConnectorSampleViewModel : BindableBase
    {
        public ReactiveCommand ResetCommand { get; set; } = new ReactiveCommand();
        public ConnectorViewModel Connector { get; set;  }
        public DiagramItemVIewModel DiagramItem { get; set; }
        public ReactiveProperty<string> Message { get; set; }= new ReactiveProperty<string>();
        public ReactiveProperty<string> Watcher { get; set; }
        public ConnectorSampleViewModel()
        {
            Connector = new ConnectorViewModel(new List<System.Windows.Point>(){
                new System.Windows.Point(100,100),
                new System.Windows.Point(100,200),
                new System.Windows.Point(200,200),
                new System.Windows.Point(200,300),
                new System.Windows.Point(300,300),
            }) ;
            DiagramItem = new DiagramItemVIewModel { 
                Left = 500,
                Top = 100,
                Width = 100,
                Height = 100
            };

            ResetCommand.Subscribe(() => {
                Connector = new ConnectorViewModel(new List<System.Windows.Point>(){
                    new System.Windows.Point(100,100),
                    new System.Windows.Point(100,200),
                    new System.Windows.Point(200,200),
                    new System.Windows.Point(200,300),
                    new System.Windows.Point(300,300),
                });
                RaisePropertyChanged(nameof(Connector));
                Connector.PointsDump.Subscribe(x => {
                    Message.Value = x;
                    RaisePropertyChanged(nameof(Message));

                });
            });
            Connector.PointsDump.Subscribe(x => {
                Message.Value = x;
                RaisePropertyChanged(nameof(Message));

            });

        }
    }
}
