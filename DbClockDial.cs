using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DbClock;
class DbClockDial : Viewbox {

    #region Canvas

    public UIElementCollection Target
        => Child is Canvas canvas
            ? canvas.Children
            : ((Child = new Canvas { Width = 600, Height = 600 }) as Canvas).Children;

    #endregion

    #region Constructor

    public DbClockDial() {
        DrawDial(TicksColor);
        DrawHourHand(HandsColor);
        DrawMinuteHand(HandsColor);
        DrawSecondHand(SecondsColor);
        DrawCentralDisc(DiscColor);
    }

    #endregion

    #region Drawing

    public void DrawDial(Color color) {
        var dial = new Ellipse {
            Width = DialDiameter,
            Height = DialDiameter,
            Fill = new SolidColorBrush(DialColor),
            RenderTransform = new TranslateTransform { X = (600 - DialDiameter) / 2, Y = (600 - DialDiameter) / 2 } };
        Target.Add(dial);
        for (int i = 0; i < 360; i += 6) {
            var isMainHour = (i % 90) == 0;
            var isSmallHour = !isMainHour && ((i % 30) == 0);
            var isMinute = !isMainHour && !isSmallHour && ((i % 6) == 0);
            var width = MainHourTickWidth;
            var height = MainHourTickHeight;
            if (isSmallHour) {
                width = SmallerHourTickWidth;
                height = SmallerHourTickHeight;
            }
            else if (isMinute) {
                width = MinuteTickWidth;
                height = MinuteTickHeight;
            }
            Target.Add(GetTickRectangle(width, height, i, color));
        }
    }

    public void DrawHourHand(Color color) {
        var rectangle = new Rectangle {
            Width = HourHandWidth,
            Height = HourHandLength,
            Fill = new SolidColorBrush(color),
            RenderTransform = new TransformGroup()
        };
        var transforms = rectangle.RenderTransform as TransformGroup;
        transforms.Children.Add(new TranslateTransform { X = 300 - HourHandWidth / 2, Y = 300 - HourHandLength });
        transforms.Children.Add(HoursRotation);
        Target.Add(rectangle);
    }

    public void DrawMinuteHand(Color color) {
        var rectangle = new Rectangle {
            Width = MinuteHandWidth,
            Height = MinuteHandLength,
            Fill = new SolidColorBrush(color),
            RenderTransform = new TransformGroup()
        };
        var transforms = rectangle.RenderTransform as TransformGroup;
        transforms.Children.Add(new TranslateTransform { X = 300 - MinuteHandWidth / 2, Y = 300 - MinuteHandLength });
        transforms.Children.Add(MinutesRotation);
        Target.Add(rectangle);
    }

    public void DrawSecondHand(Color color) {
        var cx = 300 - SecondCircleDiameter / 2.0;
        var cy = 300 - SecondCircleDiameter / 2 - SecondCircleCenterOffset;
        var cb = 300 + SecondCircleDiameter / 2 - SecondCircleCenterOffset;
        var p1 = new Point(300 - SecondHandWidth0 / 2.0, 0);
        var p2 = new Point(300 + SecondHandWidth0 / 2.0, 0);
        var p3 = new Point(300 + SecondHandWidth1 / 2.0, cy + SecondHandWidth1 / 2.0);
        var p4 = new Point(300 - SecondHandWidth1 / 2.0, cy + SecondHandWidth1 / 2.0);
        var p5 = new Point(300 - SecondHandWidth1 / 2.0, cb - SecondHandWidth1 / 2.0);
        var p6 = new Point(300 + SecondHandWidth1 / 2.0, cb - SecondHandWidth1 / 2.0);
        var p7 = new Point(300 + SecondHandWidth2 / 2.0, 300);
        var p8 = new Point(300 - SecondHandWidth2 / 2.0, 300);
        var poly1 = new Polygon();
        poly1.Points.Add(p1);
        poly1.Points.Add(p2);
        poly1.Points.Add(p3);
        poly1.Points.Add(p4);
        poly1.Fill = new SolidColorBrush(color);
        var poly2 = new Polygon();
        poly2.Points.Add(p5);
        poly2.Points.Add(p6);
        poly2.Points.Add(p7);
        poly2.Points.Add(p8);
        poly2.Fill = new SolidColorBrush(color);
        var circle = new Ellipse { Width = SecondCircleDiameter, Height = SecondCircleDiameter, Stroke = new SolidColorBrush(color), StrokeThickness = SecondHandWidth1 };
        poly1.RenderTransform = SecondsRotation;
        var circleTransform = new TransformGroup();
        circleTransform.Children.Add(new TranslateTransform { X = cx, Y = cy });
        circleTransform.Children.Add(SecondsRotation);
        circle.RenderTransform = circleTransform;
        poly2.RenderTransform = SecondsRotation;
        Target.Add(poly1);
        Target.Add(circle);
        Target.Add(poly2);
    }

    public void DrawCentralDisc(Color color) {
        var disc = new Ellipse {
            Width = CentralDiscDiameter,
            Height = CentralDiscDiameter,
            Fill = new SolidColorBrush(color),
            RenderTransform = new TranslateTransform {
                X = 300 - CentralDiscDiameter / 2.0,
                Y = 300 - CentralDiscDiameter / 2.0
            }
        };
        Target.Add(disc);
    }

    static Rectangle GetTickRectangle(double width, double height, double angle, Color color) {
        var rectangle = new Rectangle {
            Width = width,
            Height = height,
            Fill = new SolidColorBrush(color),
            RenderTransform = new TransformGroup()
        };
        var transformGroup = rectangle.RenderTransform as TransformGroup;
        transformGroup.Children.Add(new TranslateTransform { X = 300.0 - (width / 2.0), Y = 0.0 });
        transformGroup.Children.Add(new RotateTransform { Angle = angle, CenterX = 300.0, CenterY = 300.0 });
        return rectangle;
    }

    #endregion

    #region Dependency properties

    public static readonly DependencyProperty TicksColorProperty = DependencyProperty.Register(nameof(TicksColor), typeof(Color), typeof(DbClockDial), new PropertyMetadata {
        DefaultValue = ColorConverter.ConvertFromString("#111")
    });

    public static readonly DependencyProperty HandsColorProperty = DependencyProperty.Register(nameof(HandsColor), typeof(Color), typeof(DbClockDial), new PropertyMetadata {
        DefaultValue = ColorConverter.ConvertFromString("#222")
    });

    public static readonly DependencyProperty SecondsColorProperty = DependencyProperty.Register(nameof(SecondsColor), typeof(Color), typeof(DbClockDial), new PropertyMetadata {
        DefaultValue = ColorConverter.ConvertFromString("#c00")
    });

    public static readonly DependencyProperty DiscColorProperty = DependencyProperty.Register(nameof(DiscColor), typeof(Color), typeof(DbClockDial), new PropertyMetadata {
        DefaultValue = ColorConverter.ConvertFromString("#333")
    });

    public static readonly DependencyProperty DialColorProperty = DependencyProperty.Register(nameof(DialColor), typeof(Color), typeof(DbClockDial), new PropertyMetadata {
        DefaultValue = ColorConverter.ConvertFromString("#beee")
    });

    public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(nameof(Time), typeof(DateTime), typeof(DbClockDial), new PropertyMetadata {
        PropertyChangedCallback = new PropertyChangedCallback(OnTimeChanged)
    });

    static void OnTimeChanged(DependencyObject @object, DependencyPropertyChangedEventArgs e) {
        var view = @object as DbClockDial;
        var time = (DateTime)e.NewValue;
        if (time != (DateTime)e.OldValue) {
            var tod = time.TimeOfDay;
            var hours = (double)tod.Hours + (tod.TotalHours - Math.Floor(tod.TotalHours));
            var minutes = (double)tod.Minutes + (tod.TotalMinutes - Math.Floor(tod.TotalMinutes));
            var seconds = (double)tod.Seconds + (tod.TotalSeconds - Math.Floor(tod.TotalSeconds));
            view.HoursRotation.Angle = hours * 30.0;
            view.MinutesRotation.Angle = minutes * 6.0;
            view.SecondsRotation.Angle = seconds * 6.0;
        }
    }

    public Color DialColor { get => (Color)GetValue(DialColorProperty); set => SetValue(DialColorProperty, value); }

    public Color TicksColor { get => (Color)GetValue(TicksColorProperty); set => SetValue(TicksColorProperty, value); }

    public Color HandsColor { get => (Color)GetValue(HandsColorProperty); set => SetValue(HandsColorProperty, value); }

    public Color SecondsColor { get => (Color)GetValue(SecondsColorProperty); set => SetValue(SecondsColorProperty, value); }

    public Color DiscColor { get => (Color)GetValue(DiscColorProperty); set => SetValue(DiscColorProperty, value); }

    public DateTime Time { get => (DateTime)GetValue(TimeProperty); set => SetValue(TimeProperty, value); }

    #endregion

    #region Dimensions

    // dimmensions for 600 units for tickcs outer diameter

    public const double DialDiameter = 632;
    public const double MainHourTickWidth = 24;
    public const double MainHourTickHeight = 94;
    public const double SmallerHourTickWidth = 24;
    public const double SmallerHourTickHeight = 74;
    public const double MinuteTickWidth = 10;
    public const double MinuteTickHeight = 24;
    public const double MinuteHandLength = 284;
    public const double MinuteHandWidth = 24;
    public const double HourHandLength = 184;
    public const double HourHandWidth = 34;
    public const double CentralDiscDiameter = 64;
    public const double SecondCircleCenterOffset = 174;
    public const double SecondCircleDiameter = 60;
    public const double SecondHandWidth0 = 4;
    public const double SecondHandWidth1 = 8;
    public const double SecondHandWidth2 = 16;

    #endregion

    #region Rotations

    readonly RotateTransform HoursRotation = new() { CenterX = 300, CenterY = 300 };
    readonly RotateTransform MinutesRotation = new() { CenterX = 300, CenterY = 300 };
    readonly RotateTransform SecondsRotation = new() { CenterX = 300, CenterY = 300 };

    #endregion

}