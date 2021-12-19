using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace DraggableApp.Controls.ViewModels
{
    public class ConnectorViewModel : BindableBase
    {
        public ObservableCollection<Point> Points { get; set; } = new ObservableCollection<Point>();
        public DelegateCommand<DragDeltaEventArgs> LineDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> LineDragStartedCommand { get; set; }
        public DelegateCommand<DragCompletedEventArgs> LineDragCompletedCommand { get; set; }
        public ObservableCollection<LineGeometry> Lines { get; set; } = new ObservableCollection<LineGeometry>();


        private Point InitialPosition;
        int lineIndex;

        public ConnectorViewModel(List<Point> points)
        {
            Points = new ObservableCollection<Point>(points);
            Lines = new ObservableCollection<LineGeometry>();
            for(var i = 0; i < points.Count - 1; i++)
            {
                Lines.Add(new LineGeometry(points[i], points[i + 1]));
            }

            LineDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>(OnDragDelta);
            LineDragCompletedCommand = new DelegateCommand<DragCompletedEventArgs>(OnDragCompleted);
            LineDragStartedCommand = new DelegateCommand<DragStartedEventArgs>(OnDragStarted);

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
        private void OnDragStarted(DragStartedEventArgs args)
        {
            var source = (args.Source as Thumb).DataContext as LineGeometry;
            lineIndex = Lines.Select((v, i) => new { line = v, i })
                .Where(x => x.line == source)
                .Select(x => x.i).First();
            InitialPosition = new Point { X = source.StartPoint.X, Y = source.StartPoint.Y };

        }
        private void OnDragDelta(DragDeltaEventArgs args)
        {
            var target = Lines[lineIndex];
            if (lineIndex == 0)
            {
                Lines.Insert(0, new LineGeometry(target.StartPoint, target.EndPoint));
                Points.Insert(0, new Point(target.StartPoint.X, target.StartPoint.Y));
                lineIndex = 1;
            }
            else if ((lineIndex + 1) >= Lines.Count)
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
                Lines[lineIndex + 1].StartPoint = new Point(target.EndPoint.X, target.EndPoint.Y);

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
            args.Handled = true;
            RaisePropertyChanged(nameof(Points));
        }
        private void OnDragCompleted(DragCompletedEventArgs args)
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
            if (lineIndex > 0)
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
            if (lineIndex > 0)
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

        }
    }
}
