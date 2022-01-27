using System.Diagnostics;

namespace AupInfo.Wpf
{
    public static class Link
    {
        public static void OpenInBrowser(string url)
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}
