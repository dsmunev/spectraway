using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SpectraWay.Device.Spectrometer;

namespace SpectraWay.Controls
{
    public partial class PerfGraph : UserControl
    {
        public PerfGraph()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }



        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set
            {
                SetValue(MaxXProperty, value);
                RefreshTicks();
            }
        }
        public double MinX
        {
            get { return (double)GetValue(MinXProperty); }
            set
            {
                SetValue(MinXProperty, value);
                RefreshTicks();
            }
        }

        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set
            {
                SetValue(MaxYProperty, value);
                RefreshTicks();
            }
        }

        public double MinY
        {
            get { return (double)GetValue(MinYProperty); }
            set
            {
                SetValue(MinYProperty, value);
                RefreshTicks();
            }
        }

        public List<SpectrometerDataPoint> DataPoints
        {
            get { return (List<SpectrometerDataPoint>)GetValue(DataPointsProperty); }
            set { SetValue(DataPointsProperty, value); }
        }

        public Style BackgroundStyle
        {
            get { return (Style)GetValue(BackgroundStyleProperty); }
            set { SetValue(BackgroundStyleProperty, value); }
        }

        public SpectrometerDataPoint DataItem
        {
            get { return (SpectrometerDataPoint)GetValue(DataItemProperty); }
            set { SetValue(DataItemProperty, value); }
        }

        public bool ShowTooltip
        {
            get { return (bool)GetValue(ShowTooltipProperty); }
            set
            {
                SetValue(ShowTooltipProperty, value);
                Tooltip.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool ShowLocator
        {
            get { return (bool)GetValue(ShowLocatorProperty); }
            set { SetValue(ShowLocatorProperty, value); }
        }



        public static readonly DependencyProperty MaxXProperty =
            DependencyProperty.Register(
                "MaxX",
                typeof(double),
                typeof(PerfGraph),
                new PropertyMetadata(.0, OnMaxMinChanged));


        public static readonly DependencyProperty MinXProperty =
            DependencyProperty.Register(
                "MinX",
                typeof(double),
                typeof(PerfGraph),
                new PropertyMetadata(.0, OnMaxMinChanged));


        public static readonly DependencyProperty MaxYProperty =
            DependencyProperty.Register(
                "MaxY",
                typeof(double),
                typeof(PerfGraph),
                new PropertyMetadata(.0, OnMaxMinChanged));




        public static readonly DependencyProperty MinYProperty =
            DependencyProperty.Register(
                "MinY",
                typeof(double),
                typeof(PerfGraph),
                new PropertyMetadata(.0, OnMaxMinChanged));


        public static readonly DependencyProperty DataPointsProperty =
            DependencyProperty.Register(
                "DataPoints",
                typeof(List<SpectrometerDataPoint>),
                typeof(PerfGraph),
                new PropertyMetadata(null, OnDataPointsChanged));


        public static readonly DependencyProperty BackgroundStyleProperty =
            DependencyProperty.Register(
                "BackgroundStyle",
                typeof(Style),
                typeof(PerfGraph),
                new PropertyMetadata(null));


        public static readonly DependencyProperty DataItemProperty =
            DependencyProperty.Register(
                "DataItem",
                typeof(SpectrometerDataPoint),
                typeof(PerfGraph),
                new PropertyMetadata(null));



        public static readonly DependencyProperty ShowTooltipProperty =
            DependencyProperty.Register(
                "ShowTooltip",
                typeof(bool),
                typeof(PerfGraph),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ShowLocatorProperty =
            DependencyProperty.Register(
                "ShowLocator",
                typeof(bool),
                typeof(PerfGraph),
                new PropertyMetadata(false));


        private static void OnMaxMinChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as PerfGraph;
            control?.RefreshTicks();
            control?.UpdateView();
        }

        private static void OnDataPointsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            //Debug.Print($"{nameof(OnDataPointsChanged)} fire");
            var control = dependencyObject as PerfGraph;
            control?.UpdateView();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var control = sender as PerfGraph;
            control?.RefreshTicks();
            control?.UpdateView();
        }
        private static Brush _blackBrush = new SolidColorBrush(Colors.Black);
        private static Brush _greyBrush = new SolidColorBrush(Colors.LightGray);
        private static Brush _whiteBrush = new SolidColorBrush(Colors.White);
        private static int _tickHeight = 6;
        private void RefreshTicks()
        {
            //return;
            TicksCanvas.Children.RemoveRange(0, TicksCanvas.Children.Count);
            var deltaX = MaxX - MinX;
            var deltaY = MaxY - MinY;
            if (!(deltaX > 0 && deltaY > 0)) return;

            var miniStepX = 10;
            var stepX = 50;
            if (deltaX > 100)
            {
                miniStepX = 20;
                stepX = 100;
            }

            var miniStepY = 2;
            var stepY = 10;
            if (deltaY > 100)
            {
                miniStepY = 10;
                stepY = 50;
            }
            if (deltaY > 500)
            {
                miniStepY = 50;
                stepY = 100;
            }
            if (deltaY > 2000)
            {
                miniStepY = 100;
                stepY = 500;
            }
            if (deltaY > 10000)
            {
                miniStepY = 1000;
                stepY = 5000;
            }

            //foreach (var point in dataPoints)
            //{
            //    var x = deltaX > 0 ? Normalize(width * ((point.WaveLength - MinX) / deltaX), width, 1) : width / 2;
            //    var y = deltaY > 0 ? Normalize(height * (1 - (point.Intencity - MinY) / deltaY), height, 1) : height / 2;
            //    yield return new Tuple<Point, ISpectrometerDataPoint>(new Point(x, y), point);
            //}

            var startOffsetX = MinX % miniStepX;
            var endOffsetX = MaxX % miniStepX;

            for (var currentX = (int)(MinX - MinX % miniStepX + miniStepX); currentX < MaxX; currentX += miniStepX)
            {
                var x = GraphCanvas.ActualWidth * (currentX - MinX) / deltaX;
                var isStepX = currentX % stepX == 0;
                if (isStepX)
                {
                    var textBlock = new TextBlock
                    {
                        Text = currentX.ToString(),
                        Foreground = _blackBrush,
                        FontSize = 12,
                        Background = _whiteBrush,
                        TextAlignment = TextAlignment.Right
                    };
                    textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    var size = textBlock.DesiredSize;
                    TicksCanvas.Children.Add(textBlock);
                    Canvas.SetLeft(textBlock, x - size.Width / 2);
                    Canvas.SetTop(textBlock, GraphCanvas.ActualHeight + _tickHeight + 2);
                }

                TicksCanvas.Children.Add(new Line
                {
                    X1 = x,
                    X2 = x,
                    Y1 = GraphCanvas.ActualHeight,
                    Y2 = GraphCanvas.ActualHeight + _tickHeight,
                    Stroke = _blackBrush,
                    StrokeThickness = isStepX ? 1 : .5
                });

                //TicksCanvas.Children.Add(new Line
                //{
                //    X1 = x,
                //    X2 = x,
                //    Y1 = 0,
                //    Y2 = GraphCanvas.ActualHeight,
                //    Stroke = _greyBrush,
                //    StrokeDashArray = new DoubleCollection() { 1, 1 },
                //    StrokeThickness = isStepX ? 2 : 1
                //});

            }


            for (var currentY = (int)(MinY - MinY % miniStepY + miniStepY); currentY < MaxY; currentY += miniStepY)
            {
                var y = GraphCanvas.ActualHeight * (1 - (currentY - MinY) / deltaY);
                var isStepY = currentY % stepY == 0;
                if (isStepY)
                {
                    var textBlock = new TextBlock
                    {
                        Text = currentY.ToString(),
                        Foreground = _blackBrush,
                        FontSize = 12,
                        Background = _whiteBrush,
                        TextAlignment = TextAlignment.Right
                    };
                    textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    var size = textBlock.DesiredSize;
                    TicksCanvas.Children.Add(textBlock);
                    Canvas.SetLeft(textBlock, -size.Width - 2 - _tickHeight);
                    Canvas.SetTop(textBlock, y - size.Height / 2);

                }
                TicksCanvas.Children.Add(new Line
                {
                    Y1 = y,
                    Y2 = y,
                    X1 = 0,
                    X2 = -_tickHeight,
                    Stroke = _blackBrush,
                    StrokeThickness = isStepY ? 1 : .5
                });

                //TicksCanvas.Children.Add(new Line
                //{
                //    Y1 = y,
                //    Y2 = y,
                //    X1 = 0,
                //    X2 = GraphCanvas.ActualWidth,
                //    Stroke = _greyBrush,
                //    StrokeDashArray = new DoubleCollection() { 1, 1 },
                //    StrokeThickness = isStepY ? 2 : 1
                //});

            }


        }



        private Tuple<Point, SpectrometerDataPoint>[] pointsForShow;
        private async void UpdateView()
        {
            if (DataPoints == null || DataPoints.Count == 0)
            {
                Path.Data = null;
                return;
            }
                
            pointsForShow = await CalculatePoints(DataPoints, (int)GraphCanvas.ActualWidth, (int)GraphCanvas.ActualHeight);
            if(pointsForShow.Length == 0) return;
            var segmentsList = new PathSegmentCollection();
            for (int i = 1; i < pointsForShow.Length; i++)
            {
                var lineSegment = new LineSegment()
                {
                    Point = pointsForShow[i].Item1,
                };

                //lineSegment.IsSmoothJoin = true;
                segmentsList.Add(lineSegment);
            }
            PathFigure pathFigure = new PathFigure()
            {
                StartPoint = pointsForShow[0].Item1,
                Segments = segmentsList,
                IsFilled = false,
                IsClosed = false
            };
            var geometry = new PathGeometry() { Figures = new PathFigureCollection() { pathFigure } };
            Path.Data = geometry;
            SetTooltip();
        }

        private async Task<Tuple<Point, SpectrometerDataPoint>[]> CalculatePoints(List<SpectrometerDataPoint> dataPoints, int width, int height)
        {

            var deltaX = MaxX - MinX;

            var deltaY = MaxY - MinY;

            if (!(deltaX > 0 && deltaY > 0)) return  new Tuple<Point, SpectrometerDataPoint>[] { };

            if (dataPoints.Count == 1)
            {
                return new Tuple<Point, SpectrometerDataPoint>[] {
                    new Tuple<Point, SpectrometerDataPoint>(new Point(0, height / 2), dataPoints[0]),
                    new Tuple<Point, SpectrometerDataPoint>(new Point(width, height / 2), dataPoints[0])
                };
            }
            var minX = MinX;
            var minY = MinY;
            var array = new Tuple<Point, SpectrometerDataPoint>[dataPoints.Count];
            await Task.Run(() =>
            {
                for (int index = 0; index < dataPoints.Count; index++)
                {
                    var point = dataPoints[index];
                    var x = deltaX > 0 ? Normalize(width * ((point.WaveLength - minX) / deltaX), width, 1) : width / 2;
                    var y = deltaY > 0 ? Normalize(height * (1 - (point.Intencity - minY) / deltaY), height, 2) : height / 2;
                    array[index] = new Tuple<Point, SpectrometerDataPoint>(new Point(x, y), point);
                }

            });


            return array;

        }

        private double Normalize(double d, int maxValue, int padding)
        {
            if (d <= padding)
                return padding;
            if (d > maxValue - padding)
                return maxValue - padding;
            return d;
        }

        private double _previousX;
        private bool _isSetYForTooltip;
        private volatile bool _isMouseEnter;
        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!ShowLocator || pointsForShow == null || pointsForShow.Length < 10) return;
            ShowTooltip = true;
            _previousX = e.GetPosition(GraphCanvas).X;
            _isSetYForTooltip = true;
            _isMouseEnter = true;
            SetTooltip();
        }

        private void SetTooltip()
        {
            if (!ShowTooltip || !_isMouseEnter) return;

            double minRange = Double.MaxValue;
            Tuple<Point, SpectrometerDataPoint> minValue = null;
            var rawIndex = (int) (_previousX / GraphCanvas.ActualWidth*pointsForShow.Length);
            for (int index = Math.Max(rawIndex - 4, 0); index < Math.Min(rawIndex + 4, pointsForShow.Length); index++)
            {
                Tuple<Point, SpectrometerDataPoint> tuple = pointsForShow[index];
                var range = Math.Abs(tuple.Item1.X - _previousX);
                if (range < minRange)
                {
                    minRange = range;
                    minValue = tuple;
                }
            }
            if (minValue != null)
            {
                Tooltip.Visibility = Visibility.Visible;
                DataItem = minValue.Item2;
                XBlock.Text = Math.Round(minValue.Item2.WaveLength, 2).ToString(CultureInfo.InvariantCulture);
                YBlock.Text = Math.Round(minValue.Item2.Intencity, 2).ToString(CultureInfo.InvariantCulture);

                Canvas.SetLeft(Tooltip, minValue.Item1.X - 10 - Tooltip.ActualWidth);
                
                if (_isSetYForTooltip) Canvas.SetTop(Tooltip, minValue.Item1.Y - 33);
                _isSetYForTooltip = false;

                HorisontalLocator.Data = new LineGeometry()
                {
                    StartPoint = new Point(0, minValue.Item1.Y),
                    EndPoint = new Point(GraphCanvas.ActualWidth, minValue.Item1.Y)
                };
                VerticalLocator.Data = new LineGeometry()
                {
                    StartPoint = new Point(minValue.Item1.X, 0),
                    EndPoint = new Point(minValue.Item1.X, GraphCanvas.ActualHeight)
                };
            }
            else
            {
                Tooltip.Visibility = Visibility.Collapsed;
                DataItem = new SpectrometerDataPoint();
                HorisontalLocator.Data = null;
                VerticalLocator.Data = null;
                ShowTooltip = false;
            }
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseEnter = false;
            Tooltip.Visibility = Visibility.Collapsed;
            ShowTooltip = false;
            HorisontalLocator.Data = null;
            VerticalLocator.Data = null;
        }

    }
}
