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

namespace DraggableApp.Controls.ViewModels
{
    public class ConnectorViewModel : BindableBase
    {
        public ObservableCollection<Point> Points { get; set; } = new ObservableCollection<Point>();
        public ObservableCollection<LineGeometry> Lines { get; set; } = new ObservableCollection<LineGeometry>();

        private Point _startPoint;
        public Point StartPoint
        {
            get => _startPoint;
            set => SetProperty(ref _startPoint, value);
        }

        private Point _endPoint;
        public Point EndPoint
        {
            get => _endPoint;
            set => SetProperty(ref _endPoint, value);
        }

        public ReactiveProperty<string> PointsDump { get; set; } = new ReactiveProperty<string>();

        public DelegateCommand<DragDeltaEventArgs> LineDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> LineDragStartedCommand { get; set; }
        public DelegateCommand<DragCompletedEventArgs> LineDragCompletedCommand { get; set; }

        public DelegateCommand<DragDeltaEventArgs> EdgeDragDeltaCommand { get; set; }
        public DelegateCommand<DragStartedEventArgs> EdgeDragStartedCommand { get; set; }
        public DelegateCommand<DragCompletedEventArgs> EdgeDragCompletedCommand { get; set; }



        private Point _initialPosition;
        int _lineIndex;
        int _pointIndex;

        public ConnectorViewModel(List<Point> points)
        {
            Points = new ObservableCollection<Point>(points);
            Lines = new ObservableCollection<LineGeometry>();
            for(var i = 0; i < points.Count - 1; i++)
            {
                Lines.Add(new LineGeometry(points[i], points[i + 1]));
            }
            StartPoint = points.FirstOrDefault();
            EndPoint = points.LastOrDefault();

            LineDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>(OnLineDragDelta);
            LineDragCompletedCommand = new DelegateCommand<DragCompletedEventArgs>(OnLineDragCompleted);
            LineDragStartedCommand = new DelegateCommand<DragStartedEventArgs>(OnLineDragStarted);

            EdgeDragDeltaCommand = new DelegateCommand<DragDeltaEventArgs>(OnEdgeDragDelta);
            EdgeDragCompletedCommand = new DelegateCommand<DragCompletedEventArgs>(OnEdgeDragCompleted);
            EdgeDragStartedCommand = new DelegateCommand<DragStartedEventArgs>(OnEdgeDragStarted);
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
        private void OnLineDragStarted(DragStartedEventArgs args)
        {
            var source = (args.Source as Thumb).DataContext as LineGeometry;
            _lineIndex = Lines.Select((v, i) => new { line = v, i })
                .Where(x => x.line == source)
                .Select(x => x.i).First();
            _initialPosition = new Point { X = source.StartPoint.X, Y = source.StartPoint.Y };

        }
        private void OnLineDragDelta(DragDeltaEventArgs args)
        {
            var target = Lines[_lineIndex];
            if (_lineIndex == 0)
            {
                Lines.Insert(0, new LineGeometry(target.StartPoint, target.EndPoint));
                Points.Insert(0, new Point(target.StartPoint.X, target.StartPoint.Y));
                _lineIndex = 1;
            }
            if ((_lineIndex + 1) >= Lines.Count)
            {
                Lines.Add(new LineGeometry(target.StartPoint, target.EndPoint));
                Points.Add(new Point(target.EndPoint.X, target.EndPoint.Y));
                _lineIndex = Lines.Count - 2;
            }

            if (target.StartPoint.X == target.EndPoint.X)
            {
                var positionX = _initialPosition.X + (int)args.HorizontalChange; // horizontal
                target.StartPoint = new Point(positionX, target.StartPoint.Y);
                target.EndPoint = new Point(positionX, target.EndPoint.Y);

                Lines[_lineIndex - 1].EndPoint = new Point(target.StartPoint.X, target.StartPoint.Y);
                Lines[_lineIndex + 1].StartPoint = new Point(target.EndPoint.X, target.EndPoint.Y);

                Points[_lineIndex] = new Point(target.StartPoint.X, target.StartPoint.Y);
                Points[_lineIndex + 1] = new Point(target.EndPoint.X, target.EndPoint.Y);


            }
            if (target.StartPoint.Y == target.EndPoint.Y)
            {
                var positionY = _initialPosition.Y + (int)args.VerticalChange; // vertical
                target.StartPoint = new Point(target.StartPoint.X, positionY);
                target.EndPoint = new Point(target.EndPoint.X, positionY);

                Lines[_lineIndex - 1].EndPoint = new Point(target.StartPoint.X, target.StartPoint.Y);
                Lines[_lineIndex + 1].StartPoint = new Point(target.EndPoint.X, target.EndPoint.Y);

                Points[_lineIndex] = new Point(target.StartPoint.X, target.StartPoint.Y);
                Points[_lineIndex + 1] = new Point(target.EndPoint.X, target.EndPoint.Y);

            }
            args.Handled = true;
            RaisePropertyChanged(nameof(Points));
        }
        private void OnLineDragCompleted(DragCompletedEventArgs args)
        {
            if(_lineIndex > 0)
            {

                if (Lines[_lineIndex - 1].StartPoint == Lines[_lineIndex - 1].EndPoint)
                {
                    Lines.Remove(Lines[_lineIndex - 1]);
                    Points.Remove(Points[_lineIndex - 1]);
                    _lineIndex--;
                }
                if(_lineIndex > 0)
                {
                    if (Lines[_lineIndex - 1].EndPoint == Lines[_lineIndex].StartPoint && Lines[_lineIndex - 1].StartPoint.X == Lines[_lineIndex].EndPoint.X)
                    {
                        RemoveLine(_lineIndex, 1);
                        RemovePoint(_lineIndex, 1);
                        Lines[_lineIndex - 1].EndPoint = Lines[_lineIndex].StartPoint;
                        _lineIndex--;
                    }
                    else if (Lines[_lineIndex - 1].EndPoint == Lines[_lineIndex].StartPoint && Lines[_lineIndex - 1].StartPoint.Y == Lines[_lineIndex].EndPoint.Y)
                    {
                        RemoveLine(_lineIndex, 1);
                        RemovePoint(_lineIndex, 1);
                        Lines[_lineIndex - 1].EndPoint = Lines[_lineIndex].StartPoint;
                        _lineIndex--;
                    }
                }
            }
            if(_lineIndex < (Lines.Count - 1))
            {
                if (Lines[_lineIndex + 1].StartPoint == Lines[_lineIndex + 1].EndPoint)
                {
                    RemoveLine(_lineIndex + 1, 1);
                    RemovePoint(_lineIndex + 1, 1);
                }
                if (_lineIndex < (Lines.Count - 1))
                {
                    if (Lines[_lineIndex].EndPoint == Lines[_lineIndex + 1].StartPoint && Lines[_lineIndex].StartPoint.X == Lines[_lineIndex + 1].EndPoint.X)
                    {
                        if (_lineIndex < (Lines.Count - 1))
                        {
                            Lines[_lineIndex].EndPoint = Lines[_lineIndex + 1].EndPoint;
                        }
                        RemoveLine(_lineIndex + 1, 1);
                        RemovePoint(_lineIndex + 1, 1);

                    }
                    else if (Lines[_lineIndex].EndPoint == Lines[_lineIndex + 1].StartPoint && Lines[_lineIndex].StartPoint.Y == Lines[_lineIndex + 1].EndPoint.Y)
                    {
                        if (_lineIndex < (Lines.Count - 1))
                        {
                            Lines[_lineIndex].EndPoint = Lines[_lineIndex + 1].EndPoint;
                        }
                        RemoveLine(_lineIndex + 1, 1);
                        RemovePoint(_lineIndex + 1, 1);
                    }
                }

            }

            DumpPoints();
            RaisePropertyChanged(nameof(Points));

        }
        private void OnEdgeDragStarted(DragStartedEventArgs args)
        {
            
            var sourceName = (args.Source as Thumb).Name;
            if (sourceName == "startPoint")
            {
                _initialPosition = new Point(StartPoint.X, StartPoint.Y);
                _pointIndex = 0;
                _lineIndex = 0;
                if (Points.Count == 2)
                {
                    Points.Insert(0, _initialPosition);
                    Lines.Insert(0, new LineGeometry(_initialPosition, _initialPosition));
                }
            }
            else if(sourceName== "endPoint")
            {
                _initialPosition = new Point(EndPoint.X, EndPoint.Y);
                _pointIndex = Points.Count - 1;
                _lineIndex = Lines.Count - 1;

                if (Points.Count == 2)
                {
                    PointsDump.Value = string.Join("\r\n", Points.Select(p => $"{p.X} {p.Y}"));
                    Points.Add(_initialPosition);
                    Lines.Add(new LineGeometry(_initialPosition, _initialPosition));
                    _pointIndex = Points.Count - 1;
                    _lineIndex = Lines.Count - 1;
                }
            }
        }
        private void OnEdgeDragDelta(DragDeltaEventArgs args)
        {
            var pointStart = new Point(_initialPosition.X + (int)args.HorizontalChange, _initialPosition.Y + (int)args.VerticalChange);
            if(_pointIndex == 0)
            {
                var pointEnd = Points[_pointIndex + 1];
                StartPoint = pointStart;
                if (Points[_pointIndex ] == Points[_pointIndex + 1])
                {
                    if (_pointIndex < Points.Count - 2)
                    {
                        if (Points[_pointIndex + 1].X == Points[_pointIndex + 2].X)
                        {
                            pointEnd.X = Points[_pointIndex + 1].X;
                            pointEnd.Y = pointStart.Y;
                        }
                        else if (Points[_pointIndex + 1].Y == Points[_pointIndex + 2].Y)
                        {
                            pointEnd.X = pointStart.X;
                            pointEnd.Y = Points[_pointIndex + 1].Y;

                        }
                    }
                    Lines[_lineIndex].StartPoint = new Point(pointStart.X, pointStart.Y);
                    Lines[_lineIndex].EndPoint = new Point(pointEnd.X, pointEnd.Y);

                    Lines[_lineIndex + 1].StartPoint = new Point(pointStart.X, pointEnd.Y);
                    Points[_pointIndex] = new Point(pointStart.X, pointStart.Y);
                    Points[_pointIndex + 1] = new Point(pointEnd.X, pointEnd.Y);
                }
                else if (Points[_pointIndex].X == Points[_pointIndex + 1].X)
                {
                    pointEnd.X = pointStart.X;

                    Lines[_lineIndex].StartPoint = new Point(pointStart.X, pointStart.Y);
                    Lines[_lineIndex].EndPoint = new Point(pointEnd.X, pointEnd.Y);

                    Lines[_lineIndex + 1].StartPoint = new Point(pointStart.X, pointEnd.Y);
                    Points[_pointIndex] = new Point(pointStart.X, pointStart.Y);
                    Points[_pointIndex + 1] = new Point(pointEnd.X, pointEnd.Y);
                }
                else if (Points[_pointIndex].Y == Points[_pointIndex + 1].Y)
                {
                    pointEnd.Y = pointStart.Y;

                    Lines[_lineIndex].StartPoint = new Point(pointStart.X, pointStart.Y);
                    Lines[_lineIndex].EndPoint = new Point(pointEnd.X, pointEnd.Y);

                    Lines[_pointIndex + 1].StartPoint = new Point(pointEnd.X, pointEnd.Y);
                    Points[_pointIndex] = new Point(pointStart.X, pointStart.Y);
                    Points[_pointIndex + 1] = new Point(pointEnd.X, pointEnd.Y);
                }
            }
            else
            {
                var pointEnd = new Point(Points[_pointIndex - 1].X, Points[_pointIndex - 1].Y);
                EndPoint = pointStart;
                if (Points[_pointIndex] == Points[_pointIndex - 1])
                {
                    if(_pointIndex > 1)
                    {
                        if(Points[_pointIndex - 1].X == Points[_pointIndex - 2].X)
                        {
                            pointEnd.X = Points[_pointIndex - 1].X;
                            pointEnd.Y = pointStart.Y;
                        }
                        else if (Points[_pointIndex - 1].Y == Points[_pointIndex - 2].Y)
                        {
                            pointEnd.X = pointStart.X;
                            pointEnd.Y = Points[_pointIndex - 1].Y;

                        }
                    }
                    Lines[_lineIndex].StartPoint = new Point(pointEnd.X, pointEnd.Y);
                    Lines[_lineIndex].EndPoint = new Point(pointStart.X, pointStart.Y);

                    if (Lines.Count > 1)
                    {
                        Lines[_lineIndex - 1].EndPoint = new Point(pointEnd.X, pointEnd.Y);
                        Points[_pointIndex] = new Point(pointStart.X, pointStart.Y);
                        Points[_pointIndex - 1] = new Point(pointEnd.X, pointEnd.Y);
                    }
                }
                else if (Points[_pointIndex].X == Points[_pointIndex - 1].X)
                {
                    pointEnd.X = pointStart.X;

                    Lines[_lineIndex].StartPoint = new Point(pointEnd.X, pointEnd.Y);
                    Lines[_lineIndex].EndPoint = new Point(pointStart.X, pointStart.Y);

                    if(Lines.Count > 1)
                    {
                        Lines[_lineIndex - 1].EndPoint = new Point(pointEnd.X, pointEnd.Y);
                        Points[_pointIndex] = new Point(pointStart.X, pointStart.Y);
                        Points[_pointIndex - 1] = new Point(pointEnd.X, pointEnd.Y);
                    }
                }
                else if (Points[_pointIndex].Y == Points[_pointIndex - 1].Y)
                {
                    pointEnd.Y = pointStart.Y;

                    Lines[_lineIndex].StartPoint = new Point(pointEnd.X, pointEnd.Y);
                    Lines[_lineIndex].EndPoint = new Point(pointStart.X, pointStart.Y);

                    if (Lines.Count > 1)
                    {
                        Lines[_lineIndex - 1].EndPoint = new Point(pointEnd.X, pointEnd.Y);
                        Points[_pointIndex] = new Point(pointStart.X, pointStart.Y);
                        Points[_pointIndex - 1] = new Point(pointEnd.X, pointEnd.Y);
                    }
                }
            }

            _initialPosition = pointStart;
            RaisePropertyChanged(nameof(StartPoint));
            RaisePropertyChanged(nameof(EndPoint));
            RaisePropertyChanged(nameof(Points));
            RaisePropertyChanged(nameof(Lines));
            DumpPoints();

        }
        private void OnEdgeDragCompleted(DragCompletedEventArgs args)
        {
            if(_pointIndex == 0)
            {
                if(_pointIndex < (Points.Count - 2))
                {
                    if (Points[_pointIndex + 1] == Points[_pointIndex + 2])
                    {
                        Lines[_lineIndex].EndPoint = Points[_pointIndex + 2];
                        RemovePoint(_pointIndex + 1, 1);
                        RemoveLine(_lineIndex + 1, -1);
                    }
                }
                if (_pointIndex < (Points.Count - 2))
                {
                    if (Points[_pointIndex].X == Points[_pointIndex + 2].X)
                    {
                        Lines[_lineIndex].EndPoint = Points[_pointIndex + 2];
                        RemovePoint(_pointIndex + 1, 1);
                        RemoveLine(_lineIndex + 1, 1);
                    }
                    else if (Points[_pointIndex].Y == Points[_pointIndex + 2].Y)
                    {
                        Lines[_lineIndex].StartPoint = Points[_pointIndex + 2];
                        RemovePoint(_pointIndex + 1, 1);
                        RemoveLine(_lineIndex + 1, 1);
                    }
                }


            }
            else
            {
                if(_pointIndex > 1)
                {
                    if (Points[_pointIndex - 1] == Points[_pointIndex - 2])
                    {
                        Lines[_lineIndex].StartPoint = Points[Points.Count - 2];
                        RemovePoint(_pointIndex - 1, -1);
                        RemoveLine(_lineIndex - 1, -1);
                        _pointIndex--;
                        _lineIndex--;
                    }
                }
                if (_pointIndex > 1)
                {
                    if (Points[_pointIndex].X == Points[_pointIndex - 2].X)
                    {
                        Lines[_lineIndex].StartPoint = Points[Points.Count - 2];
                        RemovePoint(_pointIndex - 1, -1);
                        RemoveLine(_lineIndex - 1, -1);
                    }
                    else if (Points[_pointIndex].Y == Points[_pointIndex - 2].Y)
                    {
                        Lines[_lineIndex].StartPoint = Points[_pointIndex - 2];
                        RemovePoint(_pointIndex - 1, -1);
                        RemoveLine(_lineIndex - 1, -1);
                    }
                }
            }
            DumpPoints();
            RaisePropertyChanged(nameof(Points));
            RaisePropertyChanged(nameof(Lines));

        }

        private void RemovePoint(int index,int direction)
        {
            if(direction < 0)
            {
                if (index > 0)
                {
                    Points.Remove(Points[index]);
                }

            }
            else
            {
                if (index < (Points.Count - 1))
                {
                    Points.Remove(Points[index]);
                }
            }
        }
        private void RemoveLine(int index, int direction)
        {
            if(direction < 0)
            {
                if (index >= 0)
                {
                    Lines.Remove(Lines[index]);
                }

            }
            else
            {
                if (index <= (Lines.Count - 1))
                {
                    Lines.Remove(Lines[index]);
                }

            }
        }
        private void DumpPoints()
        {
            PointsDump.Value = string.Join("\r\n", Points.Select(p => $"{p.X} {p.Y}")) + "\r\n\r\n"
    + String.Join("\r\n", Lines.Select(line => $"{line.StartPoint.X} {line.StartPoint.Y} : {line.EndPoint.X} {line.EndPoint.Y}"));
            RaisePropertyChanged(nameof(PointsDump));

        }
    }
}
