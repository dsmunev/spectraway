using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SpectraWay.Converter;
using SpectraWay.Device.Spectrometer;
using SpectraWay.Extension;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.Controls
{
    /// <summary>
    /// Interaction logic for CustomMultilineGraph.xaml
    /// </summary>
    public partial class CustomMultilineGraph : UserControl
    {
        public CustomMultilineGraph()
        {

            InitializeComponent();
            SizeChanged += OnSizeChanged;

        }


        public ExperimentEntityDataViewModel Data
        {
            get { return (ExperimentEntityDataViewModel)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }



        public Style BackgroundStyle
        {
            get { return (Style)GetValue(BackgroundStyleProperty); }
            set { SetValue(BackgroundStyleProperty, value); }
        }



        public bool ShowLocator
        {
            get { return (bool)GetValue(ShowLocatorProperty); }
            set { SetValue(ShowLocatorProperty, value); }
        }


        public bool IsRefresh
        {
            get { return (bool)GetValue(ShowLocatorProperty); }
            set { SetValue(ShowLocatorProperty, value); }
        }

        public bool IsLogScale
        {
            get { return (bool)GetValue(IsLogScaleProperty); }
            set { SetValue(IsLogScaleProperty, value); }
        }


        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                "Data",
                typeof(ExperimentEntityDataViewModel),
                typeof(CustomMultilineGraph),
                new PropertyMetadata(null, OnDataChanged));


        public static readonly DependencyProperty BackgroundStyleProperty =
            DependencyProperty.Register(
                "BackgroundStyle",
                typeof(Style),
                typeof(CustomMultilineGraph),
                new PropertyMetadata(null));


        public static readonly DependencyProperty DataItemProperty =
            DependencyProperty.Register(
                "DataItem",
                typeof(SpectrometerDataPoint),
                typeof(CustomMultilineGraph),
                new PropertyMetadata(null));



        public static readonly DependencyProperty ShowTooltipProperty =
            DependencyProperty.Register(
                "ShowTooltip",
                typeof(bool),
                typeof(CustomMultilineGraph),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ShowLocatorProperty =
            DependencyProperty.Register(
                "ShowLocator",
                typeof(bool),
                typeof(CustomMultilineGraph),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsRefreshProperty =
            DependencyProperty.Register(
                "IsRefresh",
                typeof(bool),
                typeof(CustomMultilineGraph),
                new PropertyMetadata(false, IsRefreshPropertyChangedCallback));

        public static readonly DependencyProperty IsLogScaleProperty =
            DependencyProperty.Register(
                "IsLogScale",
                typeof(bool),
                typeof(CustomMultilineGraph),
                new PropertyMetadata(false, IsLogScalePropertyChangedCallback));


        //public bool IsRemoveNoise
        //{
        //    get { return (bool)GetValue(IsRemoveNoiseProperty); }
        //    set { SetValue(IsRemoveNoiseProperty, value); }
        //}
        //public static readonly DependencyProperty IsRemoveNoiseProperty =
        //    DependencyProperty.Register(
        //        "IsRemoveNoise",
        //        typeof(bool),
        //        typeof(CustomMultilineGraph),
        //        new PropertyMetadata(false, IsRemoveNoisePropertyChangedCallback));



        //public bool IsNormalize
        //{
        //    get { return (bool)GetValue(IsNormalizeProperty); }
        //    set { SetValue(IsNormalizeProperty, value); }
        //}
        //public static readonly DependencyProperty IsNormalizeProperty =
        //    DependencyProperty.Register(
        //        "IsNormalize",
        //        typeof(bool),
        //        typeof(CustomMultilineGraph),
        //        new PropertyMetadata(false, IsNormalizePropertyChangedCallback));



        public bool IsDivideToBase
        {
            get { return (bool)GetValue(IsDivideToBaseProperty); }
            set { SetValue(IsDivideToBaseProperty, value); }
        }
        public static readonly DependencyProperty IsDivideToBaseProperty =
            DependencyProperty.Register(
                "IsDivideToBase",
                typeof(bool),
                typeof(CustomMultilineGraph),
                new PropertyMetadata(false, IsDivideToBasePropertyChangedCallback));



        private bool _isLogScale;
        private static void IsLogScalePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as CustomMultilineGraph;
            if (control != null)
            {
                control._isLogScale = (bool)e.NewValue;
                control.SetYTicks();
                control.UpdateView();
            }
        }

        private static void IsRefreshPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as CustomMultilineGraph;
            if (control != null)
            {
                control.RefreshTicks();
                control.UpdateView();
            }
        }

        private bool _isDivideToBase;
        private double[] _base;
        private static void IsDivideToBasePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as CustomMultilineGraph;
            if (control != null)
            {
                var isDivideToBase = (bool)e.NewValue;
                if (isDivideToBase != control._isDivideToBase)
                {
                    control._isDivideToBase = isDivideToBase;
                    if (control._isDivideToBase)
                    {
                        SetBase(control);
                    }
                    else
                    {
                        control._base = null;
                    }


                    control.RefreshTicks();
                    control.UpdateView();
                }
            }
        }

        private static void SetBase(CustomMultilineGraph control)
        {
            lock (control._syncObj)
            {
                var any = control.Data?.DataItems?.Any(x => x.IsBase);
                if (any != null && (bool)any)
                {
                    control._base = (double[])control.Data.DataItems.First(x => x.IsBase).IntensityArray.Clone();
                    var min = control._base.Where(x => x > 0).DefaultIfEmpty(0.1).Min();
                    min *= 0.1;
                    for (int i = 0; i < control._base.Length; i++)
                    {
                        if (control._base[i] <= 0)
                        {
                            control._base[i] = min;
                        }
                    }
                }
            }
        }


        public double MaxX => Data?.WaveLengthArray?.Max() ?? 0;

        public double MinX => Data?.WaveLengthArray?.Min() ?? 0;

        public double MaxY => Data?.
            DataItems?.
            Where(x => !x.IsNormalize || (x.IsNormalize && x.IsShow)).
            Where(x => !_isDivideToBase || !(x.IsNoise || x.IsNormalize)).
            Select(x => x.IntensityArray.
                            Select((data, index) => _isDivideToBase ? data / _base[index] : data).Max()).
            DefaultIfEmpty(0).
            Max() ?? 0;

        public double MinY => Data?.
            DataItems?.
            Where(x => !x.IsNormalize || (x.IsNormalize && x.IsShow)).
            Where(x => !_isDivideToBase || !(x.IsNoise || x.IsNormalize)).
            Select(x => x.IntensityArray.Select((data, index) => _isDivideToBase ? data / _base[index] : data).Where(y => !_isLogScale || y > 0).Min()).
            DefaultIfEmpty(0).
            Min() ?? (_isLogScale ? 0.01 : 0);


        private static void OnDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            //Debug.Print($"{nameof(OnDataPointsChanged)} fire");
            var control = dependencyObject as CustomMultilineGraph;
            if (control != null)
            {
                control.RefreshTicks();
                control.UpdateView();

                if (e.OldValue is ExperimentEntityDataViewModel)
                {
                    var oldData = ((ExperimentEntityDataViewModel)e.OldValue);
                    oldData.PropertyChanged -= control.SavedDataOnPropertyChanged;
                    if (oldData.DataItems != null)
                    {
                        oldData.DataItems.CollectionChanged -= control.DataOnCollectionChanged;
                        oldData.DataItems.ItemPropertyChanged -= control.DataItemOnCollectionChanged;
                    }
                }


                if (e.NewValue is ExperimentEntityDataViewModel)
                {
                    var newData = ((ExperimentEntityDataViewModel)e.NewValue);
                    newData.PropertyChanged += control.SavedDataOnPropertyChanged;
                    if (newData.DataItems != null)
                    {
                        newData.DataItems.CollectionChanged += control.DataOnCollectionChanged;
                        newData.DataItems.ItemPropertyChanged += control.DataItemOnCollectionChanged;
                    }
                }

            }
        }
        private void DataOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetYTicks();
            UpdateView();
        }

        private void SavedDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //todo remove event subscription form previous value
            if (e.PropertyName == "DataItems")
            {
                if (Data.DataItems != null)
                {
                    Data.DataItems.CollectionChanged += DataOnCollectionChanged;
                    Data.DataItems.ItemPropertyChanged += DataItemOnCollectionChanged;
                }
            }

            RefreshTicks();
            UpdateView();
        }

        private void DataItemOnCollectionChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsShow" && !((FullyObservableCollection<ExperimentEntityDataItemViewModel>)sender)[e.CollectionIndex].IsNormalize)
            {
                var key = GetKey(Data.DataItems[e.CollectionIndex]);
                if (_dictionaryPaths.ContainsKey(key))
                {
                    _dictionaryPaths[key].Visibility = Data.DataItems[e.CollectionIndex].IsShow
                        ? Visibility.Visible
                        : Visibility.Hidden;
                }
            }
            else
            {
                SetYTicks();
                UpdateView();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var control = sender as CustomMultilineGraph;
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


            if (!((MaxX - MinX) > 0 && (MaxY - MinY) > 0)) return;

            SetXTicks();
            SetYTicks();
        }

        private void SetYTicks()
        {
            if (_isLogScale)
            {
                SetLogYTicks();
                return;
            }
            YTicksCanvas.Children.RemoveRange(0, YTicksCanvas.Children.Count);

            var deltaY = MaxY - MinY;
            if (!(deltaY > 0)) return;

            var miniStepY = 0.1;
            var stepY = .5;
            if (deltaY > 10)
            {
                miniStepY = 5;
                stepY = 10;
            }
            if (deltaY > 100)
            {
                miniStepY = 50;
                stepY = 100;
            }

            if (deltaY > 1000)
            {
                miniStepY = 100;
                stepY = 500;
            }
            if (deltaY > 10000)
            {
                miniStepY = 1000;
                stepY = 5000;
            }
            if (deltaY > 100000)
            {
                miniStepY = 10000;
                stepY = 100000;
            }
            for (var currentY = (MinY - MinY % miniStepY + miniStepY); currentY < MaxY; currentY += miniStepY)
            {
                var y = GraphCanvas.ActualHeight * (1 - (currentY - MinY) / deltaY);
                //y = ScaleY(y);
                var isStepY = currentY % stepY == 0;
                if (isStepY)
                {
                    var textBlock = new TextBlock
                    {
                        Text = currentY.ToString(),
                        Foreground = _blackBrush,
                        FontSize = 12,
                        Background = _whiteBrush,
                        TextAlignment = TextAlignment.Right,


                    };
                    textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    var size = textBlock.DesiredSize;
                    YTicksCanvas.Children.Add(textBlock);

                    Canvas.SetLeft(textBlock, -size.Width - 2 - _tickHeight);
                    Canvas.SetTop(textBlock, y - size.Height / 2);
                }
                YTicksCanvas.Children.Add(new Line
                {
                    Y1 = y,
                    Y2 = y,
                    X1 = 0,
                    X2 = -_tickHeight,
                    Stroke = _blackBrush,
                    StrokeThickness = isStepY ? 1 : .5,
                });

                //YTicksCanvas.Children.Add(new Line
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

        private void SetLogYTicks()
        {
            YTicksCanvas.Children.RemoveRange(0, YTicksCanvas.Children.Count);


            var stepY = 1;

            var logMaxY = Math.Log10(MaxY);
            var logMinY = Math.Log10(MinY);
            var deltaY = Math.Ceiling(logMaxY) - Math.Floor(logMinY);


            var realMaxY = Math.Ceiling(MaxY / Math.Pow(10, Math.Floor(logMaxY))) * Math.Pow(10, Math.Floor(logMaxY));
            var realMinY = Math.Ceiling(MinY / Math.Pow(10, Math.Floor(logMinY))) * Math.Pow(10, Math.Floor(logMinY));

            double start = (int)Math.Ceiling(logMaxY) - deltaY + 1;
            var end = (int)Math.Ceiling(logMaxY);

            //var minY = (int)Math.Ceiling(logMaxY) - deltaY;
            var minY = Math.Log10(realMinY);
            var maxY = (int)Math.Log10(realMaxY);

            var realDeltaY = Math.Log10(realMaxY) - Math.Log10(realMinY);

            var showMiniTicksLabels = realDeltaY < 1;


            for (var currentY = start; currentY <= end; currentY += stepY)
            {
                //var y = GraphCanvas.ActualHeight * (1 - ((currentY - minY)) / (deltaY));
                var y = (int)Math.Round(GraphCanvas.ActualHeight * (1 - (currentY - minY) / realDeltaY));

                var currentVal = Math.Pow(10, currentY);
                if (currentVal < realMaxY && currentVal > realMinY)
                {
                    var panel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,

                        Children =
                        {
                            new TextBlock
                            {
                                Text = 10.ToString(),
                                Foreground = _blackBrush,
                                FontSize = 12,
                                Background = _whiteBrush,
                                TextAlignment = TextAlignment.Right,
                            },
                            new TextBlock
                            {
                                Text = currentY.ToString(),
                                Foreground = _blackBrush,
                                FontSize = 8,
                                VerticalAlignment = VerticalAlignment.Top,
                                Background = _whiteBrush,
                                TextAlignment = TextAlignment.Right,
                            },
                        }
                    };

                    //var textBlock =
                    panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    var size = panel.DesiredSize;
                    YTicksCanvas.Children.Add(panel);

                    Canvas.SetLeft(panel, -size.Width - 2 - _tickHeight);
                    Canvas.SetTop(panel, y - size.Height / 2);

                    YTicksCanvas.Children.Add(new Line
                    {
                        Y1 = y,
                        Y2 = y,
                        X1 = 0,
                        X2 = -_tickHeight,
                        Stroke = _blackBrush,
                        StrokeThickness = 1,
                    });
                }
                var step = Math.Pow(10, currentY - 1);
                for (var j = Math.Pow(10, currentY - 1) + step;
                    j < Math.Pow(10, currentY);
                    j += step)
                {
                    if (j < realMaxY && j > realMinY)
                    {
                        var minorY = (int)Math.Round(GraphCanvas.ActualHeight * (1 - ((Math.Log10(j) - minY)) / (realDeltaY)));
                        YTicksCanvas.Children.Add(new Line
                        {
                            Y1 = minorY,
                            Y2 = minorY,
                            X1 = 0,
                            X2 = -_tickHeight,
                            Stroke = _blackBrush,
                            StrokeThickness = 0.5,
                        });


                        if (showMiniTicksLabels)
                        {
                            var panel = new StackPanel
                            {
                                Orientation = Orientation.Horizontal,

                                Children =
                                {
                                    new TextBlock
                                    {
                                        Text = j.ToString(),
                                        Foreground = _blackBrush,
                                        FontSize = 12,
                                        Background = _whiteBrush,
                                        TextAlignment = TextAlignment.Right,
                                    },

                                }
                            };
                            panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                            var size = panel.DesiredSize;
                            YTicksCanvas.Children.Add(panel);

                            Canvas.SetLeft(panel, -size.Width - 2 - _tickHeight);
                            Canvas.SetTop(panel, y - size.Height / 2);


                        }
                    }
                }

                //YTicksCanvas.Children.Add(new Line
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

        private void SetXTicks()
        {
            XTicksCanvas.Children.RemoveRange(0, XTicksCanvas.Children.Count);
            var deltaX = MaxX - MinX;
            if (!(deltaX > 0)) return;
            var miniStepX = 10;
            var stepX = 50;
            if (deltaX > 100)
            {
                miniStepX = 20;
                stepX = 100;
            }

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
                    XTicksCanvas.Children.Add(textBlock);
                    Canvas.SetLeft(textBlock, x - size.Width / 2);
                    Canvas.SetTop(textBlock, GraphCanvas.ActualHeight + _tickHeight + 2);
                }

                XTicksCanvas.Children.Add(new Line
                {
                    X1 = x,
                    X2 = x,
                    Y1 = GraphCanvas.ActualHeight,
                    Y2 = GraphCanvas.ActualHeight + _tickHeight,
                    Stroke = _blackBrush,
                    StrokeThickness = isStepX ? 1 : .5
                });

                //XTicksCanvas.Children.Add(new Line
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
        }


        private Dictionary<double, Tuple<Point, SpectrometerDataPoint, int>[]> _dictionaryOfPointsToshow;
        private Dictionary<double, Path> _dictionaryPaths;
        private Dictionary<double, Brush> _dictionaryColors;
        private Dictionary<double, ExperimentEntityDataItemViewModel> _dictionaryViewModels;
        private object _syncObj = new object();
        private CancellationTokenSource _cts;
        private Thread _thread;
        private void UpdateView()
        {

            if (Data?.DataItems == null)// || !Data.DataItems.Any(x => x.IsShow))
                return;

            _thread?.Abort();

            _dictionaryOfPointsToshow = new Dictionary<double, Tuple<Point, SpectrometerDataPoint, int>[]>();
            _dictionaryPaths = new Dictionary<double, Path>();
            _dictionaryColors = new Dictionary<double, Brush>();
            _dictionaryViewModels = new Dictionary<double, ExperimentEntityDataItemViewModel>();
            var width = (int)GraphCanvas.ActualWidth;
            var height = (int)GraphCanvas.ActualHeight;
            var data = Data;
            var maxX = MaxX;
            var maxY = MaxY;
            var minX = MinX;
            var minY = MinY;
            _thread = new Thread(() =>
            {

                try
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        GraphCanvas.Children.RemoveRange(0, GraphCanvas.Children.Count);
                    });
                    var number = 0;
                    foreach (var dataItem in data.DataItems.OrderByDescending(x => x.IsShow))
                    {

                        ProcessDataItem(data, dataItem, width, height, maxX, maxY, minX, minY, number);
                        number++;
                    }
                }
                catch (Exception e) {}
            });
            _thread.Start();
        }

        private void ProcessDataItem(ExperimentEntityDataViewModel data, ExperimentEntityDataItemViewModel dataItem, int width,
            int height, double maxX, double maxY, double minX, double minY, int number)
        {
            if (data.WaveLengthArray.Length != dataItem.IntensityArray.Length || data.WaveLengthArray.Length < 2)
                return;
            if (_isDivideToBase)
            {
                if (dataItem.IsNoise || dataItem.IsNormalize) return;
            }


            var key = GetKey(dataItem);
            var intensityArray = dataItem.IntensityArray;
            if (_isDivideToBase)
            {
                intensityArray = (double[])intensityArray.Clone();
                for (int i = 0; i < intensityArray.Length; i++)
                {
                    intensityArray[i] /= _base[i];
                }
            }
            var pointsForShow = CalculatePoints(data.WaveLengthArray, intensityArray, width, height, maxX, maxY, minX,
                minY);


            Application.Current.Dispatcher.Invoke((Action)delegate ()
           {
               var segmentsList = new PathSegmentCollection();
               for (int i = 1; i < pointsForShow.Count(); i++)
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
                   IsClosed = false,
                   IsFilled = false
               };
               var geometry = new PathGeometry() { Figures = new PathFigureCollection() { pathFigure } };
               geometry.Freeze();
               var stroke = _brushes[number % _brushes.Length];

               var path = new Path()
               {
                   Data = geometry,

                   StrokeThickness = 1,
                   Stroke = stroke,
                   Visibility = dataItem.IsShow ? Visibility.Visible : Visibility.Hidden
               };

               GraphCanvas.Children.Add(path);
               if (_dictionaryPaths.ContainsKey(key))
               {
                   _dictionaryPaths[key] = path;
               }
               else
               {
                   _dictionaryPaths.Add(key, path);
               }
               if (_dictionaryColors.ContainsKey(key))
               {
                   _dictionaryColors[key] = stroke;
               }
               else
               {
                   _dictionaryColors.Add(key, stroke);
               }
           });
            if (_dictionaryOfPointsToshow.ContainsKey(key))
            {
                _dictionaryOfPointsToshow[key] = pointsForShow;
            }
            else
            {
                _dictionaryOfPointsToshow.Add(key, pointsForShow);
            }


            if (_dictionaryViewModels.ContainsKey(key))
            {
                _dictionaryViewModels[key] = dataItem;
            }
            else
            {
                _dictionaryViewModels.Add(key, dataItem);
            }
            //Path.Data = geometry;

        }

        private static double GetKey(ExperimentEntityDataItemViewModel dataItem)
        {
            var key = dataItem.Distance;
            if (dataItem.IsNoise)
            {
                key = -Math.PI;
            }
            if (dataItem.IsNormalize)
            {
                key = -Math.E;
            }
            return key*(dataItem.IsFiltred?-1:1);
        }
        private static string GetDescByKey(double key)
        {

            if (Math.Abs(key - (-Math.PI)) < 0.000000001)
            {
                return "Noise";
            }
            if (Math.Abs(key - (-Math.E)) < 0.000000001)
            {
                return "Normalise";
            }
            return $"L = {Math.Abs(key)}{(key>0?"":" (filtred)")}";
        }


        private double ScaleY(double y)
        {
            return _isLogScale ? Math.Log10(y == 0 ? 0.0001 : y) : y;
        }

        //todo check length of arrays
        private Tuple<Point, SpectrometerDataPoint, int>[] CalculatePoints(double[] xArray, double[] yArray, int width, int height, double maxX, double maxY, double minX, double minY)
        {
            if (xArray.Length != yArray.Length) return null;

            var deltaX = maxX - minX;

            var deltaY = maxY - minY;
            var innerMinY = minY;
            if (_isLogScale)
            {
                var logMaxY = Math.Log10(maxY);
                var logMinY = Math.Log10(minY);
                //deltaY = Math.Ceiling(logMaxY) - Math.Floor(logMinY);
                //innerMinY = (int)Math.Ceiling(logMaxY) - deltaY;
                var realMaxY = Math.Ceiling(maxY / Math.Pow(10, Math.Floor(logMaxY))) * Math.Pow(10, Math.Floor(logMaxY));
                var realMinY = Math.Ceiling(minY / Math.Pow(10, Math.Floor(logMinY))) * Math.Pow(10, Math.Floor(logMinY));

                deltaY = Math.Log10(realMaxY) - Math.Log10(realMinY);
                innerMinY = Math.Log10(realMinY);
            }

            if (!(deltaX > 0 && deltaY > 0)) return new Tuple<Point, SpectrometerDataPoint, int>[] { };
            var localMinY = .0;
            if (_isLogScale)
            {
                localMinY = yArray.Where(x => x > 0).Min();
            }
            var array = new Tuple<Point, SpectrometerDataPoint, int>[xArray.Length];
            for (int index = 0; index < xArray.Length; index++)
            {

                var x = deltaX > 0 ? Normalize(width * ((xArray[index] - minX) / deltaX), width, 1) : width / 2;
                var y = deltaY > 0 ? Normalize(height * (1 - (ScaleY(_isLogScale ? (yArray[index] <= 0 ? localMinY : yArray[index]) : yArray[index]) - (innerMinY)) / (deltaY)), height, 3) : height / 2;
                //y = ScaleY(y);
                //x = (int)Math.Round(x);
                //y = (int)Math.Round(y);
                array[index] = new Tuple<Point, SpectrometerDataPoint, int>(new Point(x, y), new SpectrometerDataPoint(xArray[index], Math.Round(yArray[index], 2)), index);
            }
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
        private static IValueConverter _descConverter = new ExperimentEntityDataItemToDesc();
        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!ShowLocator || _dictionaryOfPointsToshow == null) return;

            var mousePoint = e.GetPosition(GraphCanvas);
            double minRange = Double.MaxValue;
            //Tuple<Point, SpectrometerDataPoint, int> minValue = null;
            var rawIndex = (int)(mousePoint.X / GraphCanvas.ActualWidth * Data.WaveLengthArray.Length);



            var xIndex = -1;
            var xValue = -1.0;
            var waveLength = -1;
            var values = new List<Tuple<string, double, Brush>>();

            foreach (var tuplese in _dictionaryOfPointsToshow)
            {
                if (!_dictionaryViewModels[tuplese.Key].IsShow) continue;
                var pointsForShow = tuplese.Value;
                if (xIndex < 0)
                {
                    for (int index = Math.Max(rawIndex - 4, 0);
                        index < Math.Min(rawIndex + 4, pointsForShow.Length);
                        index++)
                    {
                        Tuple<Point, SpectrometerDataPoint, int> tuple = pointsForShow[index];
                        var range = Math.Abs(tuple.Item1.X - mousePoint.X);
                        if (range < minRange)
                        {
                            minRange = range;
                            xIndex = index;
                            xValue = tuple.Item1.X;
                        }
                    }
                }
                if (xIndex >= 0)
                {
                    values.Add(new Tuple<string, double, Brush>(GetDescByKey(tuplese.Key), tuplese.Value[xIndex].Item2.Intencity, _dictionaryColors[tuplese.Key]));
                }
                //if (minValue != null)
                //{
                //    //Tooltip.Visibility = Visibility.Visible;
                //    //DataItem = minValue.Item2;
                //    //XBlock.Text = minValue.Item2.WaveLength.ToString(CultureInfo.InvariantCulture);
                //    //YBlock.Text = minValue.Item2.Intencity.ToString(CultureInfo.InvariantCulture);
                //    //Canvas.SetLeft(Tooltip, minValue.Item1.X - 10 - Tooltip.ActualWidth);
                //    //Canvas.SetTop(Tooltip, minValue.Item1.Y - 33);


                //    VerticalLocator.Data = new LineGeometry()
                //    {
                //        StartPoint = new Point(minValue.Item1.X, 0),
                //        EndPoint = new Point(minValue.Item1.X, GraphCanvas.ActualHeight)
                //    };
                //}
                //else
                //{
                //    //Tooltip.Visibility = Visibility.Collapsed;
                //    //DataItem = new SpectrometerDataPoint();
                //    //HorisontalLocator.Data = null;
                //    VerticalLocator.Data = null;
                //    //ShowTooltip = false;
                //}
            }

            if (values.Count > 0)
            {
                TooltipPanel.Children.RemoveRange(0, TooltipPanel.Children.Count);
                TooltipPanel.Children.Add(new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                        {
                            new TextBlock
                            {
                                Text = $"{Math.Round(Data.WaveLengthArray[xIndex], 2).ToString(CultureInfo.InvariantCulture)} nm",
                                FontWeight = FontWeights.Bold
                            }
                        }
                });
                foreach (var value in values)
                {
                    TooltipPanel.Children.Add(new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = Math.Round(value.Item2, 2).ToString(CultureInfo.InvariantCulture),
                                FontWeight = FontWeights.Bold,
                                Width = 45
                            },
                            new Rectangle
                            {
                                Width = 12,
                                Height = 12,
                                Fill = value.Item3
                            },
                            new TextBlock
                            {
                                Text = $" ({value.Item1})"
                            }
                        }
                    });
                }
                VerticalLocator.Data = new LineGeometry()
                {
                    StartPoint = new Point(xValue, 0),
                    EndPoint = new Point(xValue, GraphCanvas.ActualHeight)
                };

                Canvas.SetLeft(Tooltip, xValue - 10 - Tooltip.ActualWidth);
                Canvas.SetTop(Tooltip, mousePoint.Y - 33);

                Tooltip.Visibility = Visibility.Visible;



            }
        }

        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Tooltip.Visibility = Visibility.Collapsed;
            //ShowTooltip = false;
            //HorisontalLocator.Data = null;
            VerticalLocator.Data = null;
        }

        private Brush[] _brushes =
        {
            new SolidColorBrush(Colors.Black),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.BlueViolet),
            new SolidColorBrush(Colors.Brown),
            new SolidColorBrush(Colors.DarkGreen),
            new SolidColorBrush(Colors.LawnGreen),
            new SolidColorBrush(Colors.Magenta),
            new SolidColorBrush(Colors.Olive),
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Colors.RoyalBlue),
            new SolidColorBrush(Colors.Navy),
            new SolidColorBrush(Colors.Black),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.BlueViolet),
            new SolidColorBrush(Colors.Brown),
            new SolidColorBrush(Colors.DarkGreen),
            new SolidColorBrush(Colors.LawnGreen),
            new SolidColorBrush(Colors.Magenta),
            new SolidColorBrush(Colors.Olive),
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Colors.RoyalBlue),
            new SolidColorBrush(Colors.Navy),

        };




        //////private class Wrapper
        //////{
        //////    public point
        //////}

    }


}
