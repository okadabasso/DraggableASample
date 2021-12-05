using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DraggableApp.ViewModels
{
    public class CanvasViewModel : BindableBase
    {
        public ReactiveProperty<string> SampleText { get; set; } = new ReactiveProperty<string>("sample");
        public DelegateCommand<DragDeltaEventArgs> ResizableDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> ResizableDragStartedCommand { get; set; }
        public DelegateCommand<DragDeltaEventArgs> DraggableDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> DraggableDragStartedCommand { get; set; }

        public ReactiveProperty<double> X { get; set; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> Y { get; set; } = new ReactiveProperty<double>();

        public ReactiveProperty<int> ResizableWidth { get; set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> ResizableHeight { get; set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> DraggableTop { get; set; } = new ReactiveProperty<int>(200);
        public ReactiveProperty<int> DraggableLeft { get; set; } = new ReactiveProperty<int>(100);
        private Point InitialPosition;


        public CanvasViewModel()
        {
            ResizableDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>((x) =>
            {
                X.Value = x.HorizontalChange;
                Y.Value = x.VerticalChange;

                var width = (int)InitialPosition.X + (int)x.HorizontalChange;
                var heigth = (int)InitialPosition.Y + (int)x.VerticalChange;
                ResizableWidth.Value = width >= 0 ? width : 0;
                ResizableHeight.Value = heigth >= 0 ? heigth : 0;
                InitialPosition = new Point { X = ResizableWidth.Value, Y = ResizableHeight.Value };
            });
            ResizableDragStartedCommand = new DelegateCommand<DragStartedEventArgs>((x) =>
            {
                InitialPosition = new Point { X = ResizableWidth.Value, Y = ResizableHeight.Value };
            });

            DraggableDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>((x) =>
            {
                X.Value = x.HorizontalChange;
                Y.Value = x.VerticalChange;

                DraggableLeft.Value += (int)x.HorizontalChange;
                DraggableTop.Value += (int)x.VerticalChange;
                InitialPosition = new Point { X = DraggableTop.Value, Y = DraggableLeft.Value };
            });
            DraggableDragStartedCommand = new DelegateCommand<DragStartedEventArgs>((x) =>
            {
                InitialPosition = new Point { X = DraggableTop.Value, Y = DraggableLeft.Value };
            });
        }
    }
}
