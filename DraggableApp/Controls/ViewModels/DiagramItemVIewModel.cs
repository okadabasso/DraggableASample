using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace DraggableApp.Controls.ViewModels
{
    public class DiagramItemVIewModel : BindableBase
    {
        private double _top;
        private double _left;
        private double _height;
        private double _width;
        private double _resizerTop;
        private double _resizerLeft;

        public double Top { get => _top; set => SetProperty(ref _top, value); }
        public double Left { get => _left; set => SetProperty(ref _left, value); }
        public double Height { get => _height; set => SetProperty(ref _height, value); }
        public double Width { get => _width; set => SetProperty(ref _width, value); }

        public double ResizerTop { get => _resizerTop; set => SetProperty(ref _resizerTop, value); }
        public double ResizerLeft { get => _resizerLeft; set => SetProperty(ref _resizerLeft, value); }


        public DelegateCommand<DragDeltaEventArgs> ResizableDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> ResizableDragStartedCommand { get; set; }
        public DelegateCommand<DragCompletedEventArgs> ResizableDragCompleteCommand { get; set; }
        public DelegateCommand<DragDeltaEventArgs> DraggableDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> DraggableDragStartedCommand { get; set; }
        public DelegateCommand<DragCompletedEventArgs> DraggableDragCompleteCommand { get; set; }
        private Point InitialPosition;


        public DiagramItemVIewModel()
        {
            ResizableDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>((x) =>
            {
                ResizerLeft = x.HorizontalChange;
                ResizerTop = x.VerticalChange;

                var width = InitialPosition.X + x.HorizontalChange;
                var heigth = InitialPosition.Y + x.VerticalChange;
                Width = width >= 0 ? width : 0;
                Height = heigth >= 0 ? heigth : 0;
                InitialPosition = new Point { X = Width, Y = Height };
            });
            ResizableDragStartedCommand = new DelegateCommand<DragStartedEventArgs>((x) =>
            {
                InitialPosition = new Point { X = Width, Y = Height };
            });

            DraggableDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>((x) =>
            {
                Left += x.HorizontalChange;
                Top += x.VerticalChange;

                InitialPosition = new Point { X = Left, Y = Top };
            });
            DraggableDragStartedCommand = new DelegateCommand<DragStartedEventArgs>((x) =>
            {
                InitialPosition = new Point { X = Left, Y = Top };
            });

        }
    }
}
