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
        private ScatterViewItem svi;
        //private ScatterView scatter;
        List<EventData> events;
        private List<String> tagsOnSurface = new List<String>(); 
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            // Add handlers for Tags
            InitializeDefinitions();

            //scatter = new ScatterView();

            //screenHolder.Content = scatter;

            events = new List<EventData>();
            // load text
            LoadText(events);
            setSize(events);
            // load image
            LoadImages(events);

            foreach (object obj in scatter.Items)
            {
                ScatterViewItem svi = scatter.ItemContainerGenerator.ContainerFromItem(obj) as ScatterViewItem;
            }

        }

        private void setSize(List<EventData> events)
        {
            int num = 0;
            DateTime today = DateTime.Today;
            DateTime tempToday = DateTime.Today;
            int length = events.Count;
            while (length > num+1){
                DateTime EventDay = events.ElementAt(num).getDate();
                int size = 2; //dice 2, further than a month away
                int compare = DateTime.Compare(EventDay, today);
                if (compare == 0) { // Events of today
                    size = 6;
                }
                else {
                    if(compare < 0) {   // posters from the past
                        size = 1;
                    }
                    else {              //dice 4, within 10 days
                        tempToday = DateTime.Today; 
                        tempToday = tempToday.AddDays(10);
                        if (DateTime.Compare(EventDay, tempToday) < 0) {
                            size = 4;
                        }
                        else {          //dice 3, further than 10 days, but within a month 
                            tempToday = DateTime.Today;
                            tempToday = tempToday.AddDays(30);
                            if (DateTime.Compare(EventDay, tempToday) < 0){
                                size = 3;
                            }
                        }
                    }
                }
                 events[num].setSize(size);
                 num++;
            }
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
            //ManipulationDelta += OnManipulationDelta;
        }

        // maximize zooming in
        /*public void OnManipulationDelta(object sender, ManipulationDeltaEventArgs args)
        {
            if (sender.GetType() == typeof(ScatterViewItem))
            {
                ScatterViewItem name = (ScatterViewItem) (sender);

                if (name.ActualHeight > 700 || name.ActualWidth > 700)
                {
                    bool W = name.ActualHeight == Math.Max(name.ActualHeight, name.ActualWidth);
                    if (W)
                    {
                        name.Height = 700;
                    }
                    else
                    {
                        name.Width = 700;
                    }
                }
            }
            //svi.MaxHeight = 700;
            //svi.MaxWidth = 700;
        }*/

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
        void LoadImages(List<EventData> events)
        {
            string envDir = Environment.CurrentDirectory;
            string[] fileNames = Directory.GetFiles(envDir+@"\Resources\Posters", "*.jpg");
            int num = 0;
            foreach (string name in fileNames)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(name, UriKind.Absolute));
                int size = events.ElementAt(num).getSize();
                img.Tag = events.ElementAt(num);
                                
                svi = new ScatterViewItem();
                svi.Tag = events.ElementAt(num);
                svi.Content = img;
                int scale = (int)(100 * (img.Source.Width) / img.Source.Height);
                svi.Height = 100 * size;
                svi.Width = scale * size;
                if (svi.Width > svi.Height)
                {
                    scale = (int)(100 * (img.Source.Height) / img.Source.Width);
                    svi.Width = 100 * size;
                    svi.Height = scale * size;
                }


                svi.AddHandler(TouchExtensions.TapGestureEvent, new RoutedEventHandler(img_PreviewTouchDown), true);
                
                //svi.PreviewTouchDown += new EventHandler<TouchEventArgs>(img_PreviewTouchDown);
                
                
                scatter.Items.Add(svi);
                num++;
            }
        }

        void img_PreviewTouchDown(object sender, RoutedEventArgs e)
        {
            //TouchDevice c = e.;
            
            //if (c.GetIsFingerRecognized() == true)
            //{
                var element = sender as ContentControl;
                Double x = 0;
                Double y = 0;
                if (element != null)
                {
                    var location = element.PointToScreen(new Point(0, 0));
                    x = location.X;
                    y = location.Y;
                }

                ScatterViewItem item = (ScatterViewItem)sender;
                item.Visibility = Visibility.Hidden;
                Image img = (Image)item.Content;

                EventData eventData = (EventData)img.Tag;

                svi = new ScatterViewItem();
                Image img1 = new Image();
                img1.Source = img.Source.Clone();

                Canvas canvas = new Canvas();
                //SetPosterBackground(canvas);

                ScatterViewItem addi = new ScatterViewItem();
                SetScatterView(addi, img1);
                canvas.Children.Add(addi);
                Button closeButton = new Button();
                
                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Vertical,

                    Children =
                {
                    closeButton,
                    new Label
                    {
                        Content = "   ",
                        
                    },
                    new Label
                    {
                        Content = eventData.getName(),
                        Foreground = System.Windows.Media.Brushes.Black,
                    },
                    new Border
                    {
                        Height = 2,
                        Background = Brushes.Yellow, // Set background here
                    },

                    new Label
                    {
                        Content = "Data: "+ eventData.getDate().ToString(),
                        Foreground = System.Windows.Media.Brushes.Black,
                    },

                    new Label 
                    {                       
                        Content = eventData.getText(),
                        Foreground = System.Windows.Media.Brushes.Black,
                        //Background = Brushes.Pink, // Set background here
                    },
                    
                }
                };

                svi.Content = canvas;
                if (addi.Width > addi.Height)
                {
                    svi.Width = addi.Width;
                    svi.Height = addi.Height * 2;
                    sp.Width = addi.Width;
                    sp.Height = addi.Height * 2;
                    Canvas.SetTop(sp, addi.Height);
                }
                else
                {
                    svi.Width = addi.Width * 2;
                    svi.Height = addi.Height;
                    sp.Width = addi.Width;
                    sp.Height = addi.Height * 2;
                    Canvas.SetLeft(sp, addi.Width);
                }

               
                canvas.Children.Add(sp);
                svi.Center = new Point(x, y);

                
                
                String dir = Environment.CurrentDirectory;

                ScatterViewItem a = new ScatterViewItem();
                a.Center = new Point(x - svi.ActualWidth / 2 - 300, y + svi.ActualHeight + 300);              
                String imagePath1 = dir + @"\Resources\More-images\image 1.jpg";
                Image img_1 = new Image();
                img_1.Source = new BitmapImage(new Uri(imagePath1, UriKind.Absolute));
                SetScatterView1(a, img_1);
                Line line = new Line { Stroke = Brushes.Black, StrokeThickness = 2.0 };
                BindLineToScatterViewItems(line, svi, a);

                ScatterViewItem b = new ScatterViewItem();
                b.Center = new Point(x - svi.ActualWidth / 2 - 300, y - svi.ActualHeight - 300);
                b.Width = 200;
                b.Height = 200;
                String imagePath2 = dir + @"\Resources\More-images\image 2.jpg";
                Image img_2 = new Image();
                img_2.Source = new BitmapImage(new Uri(imagePath2, UriKind.Absolute));
                SetScatterView1(b, img_2);
                Line line2 = new Line { Stroke = Brushes.Black, StrokeThickness = 2.0 };
                BindLineToScatterViewItems(line2, svi, b);

                ScatterViewItem c1 = new ScatterViewItem();
                c1.Center = new Point(x + svi.ActualWidth / 2 + 300, y + svi.ActualHeight + 300);
                c1.Width = 200;
                c1.Height = 200;
                String imagePath3 = dir + @"\Resources\More-images\image 3.jpg";
                Image img_3 = new Image();
                img_3.Source = new BitmapImage(new Uri(imagePath3, UriKind.Absolute)); 
                SetScatterView1(c1, img_3);
                Line line3 = new Line { Stroke = Brushes.Black, StrokeThickness = 2.0 };
                BindLineToScatterViewItems(line3, svi, c1);

                ScatterViewItem d = new ScatterViewItem();
                d.Center = new Point(x + svi.ActualWidth / 2 + 300, y - svi.ActualHeight - 300);
                d.Width = 200;
                d.Height = 200;
                String imagePath4 = dir + @"\Resources\More-images\image 4.jpg";
                Image img_4 = new Image();
                img_4.Source = new BitmapImage(new Uri(imagePath4, UriKind.Absolute));
                SetScatterView1(d, img_4);
                Line line4 = new Line { Stroke = Brushes.Black, StrokeThickness = 2.0 };
                BindLineToScatterViewItems(line4, svi, d);

                SetCloseButton(closeButton,line, line2, line3, line4, svi, item, a, b, c1, d);
               
                scatter.Items.Add(svi);
                scatter.Items.Add(a);
                scatter.Items.Add(b);
                scatter.Items.Add(c1);
                scatter.Items.Add(d);

                LineHost.Children.Add(line);
                LineHost.Children.Add(line2);
                LineHost.Children.Add(line3);
                LineHost.Children.Add(line4);
            
        }

        private void BindLineToScatterViewItems(Line line, ScatterViewItem origin,
    ScatterViewItem destination)
        {
            // Bind line.(X1,Y1) to origin.ActualCenter  
            BindingOperations.SetBinding(line, Line.X1Property, new Binding
            {
                Source = origin,
                Path = new PropertyPath("ActualCenter.X")
            });
            BindingOperations.SetBinding(line, Line.Y1Property, new Binding
            {
                Source = origin,
                Path = new PropertyPath("ActualCenter.Y")
            });

            // Bind line.(X2,Y2) to destination.ActualCenter  
            BindingOperations.SetBinding(line, Line.X2Property, new Binding
            {
                Source = destination,
                Path = new PropertyPath("ActualCenter.X")
            });
            BindingOperations.SetBinding(line, Line.Y2Property, new Binding
            {
                Source = destination,
                Path = new PropertyPath("ActualCenter.Y")
            });
        }

        void btn_CloseClick(object sender, RoutedEventArgs e)
        {
            Button b = (Button) sender;
            List<Object> scas = (List<Object>)b.Tag;
            Line a = (Line)scas.ElementAt(0);
            a.Visibility = Visibility.Hidden;
            LineHost.Children.Remove(a);
            Line a1 = (Line)scas.ElementAt(1);
            a1.Visibility = Visibility.Hidden;
            LineHost.Children.Remove(a1);
            Line a2 = (Line)scas.ElementAt(2);
            a2.Visibility = Visibility.Hidden;
            LineHost.Children.Remove(a2);
            Line a3 = (Line)scas.ElementAt(3);
            a3.Visibility = Visibility.Hidden;
            LineHost.Children.Remove(a3);

            ScatterViewItem s1 = (ScatterViewItem)scas.ElementAt(4);
            s1.Visibility = Visibility.Hidden;
            scatter.Items.Remove(s1);
            ScatterViewItem s2 = (ScatterViewItem)scas.ElementAt(5);
            s2.Visibility = Visibility.Visible;

            ScatterViewItem s3 = (ScatterViewItem)scas.ElementAt(6);
            s3.Visibility = Visibility.Hidden;
            scatter.Items.Remove(s3);

            ScatterViewItem s4 = (ScatterViewItem)scas.ElementAt(7);
            s4.Visibility = Visibility.Hidden;
            scatter.Items.Remove(s4);

            ScatterViewItem s5 = (ScatterViewItem)scas.ElementAt(8);
            s5.Visibility = Visibility.Hidden;
            scatter.Items.Remove(s5);

            ScatterViewItem s6 = (ScatterViewItem)scas.ElementAt(9);
            s6.Visibility = Visibility.Hidden;
            scatter.Items.Remove(s6);

        }

        void SetCloseButton(Button closeButton, Line l, Line l2, Line l3, Line l4, ScatterViewItem svi, ScatterViewItem item, 
                           ScatterViewItem a,  ScatterViewItem b,  ScatterViewItem c, ScatterViewItem d)
        {
            closeButton.Content = "close";

            List<Object> scas = new List<Object>();
            scas.Add(l);
            scas.Add(l2);
            scas.Add(l3);
            scas.Add(l4);
            scas.Add(svi);
            scas.Add(item);
            scas.Add(a);
            scas.Add(b);
            scas.Add(c);
            scas.Add(d);
            closeButton.Tag = scas;
            //closeButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(btn_CloseClick));
            closeButton.PreviewTouchDown += new EventHandler<TouchEventArgs>(btn_CloseClick);
        }

        void SetScatterView(ScatterViewItem addi, Image img)
        {
            int scale = (int)(100 * (img.Source.Width) / img.Source.Height);
            addi.Height = 100 * 4;
            addi.Width = scale * 4;
            addi.Content = img;
        }

        void SetScatterView1(ScatterViewItem addi, Image img)
        {
            int scale = (int)(100 * (img.Source.Width) / img.Source.Height);
            addi.Height = 100 * 2;
            addi.Width = scale * 2;
            addi.Content = img;
        }


        void SetPosterBackground(Canvas canvas)
        {
            ImageBrush ib = new ImageBrush();
            String dir = Environment.CurrentDirectory;
            String imagePath = dir + @"\Resources\background\plain-hd-wallpapers.jpg";
            ib.ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            canvas.Background = ib;
        }


        void OnPosterTap(object sender, RoutedEventArgs e)
        {

            ScatterViewItem svi;

            // Find ScatterViewItem parent of image (object of type System.Windows.Controls.Image returned as source normally)
            if (e.Source.GetType().Equals(typeof(Image)))
            {
                DependencyObject parent = LogicalTreeHelper.GetParent(e.Source as Image);
                svi = parent as ScatterViewItem;
            } 
            else
            {
                svi = e.Source as ScatterViewItem;
            }

            // Handle click event
            /*if (svi != null) OLD CODE
            {

                // Change screen upon click on poster
                ScatterView newScatter = new ScatterView();

                ScatterViewItem item = new ScatterViewItem();

                Label label = new Label();
                label.Content = "New Screen! :D";

                item.Content = label;
                newScatter.Items.Add(item);

                //screenHolder.Content = newScatter;
            }*/
                    
            return;

        }

        // load text from source
        private List<EventData> LoadText(List<EventData> events)
        {
            try
            {
                string path = Environment.CurrentDirectory + @"\Resources\Text Posters\TextImage.txt";
                using (StreamReader sr = new StreamReader(path))
                {
                    String name = sr.ReadLine();
                    while (name != null)
                    {
                        String dat = sr.ReadLine();
                        DateTime date = DateTime.ParseExact(dat, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        List<String> tag= new List<String>();
                        String tag0 = sr.ReadLine();
                        while (tag0 != "$")
                        {
                            tag.Add(tag0);
                            tag0 = sr.ReadLine();
                        }
                        String line0 = sr.ReadLine();
                        String line = null;
                        while (line0 != "$")
                        {
                            line = String.Concat(line,line0);
                            line0 = sr.ReadLine();
                        }
                        events.Add(new EventData(name, date, tag, line));
                        name = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return events;
        }

        // class to ask for data per image
        private class EventData
        {
            private List<String> type;
            private String text, name;
            private DateTime date;
            private int size = 0;

            public EventData(String name, DateTime date, List<String> type, String text){
                this.name = name;
                this.date = date;
                this.type = type;
                this.text = text;
            }

            public string getName(){
                return name;
            }

            public DateTime getDate(){
                return date;
            }

            public List<string> getTags(){
                return type;
            }

            public string getText(){
                return text;
            }

            public int getSize() {
                return size;
            }
            public void setSize(int size) {
                this.size = size;
            }
        }

        private void InitializeDefinitions()
        {
            for (byte k = 1; k <= 5; k++)
            {
                TagVisualizationDefinition tagDef =
                    new TagVisualizationDefinition();
                // The tag value that this definition will respond to.
                tagDef.Value = k;
                // The .xaml file for the UI
                tagDef.Source =
                    new Uri("TagVisualization1.xaml", UriKind.Relative);
                // The maximum number for this tag value.
                tagDef.MaxCount = 1;
                // The visualization stays for 0,5 seconds.
                tagDef.LostTagTimeout = 500.0;
                // Orientation offset (default).
                tagDef.OrientationOffsetFromTag = 0.0;
                // Physical offset (horizontal inches, vertical inches).
                tagDef.PhysicalCenterOffsetFromTag = new Vector(1.0, 1.0);
                // Tag removal behavior (default).
                tagDef.TagRemovedBehavior = TagRemovedBehavior.Fade;
                // Orient UI to tag? (default).
                tagDef.UsesTagOrientation = true;
                // Add the definition to the collection.
                MyTagVisualizer.Definitions.Add(tagDef);
            }
        }

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            TagVisualization1 tag = (TagVisualization1)e.TagVisualization;

            switch (tag.VisualizedTag.Value)
            {
                case 1:
                    tagsOnSurface.Add("music");
                    tag.EventType.Content = "Music";
                    break;
                case 2:
                    tagsOnSurface.Add("sport");
                    tag.EventType.Content = "Sport";
                    break;
                case 3:
                    tag.EventType.Content = "No type yet";
                    break;
                case 4:
                    tag.EventType.Content = "No type yet";
                    break;
                default:
                    tag.EventType.Content = "Not a valid tag";
                    break;

            }

            sortOutEvents();

        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            TagVisualization1 tag = (TagVisualization1)e.TagVisualization;

            switch (tag.VisualizedTag.Value)
            {
                case 1:
                    tagsOnSurface.Remove("music");
                    break;
                case 2:
                    tagsOnSurface.Remove("sport");
                    break;
                case 3:
                    break;
                case 4:
                    break;
                default:
                    break;

            }

            sortOutEvents();
        }

        private void sortOutEvents()
        {
            // If no tags on the surface - display all events
            if (tagsOnSurface.Count == 0)
            {
                foreach (ScatterViewItem svi in scatter.Items)
                {
                    svi.Visibility = Visibility.Visible;
                }


            }else
            {

                // display the events that matches a tag
                foreach (ScatterViewItem svi in scatter.Items)
                {
                    EventData eventData = svi.Tag as EventData;
                    bool tagPresent = false;
                    foreach (string eventTag in eventData.getTags())
                    {
                        foreach (string tag in tagsOnSurface)
                        {
                            if (eventTag.Equals(tag))
                            {
                                tagPresent = true;
                                svi.Visibility = Visibility.Visible;
                                break;
                            }
                        }

                    }

                    if (!tagPresent)
                    {
                        svi.Visibility = Visibility.Hidden;
                    }

                }

            }

            
        }
    }
}