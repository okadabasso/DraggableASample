using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DraggableApp.ViewModels
{
    public class CanvasViewModel : BindableBase
    {
        public ReactiveProperty<string> SampleText { get; set; } = new ReactiveProperty<string>("sample");
        public DelegateCommand<DragDeltaEventArgs> DragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> DragStartedCommand { get; set; }

        public ReactiveProperty<double> X { get; set; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> Y { get; set; } = new ReactiveProperty<double>();

        public ReactiveProperty<int> Width { get; set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> Height { get; set; } = new ReactiveProperty<int>();
        private Point InitialPosition;

        public CanvasViewModel()
        {
            DragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>((x) =>
            {
                X.Value = x.HorizontalChange;
                Y.Value = x.VerticalChange;

                Width.Value = (int) InitialPosition.X + (int) x.HorizontalChange;
                Height.Value = (int)InitialPosition.Y + (int)x.VerticalChange;
                InitialPosition = new Point { X = Width.Value, Y = Height.Value };
            });
            DragStartedCommand = new DelegateCommand<DragStartedEventArgs>((x) =>
            {
                InitialPosition = new Point { X = Width.Value, Y = Height.Value };
            });

        }
    }
}
