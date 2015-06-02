using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

/*namespace TabletopBillboardApplication
{
    /// <summary>
    /// Interaction logic for MyUserControl.xaml
    /// </summary>
    public partial class MyUserControl : UserControl
    {
        public MyUserControl()
        {
            InitializeComponent();
        }
    }
}*/

namespace TabletopBillboardApplication
{
    /// <summary>
    /// Interaction logic for MyUserControl.xaml
    /// </summary>
    public partial class MyUserControl : UserControl
    {
        public delegate void TimeValueSelectedEventHandler(double timeValue);
        public event TimeValueSelectedEventHandler timeValueSelectedHandler = null;
        public MyUserControl()
        {
            InitializeComponent();
        }

        private bool _isPressed = false;
        private Canvas _templateCanvas = null;

        private void Ellipse_MouseLeftButtonDown(object sender, TouchEventArgs e)//MouseButtonEventArgs e)
        {
            //Enable moving mouse to change the value.
            //Debug.WriteLine(String.Concat("Button Down Works Now",e));
            _isPressed = true;
        }

        private void Ellipse_MouseLeftButtonUp(object sender, TouchEventArgs e)
        {
            //Disable moving mouse to change the value.
            //Debug.WriteLine(String.Concat("Button Up Works Now", e));
            _isPressed = false;
        }

        private void Ellipse_MouseMove(object sender, TouchEventArgs e)
        {
            if (_isPressed)
            {
                //Find the parent canvas.
                if (_templateCanvas == null)
                {
                    _templateCanvas = MyHelper.FindParent<Canvas>(e.Source as Ellipse);
                    if (_templateCanvas == null) return;
                }
                //Canculate the current rotation angle and set the value.
                const double RADIUS = 150;
                
                TouchPoint newPosition = e.GetTouchPoint(_templateCanvas);
                Point newPos = new Point(newPosition.Position.X, newPosition.Position.Y);
                //Debug.WriteLine(String.Concat(newPos));
                double angle = MyHelper.GetAngleR(newPos, RADIUS);

                double angle_degree = 180 * angle / Math.PI;

                knob.Value = (knob.Maximum - knob.Minimum) * angle / (2 * Math.PI);
                if (timeValueSelectedHandler != null)
                {
                    timeValueSelectedHandler(knob.Value);
                }
            }
        }
    }

    //The converter used to convert the value to the rotation angle.
    public class ValueAngleConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter,
                      System.Globalization.CultureInfo culture)
        {
            double value = (double)values[0];
            double minimum = (double)values[1];
            double maximum = (double)values[2];

            return MyHelper.GetAngle(value, maximum, minimum);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
              System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    //Convert the value to text.
    public class ValueTextConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
                  System.Globalization.CultureInfo culture)
        {
            double v = (double)value;
            return String.Format("{0:F2}", v);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public static class MyHelper
    {
        //Get the parent of an item.
        public static T FindParent<T>(FrameworkElement current)
          where T : FrameworkElement
        {
            do
            {
                current = VisualTreeHelper.GetParent(current) as FrameworkElement;
                if (current is T)
                {
                    return (T)current;
                }
            }
            while (current != null);
            return null;
        }

        //Get the rotation angle from the value
        public static double GetAngle(double value, double maximum, double minimum)
        {
            double current = (value / (maximum - minimum)) * 360;
            if (current == 360)
                current = 359.999;
            if (current >= 90 & current <= 180) { 
                current = 90; }
            if (current >= 180 & current <= 270){ current = 270;}

            return current;
        }

        //Get the rotation angle from the position of the mouse
        public static double GetAngleR(Point pos, double radius)
        {
            //Calculate out the distance(r) between the center and the position
            Point center = new Point(radius, radius);
            double xDiff = center.X - pos.X;
            double yDiff = center.Y - pos.Y;
            double r = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);

            //Calculate the angle
            double angle = Math.Acos((center.Y - pos.Y) / r);
            if (pos.X < radius)
            {
                angle = 2 * Math.PI - angle;
            }
            if (Double.IsNaN(angle))
                return 0.0;
            else
                return angle;
        }
    }
}
