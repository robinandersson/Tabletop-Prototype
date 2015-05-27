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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.IO;

namespace TabletopBillboardApplication
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        private Random rnd;
        private ScatterViewItem svi;
        public SurfaceWindow1()
        {
            InitializeComponent();
            ScatterViewItem item = new ScatterViewItem();
            item.Height = 200;
            item.Width = 1000;

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            
            rnd = new Random();
            // load image
            LoadImages();

           // string envDir = Environment.CurrentDirectory;
            //string[] fileNames = Directory.GetFiles(envDir + @"\Resources", "*.jpg");
            //scatter.ItemsSource = fileNames;
               // Directory.GetFiles(@"C:\Users\Public\Pictures\Sample Pictures", "*.jpg");

        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        //void c_contentHolder_

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }
        
        // load images from source
        void LoadImages()
        {
            string envDir = Environment.CurrentDirectory;
            string[] fileNames = Directory.GetFiles(envDir+@"\Resources", "*.jpg");
            //scatter.ItemsSource = fileNames;
            //foreach (object obj in scatter.Items)
            //{
            //    ScatterViewItem svi = scatter.ItemContainerGenerator.ContainerFromItem(obj) as ScatterViewItem;
            //    int dice = rnd.Next(1, 6);
            //    svi.Width = 100 * dice;
            //    svi.Height = 100*dice;
            //    scatter.UpdateLayout();
            //}
            foreach (string name in fileNames)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(name, UriKind.Absolute));
                // create random size for image
                int dice = rnd.Next(1,6);

                double width = img.Source.Width*dice*0.1;
                double height = img.Source.Height*dice*0.1;
                
                
                img.Tag = "this image 1";
                svi = new ScatterViewItem();
                svi.Width = width;
                svi.Height = height;
                svi.Content = img;
                svi.PreviewTouchDown += new EventHandler<TouchEventArgs>(img_PreviewTouchDown);
                scatter.Items.Add(svi);
            }
        }

        void img_PreviewTouchDown(object sender, TouchEventArgs e)
        {

            var element = sender as ContentControl;
            //sender. = Visibility.Hidden;
            Double x = 0;
            Double y = 0;
            if (element != null)
            {
                var location = element.PointToScreen(new Point(0, 0));
                x = location.X;
                y = location.Y;

            }
            ScatterViewItem item = (ScatterViewItem)sender;
            Image img  = (Image)item.Content;
            item.Visibility = Visibility.Hidden;

            svi = new ScatterViewItem();
            //string name = (string)img.Tag;
            //MessageBox.Show(name);
            Image img1 = new Image();
            img1.Source = img.Source.Clone();
            Canvas can = new Canvas();
            ImageBrush ib = new ImageBrush();
            String dir = Environment.CurrentDirectory;
            String imagePath = dir+ @"\Resources\background\plain-hd-wallpapers.jpg";
            ib.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            can.Background = ib;
            
            Label lab1 = new Label();
            lab1.Content = "Description of this image";
            
            Canvas.SetRight(img1,20);
            Canvas.SetLeft(img1, 20);
            Canvas.SetTop(img1, 20);
            Canvas.SetBottom(img1, 20);
            can.Children.Add(img1);

            Canvas.SetRight(lab1, 100);
            Canvas.SetLeft(lab1, 100);
            Canvas.SetTop(lab1, 100);
            Canvas.SetBottom(lab1, 100);
            can.Children.Add(lab1);
            can.Width = 500;
            can.Height = 500;
            svi.Content = can;
            svi.Center = new Point(x+100,y+100);
            //svi.Width = img1.Width;
            //svi.Height = img1.Height*2;

           
            scatter.Items.Add(svi);
           
            //foreach (Object obj in scatter.Items){
            //    ScatterViewItem sv = scatter.ItemContainerGenerator.ContainerFromItem(obj) as ScatterViewItem;
            //    if (sv.Content.GetType().Equals(typeof(Canvas)))
            //    {
            //        scatter.Items.RemoveAt(1);
            //    }
            //}
            //scatter.Items.RemoveAt(1);
        }

        private void OnItemClicked(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked and hide it.
            Button b = (Button)e.Source as Button;
            b.Visibility = Visibility.Collapsed;

            // Get the ScatterViewItem control for the clicked button.
            ScatterViewItem item = (ScatterViewItem)scatter.ContainerFromElement(b);

            // Get the image within the ScatterViewItemcontrol.
            System.Windows.Controls.ContentPresenter content = FindContentPresenter(item);
            System.Windows.Controls.Image img =
             (System.Windows.Controls.Image)content.ContentTemplate.FindName("img", content);

            // Convert the image to grayscale.
            img.Source = new FormatConvertedBitmap(
             (BitmapSource)img.Source, PixelFormats.Gray16, BitmapPalettes.Gray16, 0);
        }

        System.Windows.Controls.ContentPresenter FindContentPresenter(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is System.Windows.Controls.ContentPresenter)
                {
                    return (System.Windows.Controls.ContentPresenter)child;
                }
                else
                {
                    System.Windows.Controls.ContentPresenter childOfChild =
                        FindContentPresenter(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

    }
}