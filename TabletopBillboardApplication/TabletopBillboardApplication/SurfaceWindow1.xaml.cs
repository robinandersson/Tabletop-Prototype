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

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            
            rnd = new Random();
            // load text
            List<ImageData> data = new List<ImageData>();
            data = LoadText(data);
            List<ImageData> datum = data;
            //DateTime oldestPoster = getOldestPoster(data);
            setIndice(data);
            // load image
            LoadImages(data);

            foreach (object obj in scatter.Items)
            {
                ScatterViewItem svi = scatter.ItemContainerGenerator.ContainerFromItem(obj) as ScatterViewItem;
            }

        }

        /*private DateTime getOldestPoster(ImageData[] data)
        {
            DateTime oldest = data[0].getDate();
            int num = 1;
            while(!data[num].Equals("")){ // need to check this to be sure
                if (DateTime.Compare(data[num].getDate(),oldest)<0){
                    oldest = data[num].getDate();
                }
            }
            return oldest;
        }*/

        private void setIndice(List<ImageData> data)
        {
            int num = 0;
            DateTime today = DateTime.Today;
            DateTime tempToday = DateTime.Today;
            int length = data.Count;
            //int len = data.Length;
            //int length = data.Length/data.Rank;
            while (length > num++){ // needs to be checked
                DateTime posterDay = data[num].getDate();
                int dice = 3; //dice 3, further than a month away
                int compare = DateTime.Compare(posterDay, today);
                if (compare == 0) { // posters of today
                    dice = 6;
                }
                else {
                    if(compare < 0) {   // posters from the past
                        dice = 1;
                    }
                    else {              //dice 5, within 10 days
                        tempToday.AddDays(10);
                        if (DateTime.Compare(posterDay, tempToday) < 0) {
                            dice = 5;
                        }
                        else {          //dice 4, further than 10 days, but within a month 
                            tempToday = DateTime.Today;
                            tempToday.AddDays(30);
                            if (DateTime.Compare(posterDay, tempToday) < 0){
                                dice = 4;
                            }
                        }
                    }
                }
                 data[num].setDice(dice);
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
        void LoadImages(List<ImageData> data)
        {
            string envDir = Environment.CurrentDirectory;
            string[] fileNames = Directory.GetFiles(envDir+@"\Resources\Posters", "*.jpg");
            int num = 0;
            //DateTime today = DateTime.Today;
            foreach (string name in fileNames)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(name, UriKind.Absolute));
                // create random size for image
                //int dice = rnd.Next(1, 6);
                int dice = data[num].getDice();

                svi = new ScatterViewItem();
                svi.Content = img;
                int scale = (int) (100 * (img.Source.Height) / img.Source.Width);
                svi.Width = 100*dice;
                svi.Height = scale * dice;
                //svi.Width = img.Source.Height * dice; // Way too large
                //svi.Height = img.Source.Width * dice; // Way too large
                //int longestEdge = int (Math.Max(img.Source.Height, img.Source.Width));

                scatter.Items.Add(svi);
                num++;
            }
        }

        // load text from source
        private List<ImageData> LoadText(List<ImageData> data)
        {
            try
            {
                using (StreamReader sr = new StreamReader(@"Resources\Text Posters\TextImage.txt"))
                {
                    String name = sr.ReadLine();
                    while (name != null)
                    {
                        String dat = sr.ReadLine();
                        DateTime date = Convert.ToDateTime(dat);
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
                        data.Add(new ImageData(name, date, tag, line));
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
        private class ImageData
        {
            private List<String> Tags;
            private String Text, Name;
            private DateTime Date;
            private int Dice = 0;

            //public string Name { get; set; }
            //public int Age { get; set; }

            public ImageData(String Name0, DateTime Date0, List<String> Tags0, String Text0){
                Name = Name0;
                Date = Date0;
                Tags = Tags0;
                Text = Text0;
                //Dice = Dice0;
            }

            public string getName(){
                return Name;
            }

            public DateTime getDate(){
                return Date;
            }

            public List<string> getTags(){
                return Tags;
            }

            public string getText(){
                return Text;
            }

            public int getDice() {
                return Dice;
            }
            public void setDice(int Dice2) {
                Dice = Dice2;
            }
        }
    }
}