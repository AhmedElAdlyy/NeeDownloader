using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NeeDownloader
{
    public class VideoHandlerClass
    {
        AssistantClass assistant = new AssistantClass();

        public List<VideoInformationViewModel> GetURLsFromArticles(ReadOnlyCollection<IWebElement> articleElements)
        {

            List<string> URLs = new List<string>();
            List<VideoInformationViewModel> infos = new List<VideoInformationViewModel>();
            foreach (IWebElement articleElement in articleElements)
            {

                bool isExist = IsElementExist(articleElement, By.ClassName("label-info"));

                if (isExist)
                {


                    VideoInformationViewModel info = new VideoInformationViewModel
                    {
                        VideoUrl = articleElement.FindElement(By.ClassName("block-thumbnail")).GetAttribute("href"),
                        VideoName = articleElement.FindElement(By.TagName("h2")).Text
                    };
                    infos.Add(info);
                }

            }

            return infos;
        }

        public void DownloadVideo(string videoSrc, string videoName, string baseLocation)
        {
            WebClient client = new WebClient();

            client.Headers.Add("authority", "rr4---sn-hgn7ynek.googlevideo.com");
            client.Headers.Add("method", "GET");
            client.Headers.Add("path", videoSrc.Remove(0, videoSrc.IndexOf(".com")));
            client.Headers.Add("scheme", "https");
            client.Headers.Add("Accept", "*/*");
            client.Headers.Add("Accept-Encoding", "identity;q=1, *;q=0");
            client.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            client.Headers.Add("Referer", videoSrc);
            client.Headers.Add("Sec-Ch-Ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
            client.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
            client.Headers.Add("Sec-Ch-Ua-Platform", "Windows");
            client.Headers.Add("Sec-Fetch-Dest", "video");
            client.Headers.Add("Sec-Fetch-Mode", "no-cors");
            client.Headers.Add("Sec-Fetch-Site", "same-origin");
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");



            var subFolder = assistant.GetNextVideoName(baseLocation).ToString();

            try
            {
                Console.WriteLine($"Starting Downloading Video... {subFolder} - {videoName}");
                var videoDate = client.DownloadData(videoSrc);
                Console.WriteLine($"Video {subFolder} - {videoName} Downloaded !!");
                File.WriteAllBytes(baseLocation + "\\" + subFolder + "- " + videoName + ".mp4", videoDate);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    Console.WriteLine("Exception");
                    Console.WriteLine(ex);
                    var response = ex.Response;
                    var ds = response.GetResponseStream();
                    var reader = new StreamReader(ds);
                    var details = reader.ReadToEnd();
                    Console.Write(details);
                }
            }


            string log = "Video => " + videoName + ".mp4 has been saved to => " + baseLocation;
            File.AppendAllText("Save_video.log", log + "\n");
        }



        private bool IsElementExist(IWebElement element, By by)
        {
            try
            {
                element.FindElement(by);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
