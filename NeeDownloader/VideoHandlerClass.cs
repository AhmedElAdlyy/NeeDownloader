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
                        VideoName = articleElement.FindElement(By.XPath("//div[@class='details']/h2")).Text
                    };
                    infos.Add(info);
                }

            }

            return infos;
        }

        public void DownloadVideo(string videoSrc, string videoName, string baseLocation)
        {
            WebClient client = new WebClient();
            client.Headers.Set(HttpRequestHeader.Accept, "*/*");
            client.Headers.Set(HttpRequestHeader.AcceptEncoding, "identity;q=1, *;q=0");
            client.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
            client.Headers.Set(HttpRequestHeader.Range, "bytes=0-");
            client.Headers.Set(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36");



            var subFolder = assistant.GetNextFolderName(baseLocation).ToString();

            var videoDate = client.DownloadData(videoSrc);
            File.WriteAllBytes(baseLocation + "\\" + subFolder + "- " + videoName + ".mp4", videoDate);

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
