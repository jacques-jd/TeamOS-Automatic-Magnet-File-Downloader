using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WebBrowser = System.Windows.Forms.WebBrowser;
using TextBox = System.Windows.Controls.TextBox;
using System.Threading;

namespace torrentdownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string link;
        Dictionary<int, string> categories = new Dictionary<int, string>()
            {
            { 0, "" },
            { 18, "Team-OS Releases[VIP Releases]"},
            { 23, "    ↳Team OS Program"},
            { 19, "    ↳Windows 7 Releases"},
            { 20, "        ↳32 Bit"},
            { 21, "        ↳64 Bit"},
            { 22, "        ↳x86-x64 AIO"},
            { 24, "    ↳Windows 8.1 Release"},
            { 25, "        ↳32 Bit"},
            { 26, "        ↳64 Bit"},
            { 27, "        ↳x86-x64 AIO"},
            { 107, "    ↳Windows 10 Releases"},
            { 109, "        ↳64 Bit"},
            { 110, "        ↳x86-x64 AIO"},
            { 28, "Operating Systems"},
            { 29, "    ↳Windows XP"},
            { 30, "        ↳32 Bit"},
            { 31, "        ↳64 Bit"},
            { 32, "        ↳x86/64+AIO"},
            { 33, "    ↳Windows 7"},
            { 34, "        ↳32 Bit"},
            { 35, "        ↳64 Bit"},
            { 36, "        ↳x86/64+AIO"},
            { 37, "    ↳Windows 8 &amp; 8.1"},
            { 38, "        ↳32 Bit"},
            { 39, "        ↳64 Bit"},
            { 40, "        ↳x86/64+AIO"},
            { 42, "    ↳Windows 10"},
            { 43, "        ↳Windows 10 (x86)"},
            { 44, "        ↳Windows 10 (x64)"},
            { 45, "        ↳Windows 10 (x86 &amp; x64)"},
            { 157, "    ↳Windows 11"},
            { 158, "        ↳Windows 11 (x86)"},
            { 159, "        ↳Windows 11 (x64)"},
            { 160, "        ↳Windows 11 (x86 &amp; x64)"},
            { 140, "    ↳MacOS"},
            { 41, "    ↳Windows Servers/Old Windows OS/Others"},
            { 112, "    ↳Ghost Images"},
            { 113, "        ↳32 Bit Ghost Images"},
            { 114, "        ↳64 Bit Ghost Images"},
            { 46, "    ↳Untouched ISO(MSDN)"},
            { 47, "        ↳Windows 7 Untouched (MSDN)"},
            { 48, "            ↳32 Bit Untouched (MSDN)"},
            { 49, "            ↳64 Bit Untouched (MSDN)"},
            { 50, "            ↳x86/64+AIO Untouched ISO"},
            { 51, "        ↳Windows 8&amp;8.1 Untouched (MSDN)"},
            { 52, "            ↳32 Bit Untouched (MSDN)"},
            { 53, "            ↳64 Bit Untouched (MSDN)"},
            { 54, "            ↳x86/64+AIO Untouched ISO"},
            { 58, "        ↳Microsoft Office Multi-lang Untouch ISO&#039;S"},
            { 59, "            ↳x86-x64 bit Untouched Iso"},
            { 55, "        ↳Windows Server 2003 (Untouched)"},
            { 56, "            ↳32 Bit Untouched"},
            { 57, "            ↳64 Bit Untouched"},
            { 132, "    ↳Linux/Unix"},
            { 60, "Software Releases"},
            { 75, "    ↳Adobe Plugins"},
            { 62, "    ↳Android Apps"},
            { 141, "    ↳Mac Apps"},
            { 61, "    ↳Antivirus"},
            { 65, "    ↳Multimedia"},
            { 66, "        ↳Audio Apps"},
            { 68, "        ↳Photo Apps"},
            { 111, "        ↳3D Modeling"},
            { 67, "        ↳Video Apps"},
            { 76, "    ↳Customisation"},
            { 69, "    ↳Document Tools"},
            { 63, "    ↳Download Managers"},
            { 77, "    ↳Drivers and Necessary Tools"},
            { 64, "    ↳DVD Tools"},
            { 73, "    ↳Microsoft Office"},
            { 83, "    ↳Silent Apps"},
            { 72, "    ↳Other Softwares"},
            { 79, "    ↳Recovery Tools"},
            { 78, "    ↳Security Tools"},
            { 70, "    ↳Utility Tools"},
            { 74, "    ↳Web Tools"},
            { 80, "    ↳Windows Phone Apps"},
            { 81, "    ↳Developers"},
            { 82, "    ↳Designers"},
            { 84, "TEAM OS Other Stuff Releases"},
            { 131, "    ↳MAC Games"},
            { 88, "    ↳Android Games"},
            { 85, "    ↳Games"},
            { 86, "    ↳Ebooks"},
            { 87, "    ↳Wallpapers, Themes &amp; Screensavers"},
            { 89, "    ↳Other Releases"},
            };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //set location if settings exists
            txtLocation.Text = Properties.Settings.Default.Location;

            //Populate size unit choices
            cboSizeUnit1.ItemsSource = cboSizeUnit2.ItemsSource = new List<string>() { "MB", "GB" };
            cboSizeUnit1.SelectedIndex = cboSizeUnit2.SelectedIndex = 0;

            //initialise webbrowser location
            link = "https://teamos-hkrg.com/torrents/?direction=DESC";
            webBrowser.Navigate(link);

            //initialise categories
            ImportCategories();

            //add warning
            txtLog.Text = $"**********WARNING***********\r\n" +
                $"USE THIS TOOL AT YOUR OWN RISK. BY IMPORTING LARGE AMOUNTS OF TORRENTS INTO YOUR TORRENT PROGRAM, " +
                $"YOU RUN THE RISK OF RUINING YOUR RATIO. ALWAYS MAKE SURE YOU HAVE ENOUGH RATIO TO DOWNLOAD THE TOTAL SIZE." +
                $"\r\n-------------\r\n" +
                $"When finished downloading, the total size of all the torrents will be displayed. This is not the size" +
                $" of the torrent files, but the size of their contents, once you import them into your torrenting program.";
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            //make sure all fields are filled out properly
            if (!int.TryParse(txtPagesStart.Text, out int startpages) ||
                !int.TryParse(txtPagesMax.Text, out int endpages) ||
                !decimal.TryParse(txtSizeMin.Text, out decimal minSize) ||
                !decimal.TryParse(txtSizeMax.Text, out decimal maxSize) ||
                !(cboSizeUnit1.SelectedIndex >= 0) ||
                !(cboSizeUnit2.SelectedIndex >= 0) ||
                !webBrowser.IsLoaded)
                return;

            //passes File location and the Information log into the torrent link class
            var mld = new TorrentLinkDownloader(txtLocation.Text, txtLog);

            //parses the search criteria into $GET url params
            string args = CreateArgs();

            txtLog.Text = "> Status: Initialising...\r\n";

            //Logic Entrypoint.
            //Passing all search criteria into the method.
            // (int[] pagebounds, decimal[] sizebounds, string[] sizeboundunits, WebBrowser authentication, GET arguments)
            mld.GetTorrentLinks(new int[] { startpages, endpages }, new decimal[] { minSize, maxSize }, new string[] { cboSizeUnit1.Text, cboSizeUnit2.Text }, webBrowser, args);

            //scroll to end of log because it doesn't work when I call it from other places
            txtLog.ScrollToEnd();
        }

        private void ImportCategories()
        {
            //This populates the categories combobox with the Dictionary's values, while associating their keys too. Allows for dynamic getting of category pages
            cboCategory.ItemsSource = categories.Select(kvp => new CustomKeyValuePair<int, string> { Key = kvp.Key, Value = kvp.Value });
        }

        private void cboCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Filters out parent categories (which display nothing on their pages)
            switch(cboCategory.SelectedIndex)
            {
                case 1:
                case 3:
                case 7:
                case 14:
                case 15:
                case 19:
                case 23:
                case 27:
                case 31:
                case 37:
                case 40:
                case 41:
                case 45:
                case 49:
                case 51:
                case 55:
                case 60:
                case 80:
                    cboCategory.SelectedIndex = 0;
                    break;
            }
        }

        //just creates the GET url params
        private string CreateArgs()
        {
            string args = "";

            if (chkFreeleech.IsChecked == true)
                args += "&freeleech=1";
            if (txtUsername.Text != "")
                args += "&username=" + txtUsername.Text.Replace(' ', '+');
            if (cboCategory.SelectedIndex > 0)
                args += "&category_id=" + cboCategory.SelectedValue;
            if (txtTag.Text != "")
                args += "&tag=" + txtTag.Text.Replace(' ', '+');

            return args;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult dg = dialog.ShowDialog();
                if(dg == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    txtLocation.Text = dialog.SelectedPath;
            }
                
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //save settings (basically folder location)
            Properties.Settings.Default.Location = txtLocation.Text;
            Properties.Settings.Default.Save();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(txtLocation.Text);
            } 
            catch (Exception ex)
            {
                txtLog.Text += "\r\n-- Error: Invalid Folder Location --";
                txtLog.ScrollToEnd();
            }
        }
    }

    //All the logic is here
    class TorrentLinkDownloader
    {
        //newPages keeps count of all the WebBrowsers open
        private List<WebBrowser> newPages;
        //downloads is where all the download links go
        private List<string> downloads;
        //totalResults is a tally of all the valid torrent links that have been downloaded
        int totalResults;
        int totalResultsSize; //total size in MB
        //destination is the file destination
        private string destination;
        //cookies stores the session cookies for auth
        private string cookies;
        //log is the output log of the program
        Log log;

        public TorrentLinkDownloader(string dest, Log log)
        {
            destination = dest;
            downloads = new List<string>();
            newPages = new List<WebBrowser>();
            this.log = log;
            totalResults = 0;
            totalResultsSize = 0;
        }

        
        public void GetTorrentLinks(int[] pages, decimal[] sizes, string[] units, System.Windows.Controls.WebBrowser auth, string args)
        {
            //loops this code once for every page needed
            for (int i = pages[0]; i <= pages[1]; i++)
            {
                //creates a url based on the GET params + current page
                string url = $"{auth.Source.OriginalString}{args}&page={i}";

                log.WriteLine($"> Fetching: {url}");

                //create a new webbrowser object
                WebBrowser newpage = new WebBrowser();
                newPages.Add(newpage);
                newpage.Navigate(url);

                //once page is done loading, execute this (and scroll to the end of the log)
                newpage.DocumentCompleted += (sender, e) => { GetTorrentLinksFromPage(newpage, sizes, units); log.ScrollToEnd(); };
            }
        }

        private void GetTorrentLinksFromPage(WebBrowser auth, decimal[] sizes, string[] units)
        {
            HtmlDocument doc = auth.Document;

            List<HtmlElement> torrents = new List<HtmlElement>();

            foreach (HtmlElement d in doc.All)
            {
                if (d.GetAttribute("className") != null &&
                d.GetAttribute("className").Contains("dataList-rowGroup torrentListItem"))
                {
                    torrents.Add(d);
                }
            }

            if (torrents.Count < 1)
            {
                //pages are empty.
                newPages.Remove(auth);
                auth.Dispose();
                log.WriteLine("> Empty Page");
                log.ScrollToEnd();
                return;
            }

            log.WriteLine($"> Processing Page {auth.Url.ToString().Split('=').Last()}");
            cookies = WebHelper.GetGlobalCookies(auth.Url.AbsoluteUri);

            foreach (HtmlElement tor in torrents)
            {
                HtmlElement innerTable = tor.Children[0];
                string[] sourcesize = innerTable
                    .Children[4]
                    .InnerText
                    .Split(' ');

                decimal.TryParse(sourcesize[0], out decimal torSize);
                string torUnit = sourcesize[1];

                if(sizes[0] > 0) //only execute min size check if it's not a 0 value
                {
                    switch (units[0]) //minimum size check
                    {
                        case "MB":
                            if (torUnit == "KB")
                                continue;
                            if (torSize < sizes[0] && torUnit == units[0])
                                continue;
                            break;
                        case "GB":
                            if (torUnit == "MB")
                                continue;
                            if (torSize < sizes[0])
                                continue;
                            break;
                    }
                }

                switch (units[1]) //maximum size check
                {
                    case "MB":
                        if (torUnit == "GB")
                            continue;
                        if (torSize > sizes[1] && torUnit != "KB")
                            continue;
                        break;
                    case "GB":
                        if (torSize > sizes[1] && torUnit == units[1])
                            continue;
                        break;
                }



                string dllink = innerTable.Children[2].Children[0].GetAttribute("href");

                string[] linkparts = dllink.Split('/');
                string filename = linkparts[linkparts.Length - 2] + ".torrent";

                if (System.IO.File.Exists($"{destination}{(destination.EndsWith("/") ? "" : "/")}{filename}"))
                {
                    //Console.WriteLine($"{filename} already exists");
                    log.WriteLine($"> Found file that already exists, skipping...");
                    continue;
                }

                log.WriteLine($"> Downloading {filename} ({torSize}{sourcesize[1]})");

                downloads.Add(dllink);
                totalResults++;

                if (torUnit == "GB")
                {
                    totalResultsSize += (int)((double)torSize * 1024);
                }
                else
                {
                    totalResultsSize += (int)torSize;
                }
            }

            using (var client = new System.Net.WebClient())
            {
                client.Headers.Add("Cookie: " + cookies);
                foreach (string link in downloads)
                {
                    string[] linkparts = link.Split('/');
                    string filename = linkparts[linkparts.Length - 2] + ".torrent";

                    try
                    {
                        client.DownloadFile(link, $"{destination}{(destination.EndsWith("/") ? "" : "/")}{filename}");
                        log.WriteLine($"> Downloaded {filename}");
                    }
                    catch (System.Net.WebException ex)
                    {
                        log.WriteLine("> error: " + ex.Message + "");
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine(ex.Data);
                    }
                }

                downloads.Clear();
            }

            newPages.Remove(auth);
            auth.Dispose();
            Console.WriteLine("Finished downloads on page " + doc.Url.ToString().Split('=').Last() + ", disposed webbrowser");
            log.WriteLine("> Finished downloads on page " + doc.Url.ToString().Split('=').Last() + ", disposed webbrowser");
            if (newPages.Count > 0)
            {
                log.WriteLine("> " + newPages.Count() + " pages to go");
                if (totalResultsSize > 1024m)
                    log.WriteLine($"> So far found {totalResults} total results (~{(double)(totalResultsSize / 1024):N}GB)");
                else
                    log.WriteLine($"> So far found {totalResults} total results (~{(double)(totalResultsSize):N}MB)");
            }
            else
            {
                if (totalResultsSize > 1024m)
                    log.WriteLine($"> Finished Downloading {totalResults} total results (~{(double)(totalResultsSize / 1024):N}GB)");
                else
                    log.WriteLine($"> Finished Downloadeding {totalResults} total results (~{(double)(totalResultsSize):N}MB)");

                if (newPages.Count > 0) newPages.ForEach(page => page.Dispose());
            }

        }
    }
    public class WebHelper
    {

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        public static string GetGlobalCookies(string uri)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
                && cookieData.Length > 0)
            {
                return cookieData.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}

public class CustomKeyValuePair<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }
}
