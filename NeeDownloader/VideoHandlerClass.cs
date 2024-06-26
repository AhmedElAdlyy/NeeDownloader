﻿using NeeDownloader.ViewModels;
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

        public void DownloadVideo(OneVideoAttributesToDownload attributes)
        {
            if (attributes.VideoUrl == "")
            {
                Console.WriteLine("Video is not found");
                Thread.Sleep(6000);
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += DownloadProgressHandler;

                client.Headers.Add("authority", "rr4---sn-hgn7ynek.googlevideo.com");
                client.Headers.Add("method", "GET");
                if (attributes.VideoUrl.Contains(".com"))
                {
                    client.Headers.Add("path", attributes.VideoUrl.Remove(0, attributes.VideoUrl.IndexOf(".com")));
                }
                else
                {
                    client.Headers.Add("path", attributes.VideoUrl.Remove(0, attributes.VideoUrl.IndexOf(".info")));
                }
                
                client.Headers.Add("scheme", "https");
                client.Headers.Add("Accept", "*/*");
                client.Headers.Add("Accept-Encoding", "identity;q=1, *;q=0");
                client.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                client.Headers.Add("Referer", attributes.VideoUrl);
                client.Headers.Add("Sec-Ch-Ua", "\"Google Chrome\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"");
                client.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
                client.Headers.Add("Sec-Ch-Ua-Platform", "Windows");
                client.Headers.Add("Sec-Fetch-Dest", "video");
                client.Headers.Add("Sec-Fetch-Mode", "no-cors");
                client.Headers.Add("Sec-Fetch-Site", "same-origin");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");



                var subFolder = assistant.GetNextVideoName(attributes.BaseLocation).ToString();

                try
                {
                    Console.WriteLine($"Starting Downloading Video... {subFolder} - {attributes.VideoName}");
                    var videoDate = client.DownloadData(attributes.VideoUrl);
                    string outputPath = attributes.BaseLocation + "\\" + subFolder + "- " + attributes.VideoName.Replace("\t", " ").Replace("\n", " ").Replace("\r", "") + ".mp4";
                    Console.WriteLine($"Video {subFolder} - {attributes.VideoName} Downloaded !!");
                    File.WriteAllBytes(outputPath, videoDate);
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
                finally
                {
                    client.Dispose();
                }


                string log = "Video => " + attributes.VideoName + ".mp4 has been saved to => " + attributes.BaseLocation;
                File.AppendAllText("Save_video.log", log + "\n");
            }

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

        private static void DownloadProgressHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            double pre = (double)e.BytesReceived / e.TotalBytesToReceive * 100;
            Console.WriteLine($"Downloaded => {e.BytesReceived / 1024} KB of ${e.TotalBytesToReceive / 1024} KB (${pre:F2}%)");
        }


    }
}
