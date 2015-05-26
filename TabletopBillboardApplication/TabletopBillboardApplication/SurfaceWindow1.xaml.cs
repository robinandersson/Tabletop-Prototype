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
        private ScatterView scatter;
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add defenitions for tags
            InitializeDefinitions();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            scatter = new ScatterView();

            screenHolder.Content = scatter;
            
            // load text
            List<EventData> data = new List<EventData>();
            LoadText(data);
            setSize(data);
            // load image
            LoadImages(data);

            foreach (object obj in scatter.Items)
            {
                ScatterViewItem svi = scatter.ItemContainerGenerator.ContainerFromItem(obj) as ScatterViewItem;
            }

        }

        private void setSize(List<EventData> data)
        {
            int num = 0;
            DateTime today = DateTime.Today;
            DateTime tempToday = DateTime.Today;
            int length = data.Count;
            while (length > num+1){
                DateTime posterDay = data.ElementAt(num).getDate();
                int dice = 2; //dice 2, further than a month away
                int compare = DateTime.Compare(posterDay, today);
                if (compare == 0) { // posters of today
                    dice = 6;
                }
                else {
                    if(compare < 0) {   // posters from the past
                        dice = 1;
                    }
                    else {              //dice 4, within 10 days
                        tempToday = DateTime.Today; 
                        tempToday = tempToday.AddDays(10);
                        if (DateTime.Compare(posterDay, tempToday) < 0) {
                            dice = 4;
                        }
                        else {          //dice 3, further than 10 days, but within a month 
                            tempToday = DateTime.Today;
                            tempToday = tempToday.AddDays(30);
                            if (DateTime.Compare(posterDay, tempToday) < 0){
                                dice = 3;
                            }
                        }
                    }
                }
                 data[num].setSize(dice);
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
        void LoadImages(List<EventData> data)
        {
            string envDir = Environment.CurrentDirectory;
            string[] fileNames = Directory.GetFiles(envDir+@"\Resources\Posters", "*.jpg");
            int num2 = 0;
            foreach (string name in fileNames)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(name, UriKind.Absolute));
                int size = data.ElementAt(num2).getSize();
                img.Tag = data.ElementAt(num2);
                                
                svi = new ScatterViewItem();
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

                svi.AddHandler(TouchExtensions.TapGestureEvent, new RoutedEventHandler(OnPosterTap), true);
                scatter.Items.Add(svi);
                num2++;
            }
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
            if (svi != null)
            {

                // Change screen upon click on poster
                ScatterView newScatter = new ScatterView();

                ScatterViewItem item = new ScatterViewItem();

                Label label = new Label();
                label.Content = "New Screen! :D";

                item.Content = label;
                newScatter.Items.Add(item);

                screenHolder.Content = newScatter;
            }
                    
            return;

        }

        // load text from source
        private List<EventData> LoadText(List<EventData> data)
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
                        data.Add(new EventData(name, date, tag, line));
                        name = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return data;
        }

        // class to ask for data per image
        private class EventData
        {
            private List<String> tags;
            private String text, name;
            private DateTime date;
            private int size = 0;

            public EventData(String name, DateTime date, List<String> tags, String text){
                this.name = name;
                this.date = date;
                this.tags = tags;
                this.text = text;
            }

            public string getName(){
                return name;
            }

            public DateTime getDate(){
                return date;
            }

            public List<string> getTags(){
                return tags;
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
                tagDef.MaxCount = 2;
                // The visualization stays for 2 seconds.
                tagDef.LostTagTimeout = 2000.0;
                // Orientation offset (default).
                tagDef.OrientationOffsetFromTag = 0.0;
                // Physical offset (horizontal inches, vertical inches).
                tagDef.PhysicalCenterOffsetFromTag = new Vector(2.0, 2.0);
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
                    tag.CameraModel.Content = "Music events, Inc. ABC-12";
                    tag.myEllipse.Fill = SurfaceColors.Accent1Brush;
                    break;
                case 2:
                    tag.CameraModel.Content = "Fabrikam, Inc. DEF-34";
                    tag.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    break;
                case 3:
                    tag.CameraModel.Content = "Fabrikam, Inc. GHI-56";
                    tag.myEllipse.Fill = SurfaceColors.Accent3Brush;
                    break;
                case 4:
                    tag.CameraModel.Content = "Fabrikam, Inc. JKL-78";
                    tag.myEllipse.Fill = SurfaceColors.Accent4Brush;
                    break;
                default:
                    tag.CameraModel.Content = "UNKNOWN MODEL";
                    tag.myEllipse.Fill = SurfaceColors.ControlAccentBrush;
                    break;
            }
        }
    }
}