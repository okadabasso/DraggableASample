using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace DraggableApp.ViewModels
{
    public class LineDragViewModel : BindableBase
    {
        public DelegateCommand<DragDeltaEventArgs> DraggableDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> DraggableDragStartedCommand { get; set; }

        public DelegateCommand<DragDeltaEventArgs> LineDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> LineDragStartedCommand { get; set; }
        public DelegateCommand<DragCompletedEventArgs> LineDragCompletedCommand { get; set; }

        public ReactiveProperty<int> DraggableTop { get; set; } = new ReactiveProperty<int>(200);
        public ReactiveProperty<int> DraggableLeft { get; set; } = new ReactiveProperty<int>(100);
        private Point InitialPosition;
        public ReactiveProperty<double> X { get; set; } = new ReactiveProperty<double>();
        public ReactiveProperty<double> Y { get; set; } = new ReactiveProperty<double>();
        public ObservableCollection<Point> Points { get; set; } = new ObservableCollection<Point>(
            new Point[] {
            new Point(100, 100),
            new Point(100, 200),
            new Point(200, 200) });

        public ObservableCollection<LineGeometry> Lines { get; set; } = new ObservableCollection<LineGeometry>() { 
            new LineGeometry(new Point(100,100), new Point(100,200)),
            new LineGeometry(new Point(100,200), new Point(200,200)),
        };

        int lineIndex;
        public ReactiveProperty<string> Message { get; set; } = new ReactiveProperty<string>();
        public LineDragViewModel()
        {
            DraggableDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>((x) =>
            {
                X.Value = x.HorizontalChange;
                Y.Value = x.VerticalChange;

                DraggableLeft.Value += (int)x.HorizontalChange;
                DraggableTop.Value += (int)x.VerticalChange;
                InitialPosition = new Point { X = DraggableTop.Value, Y = DraggableLeft.Value };
            });
            DraggableDragStartedCommand = new DelegateCommand<DragStartedEventArgs>((args) =>
            {
                InitialPosition = new Point { X = DraggableTop.Value, Y = DraggableLeft.Value };
            });

            // line drag
            LineDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>((args) =>
            {
                var target = Lines[lineIndex];
                if(lineIndex == 0)
                {
                    Lines.Insert(0, new LineGeometry(target.StartPoint, target.EndPoint));
                    Points.Insert(0, new Point(target.StartPoint.X, target.StartPoint.Y));
                    lineIndex = 1;
                }
                else if((lineIndex +1) >= Lines.Count)
                {
                    Lines.Add(new LineGeometry(target.StartPoint, target.EndPoint));
                    Points.Add(new Point(target.EndPoint.X, target.EndPoint.Y));
                }

                if (target.StartPoint.X == target.EndPoint.X)
                {
                    var positionX = InitialPosition.X + (int)args.HorizontalChange; // horizontal
                    target.StartPoint = new Point(positionX, target.StartPoint.Y);
                    target.EndPoint = new Point(positionX, target.EndPoint.Y);

                    Lines[lineIndex - 1].EndPoint = new Point(target.StartPoint.X, target.StartPoint.Y);
                    Lines[lineIndex + 1].StartPoint= new Point(target.EndPoint.X, target.EndPoint.Y);

                    Points[lineIndex] = new Point(target.StartPoint.X, target.StartPoint.Y);
                    Points[lineIndex + 1] = new Point(target.EndPoint.X, target.EndPoint.Y);


                }
                if (target.StartPoint.Y == target.EndPoint.Y)
                {
                    var positionY = InitialPosition.Y + (int)args.VerticalChange; // vertical
                    target.StartPoint = new Point(target.StartPoint.X, positionY);
                    target.EndPoint = new Point(target.EndPoint.X, positionY);

                    Lines[lineIndex - 1].EndPoint = new Point(target.StartPoint.X, target.StartPoint.Y);
                    Lines[lineIndex + 1].StartPoint = new Point(target.EndPoint.X, target.EndPoint.Y);

                    Points[lineIndex] = new Point(target.StartPoint.X, target.StartPoint.Y);
                    Points[lineIndex + 1] = new Point(target.EndPoint.X, target.EndPoint.Y);

                }


                X.Value = args.HorizontalChange;
                Y.Value = target.StartPoint.Y;
                args.Handled = true;
                RaisePropertyChanged(nameof(Points));
            });
            LineDragCompletedCommand = new DelegateCommand<DragCompletedEventArgs>((args) =>
            {
                if (Lines[lineIndex + 1].StartPoint == Lines[lineIndex + 1].EndPoint)
                {
                    Lines.Remove(Lines[lineIndex + 1]);
                    if ((lineIndex + 1) < Lines.Count)
                    {
                        if (Lines[lineIndex].StartPoint.X == Lines[lineIndex + 1].StartPoint.X ||
                        Lines[lineIndex].StartPoint.Y == Lines[lineIndex + 1].StartPoint.Y)
                        {
                            Lines[lineIndex].EndPoint = Lines[lineIndex + 1].EndPoint;
                            Lines.Remove(Lines[lineIndex + 1]);
                        }
                    }
                }
                if(lineIndex > 0)
                {
                    if (Lines[lineIndex - 1].StartPoint == Lines[lineIndex - 1].EndPoint)
                    {
                        Lines.Remove(Lines[lineIndex - 1]);
                        if ((lineIndex - 2) >= 0)
                        {
                            if (Lines[lineIndex - 2].StartPoint.X == Lines[lineIndex - 1].StartPoint.X ||
                            Lines[lineIndex - 2].StartPoint.Y == Lines[lineIndex - 1].StartPoint.Y)
                            {
                                Lines[lineIndex - 1].StartPoint = Lines[lineIndex - 2].StartPoint;
                                Lines.Remove(Lines[lineIndex - 2]);
                            }
                        }
                    }

                }
                if (Points[lineIndex + 1] == Points[lineIndex + 2])
                {
                    Points.Remove(Points[lineIndex + 1]);
                    if ((lineIndex + 2) < Points.Count)
                    {
                        if (Points[lineIndex].X == Points[lineIndex + 1].X && Points[lineIndex + 1].X == Points[lineIndex + 2].X ||
                        Points[lineIndex].Y == Points[lineIndex + 1].Y && Points[lineIndex + 1].Y == Points[lineIndex + 2].Y)
                        {
                            Points.Remove(Points[lineIndex + 1]);
                        }
                    }
                }
                if(lineIndex > 0)
                {
                    if (Points[lineIndex - 1] == Points[lineIndex])
                    {
                        Points.Remove(Points[lineIndex]);
                        lineIndex--;
                        if ((lineIndex - 1) >= 0)
                        {
                            if (Points[lineIndex - 1].X == Points[lineIndex].X ||
                            Points[lineIndex - 1].Y == Points[lineIndex].Y)
                            {
                                Points.Remove(Points[lineIndex]);
                            }
                        }
                    }

                }
                RaisePropertyChanged(nameof(Points));

                var message = "";
                foreach(var point in Points)
                {
                    message += point.ToString() + Environment.NewLine;
                }
                message +=  Environment.NewLine;
                foreach (var line in Lines)
                {
                    message += line.StartPoint.ToString() + " " + line.EndPoint.ToString() + Environment.NewLine;
                }
                Message.Value = message;

            });
            LineDragStartedCommand = new DelegateCommand<DragStartedEventArgs>((x) =>
            {
                
                var source = (x.Source as Thumb).DataContext as LineGeometry ;
                lineIndex = Lines.Select((v,i) => new { line = v, i }).Where(x => x.line == source).Select(x => x.i).First();
                InitialPosition = new Point { X = source.StartPoint.X, Y = source.StartPoint.Y };
            });

        }
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }
    }
}
