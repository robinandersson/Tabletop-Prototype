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
using System.Diagnostics;

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
        private DateTime[] date;

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
            setSize(events, DateTime.Today);
            // load image
            LoadImages(events);
            date = getOldestNewestEventDate();

            foreach (object obj in scatter.Items)
            {
                ScatterViewItem svi = scatter.ItemContainerGenerator.ContainerFromItem(obj) as ScatterViewItem;
            }
            MyUserControl mainButton = new MyUserControl();
            mainButton.timeValueSelectedHandler += new MyUserControl.TimeValueSelectedEventHandler(mainButton_timeValueSelectedHandler);
            svi_MainButton.Content = mainButton;

        }

        private void setSize(List<EventData> events, DateTime today)
        {
            int num = 0;
            DateTime tempToday = today;
            int length = events.Count;
            while (length > num+1){
                DateTime EventDay = events.ElementAt(num).getDate();
                int size = 1; //size 1, further than a month away, and old ones
                int compare = DateTime.Compare(EventDay, today);
                if (compare == 0) { // Events of today
                    size = 6;
                }
                if (compare >0) {              //size 4, within 10 days
                    tempToday = DateTime.Today; 
                    tempToday = tempToday.AddDays(10);
                    if (DateTime.Compare(EventDay, tempToday) < 0) {
                        size = 4;
                    }
                    else {          //size 3, further than 10 days, but within a month 
                        tempToday = DateTime.Today;
                        tempToday = tempToday.AddDays(30);
                        if (DateTime.Compare(EventDay, tempToday) < 0){
                            size = 3;
                        }
                        else
                        {//size 2, further than 10 days, but within half a year
                            tempToday = DateTime.Today;
                            tempToday = tempToday.AddDays(183);
                            if (DateTime.Compare(EventDay, tempToday) < 0)
                            {
                                size = 2;
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
            int num2 = 0;
            foreach (string name in fileNames)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(name, UriKind.Absolute));
                svi = new ScatterViewItem();
                svi.Tag = events.ElementAt(num2);
                svi.Content = img;
                
                updateSize(svi);
                

                svi.AddHandler(TouchExtensions.TapGestureEvent, new RoutedEventHandler(OnPosterTap), true);
                scatter.Items.Add(svi);
                num2++;
            }
        }

        private void updateSize(ScatterViewItem svi)
        {
 	        Image img = (Image)svi.Content;
            int size = (svi.Tag as EventData).getSize();
            if (size == 1)
            {
                svi.Visibility = Visibility.Hidden;
            }
            else
            {
                svi.Visibility = Visibility.Visible;
                int scale = (int)(100 * (img.Source.Width) / img.Source.Height);
                svi.Height = 100 * size;
                svi.Width = scale * size;
                if (svi.Width > svi.Height)
                {
                    scale = (int)(100 * (img.Source.Height) / img.Source.Width);
                    svi.Width = 100 * size;
                    svi.Height = scale * size;
                };
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

        void mainButton_timeValueSelectedHandler(double timeValue)
        {
            getOldestNewestEventDate();
            DateTime newToday = getNewToday(date[0], date[1], DateTime.Today, timeValue);
            foreach (ScatterViewItem svi in scatter.Items)
            {
                EventData eventData = svi.Tag as EventData;
                if (eventData != null)
                {
                    setSize2(eventData, newToday);
                    updateSize(svi);
                }
            }
        }


        private DateTime getNewToday(DateTime old, DateTime newe, DateTime today, Double timeValue)
        {
            //difference in days
            Debug.WriteLine(timeValue);
            DateTime tempOld = old;
            DateTime tempToday = today;

            int i = 0;
            int j = 0;
            while (DateTime.Compare(tempOld, today) ==-1)
            {
                i++;
                tempOld = old;
                tempOld = tempOld.AddDays(i);
                //Debug.WriteLine(String.Concat(old, " + ", i, " = ", today));
            }
            while (DateTime.Compare(tempToday, newe) ==-1)
            {
                j++;
                tempToday = today;
                tempToday = tempToday.AddDays(j);
                //Debug.WriteLine(String.Concat(today, " + ", j, " = ", newe));
            }

            //the today depending on in the past or the future
            DateTime newToday;
            if(timeValue <= 6){
                double timeValue1 = Math.Min(3, timeValue);
                newToday = DateTime.Today;
                newToday = newToday.AddDays((int)(timeValue1/3*j));
                Debug.WriteLine(String.Concat("future ", timeValue," ", timeValue1," ", newToday));
            }
            else{
                double timeValue1 = -1*(Math.Max(timeValue-9,0));
                newToday = DateTime.Today;
                newToday = newToday.AddDays((int)(timeValue1/3*i));
                Debug.WriteLine(String.Concat("Past ", timeValue, " ", timeValue1, " ", newToday));
            }
            Debug.WriteLine(String.Concat("The new today is ",newToday));

 	        return newToday;
        }

        private void setSize2(EventData events, DateTime today)
        {
            DateTime tempToday = today;
            DateTime eDate = events.getDate();
            
            int size = 1; //size 1, further than a month away and old ones
            int compare = DateTime.Compare(eDate, today);
            if (compare == 0)// Events of today
                size = 6;
            if (compare > 0){           //size 4, within 10 days
                tempToday = today;
                tempToday = tempToday.AddDays(10);
                if (DateTime.Compare(eDate, tempToday) < 0)
                {
                    size = 4;
                }
                else{//size 3, further than 10 days, but within a month
                    tempToday = today;
                    tempToday = tempToday.AddDays(30);
                    if (DateTime.Compare(eDate, tempToday) < 0)
                    {
                        size = 3;
                    }
                    else
                    {//size 2, further than a month, but within half a year
                        tempToday = today;
                        tempToday = tempToday.AddDays(183);
                        if (DateTime.Compare(eDate, tempToday) < 0)
                        {
                            size = 2;
                        }
                    }
                }
            } 
            events.setSize(size);
        }

        // get the oldest and newest date of the posters in the system
        DateTime[] getOldestNewestEventDate()
        {
            DateTime[] dates = new DateTime[2]; // 0 is oldest event, 1 is newest event
            int i = 0;
            foreach (ScatterViewItem svi in scatter.Items)
            {
                EventData eventData = svi.Tag as EventData;
                if (eventData != null)
                {
                    DateTime eDate = eventData.getDate();
                    if (i == 0)
                    {
                        dates[0] = eDate;
                        dates[1] = eDate;
                    }
                    else
                    {
                        if (DateTime.Compare(dates[0], eDate) == 1) //eDate is earlier than dates[0]
                        {
                            dates[0] = eDate;
                        }
                        if (DateTime.Compare(dates[1], eDate) == -1) //eDate is later than dates[0]
                        {
                            dates[1] = eDate;
                        }
                    }
                    i++;
                }
                
            }
            return dates;
        }
    }
}