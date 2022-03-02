using System.Windows;
using System.Windows.Controls;

namespace torrentdownloader
{
    public class Log : TextBox
    {
        public void WriteLine(string text)
        {
            AppendText(text + "\r\n");
            ScrollToEnd();
        }
    }
}
