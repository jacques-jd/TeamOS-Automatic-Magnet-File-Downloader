using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using mshtml;
using WebBrowser = System.Windows.Forms.WebBrowser;
using Label = System.Windows.Controls.Label;

namespace torrentdownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string link;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cboSizeUnit.ItemsSource = new List<string>() { "MB", "GB" };
            cboSizeUnit.SelectedIndex = 0;
            link = "https://teamos-hkrg.com/torrents/?direction=DESC";
            webBrowser.Navigate(link);
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            var mld = new TorrentLinkDownloader(txtLocation.Text, new Label[2]{lblStatus,lblStatus2});

            if (!int.TryParse(txtPagesStart.Text, out int startpages) || 
                !int.TryParse(txtPagesMax.Text, out int endpages) || 
                !decimal.TryParse(txtSizeMin.Text, out decimal minSize) ||
                !decimal.TryParse(txtSizeMax.Text, out decimal maxSize) || 
                !(cboSizeUnit.SelectedIndex >= 0) || 
                !webBrowser.IsLoaded)
                return;

            mld.GetTorrentLinks(startpages, endpages, new decimal[] { minSize, maxSize }, cboSizeUnit.Text, webBrowser);

            lblStatus.Content = "Status: Initialising...";
        }

    }

    class TorrentLinkDownloader
    {
        private List<WebBrowser> newPages;
        private List<string> downloads;
        int totalResults;
        private string destination;
        private string cookies;
        Label[] status;

        public TorrentLinkDownloader(string dest, Label[] status)
        {
            destination = dest;
            downloads = new List<string>();
            newPages = new List<WebBrowser>();
            this.status = status;
            totalResults = 0;
        }

        public void GetTorrentLinks(int startpages, int endpages, decimal[] size, string unit, System.Windows.Controls.WebBrowser auth)
        {
            for (int i = startpages; i <= endpages; i++)
            {
                status[0].Content = $"Status: Fetching Page {i}";
                WebBrowser newpage = new WebBrowser();

                newPages.Add(newpage);
                
                newpage.Navigate($"{auth.Source.OriginalString}&page={i}");
                newpage.DocumentCompleted += (sender, e) => GetTorrentLinksFromPage(newpage, size, unit);
            }
        }

        private void GetTorrentLinksFromPage(WebBrowser auth, decimal[] sizes, string unit)
        {
            status[0].Content = $"Status: Processing Page {auth.Url.ToString().Split('=').Last()}";
            cookies = WebHelper.GetGlobalCookies(auth.Url.AbsoluteUri);

            HtmlDocument doc = auth.Document;

            List<HtmlElement> torrents = new List<HtmlElement>();

            foreach (HtmlElement d in doc.All)
            {
                if(d.GetAttribute("className") != null &&
                d.GetAttribute("className").Contains("dataList-rowGroup torrentListItem"))
                {
                    torrents.Add(d);
                }
            }
            
            foreach (HtmlElement tor in torrents)
            {
                HtmlElement innerTable = tor.Children[0];
                string[] sourcesize = innerTable
                    .Children[4]
                    .InnerText
                    .Split(' ');

                decimal.TryParse(sourcesize[0], out decimal torSize);

                if (unit == "MB" && sourcesize[1] == "GB")

                if ((unit == "MB" && sourcesize[1] == "GB") || sourcesize[1] == "TB")
                    continue;

                if (unit == sourcesize[1] && (torSize > sizes[1] || torSize < sizes[0]))
                    continue;

                string dllink = innerTable.Children[2].Children[0].GetAttribute("href");

                string[] linkparts = dllink.Split('/');
                string filename = linkparts[linkparts.Length - 2] + ".torrent";

                

                if (System.IO.File.Exists($"{destination}{(destination.EndsWith("/") ? "" : "/")}{filename}"))
                {
                    //Console.WriteLine($"{filename} already exists");
                    status[1].Content = $"Found file that already exists, skipping...";
                    continue;
                }

                Console.WriteLine($"Downloading file that is {torSize}{sourcesize[1]}: {filename} from {dllink}");

                downloads.Add(dllink);
                totalResults++;
            }

            status[0].Content = $"Status: Found {totalResults} total results";

            using (var client = new System.Net.WebClient())
            {
                client.Headers.Add("Cookie: " + cookies);
                foreach (string link in downloads)
                {
                    string[] linkparts = link.Split('/');
                    string filename = linkparts[linkparts.Length - 2] + ".torrent";

                    //Console.WriteLine($"Downloading {filename} from {link}");
                    status[1].Content = $"Downloading {filename} from {link}";
                    try
                    {
                        client.DownloadFile(link, $"{destination}{(destination.EndsWith("/") ? "" : "/")}{filename}");
                        status[1].Content = $"Downloaded {filename}";
                        //Console.WriteLine($"Downloaded {filename}");
                    }
                    catch (System.Net.WebException ex)
                    {
                        /*if (ex.Response != null)
                        {
                            var response = ex.Response;
                            var dataStream = response.GetResponseStream();
                            var reader = new System.IO.StreamReader(dataStream);
                            var details = reader.ReadToEnd();
                            break;
                        }*/
                        Console.WriteLine("error " + ex.Message);
                    }
                }

                downloads.Clear();
            }

            newPages.Remove(auth);
            auth.Dispose();
            Console.WriteLine("Finished downloads on page " + doc.Url.ToString().Split('=').Last() + ", disposed webbrowser");
            if (newPages.Count > 0)
                Console.WriteLine(newPages.Count() + " pages to go");
            else
            {
                status[0].Content = "Status: Finished Downloading.";
                status[1].Content = $"{totalResults} Torrent files downloaded.";
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
