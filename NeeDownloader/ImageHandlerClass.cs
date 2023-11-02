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
    public class ImageHandlerClass
    {

        AssistantClass assistant = new AssistantClass();

        public List<string> GetURLsFromAnchor(ReadOnlyCollection<IWebElement> anchorElements)
        {
            List<string> URLs = new List<string>();
            foreach (var anchorElement in anchorElements)
            {
                URLs.Add(anchorElement.GetAttribute("href"));
            }

            return URLs;
        }

        public List<string> GetImagesURLs(ReadOnlyCollection<IWebElement> ImagesElements)
        {
            List<string> srcUrls = new List<string>();
            foreach (var ImageElement in ImagesElements)
            {
                srcUrls.Add(ImageElement.GetAttribute("src"));
            }

            return srcUrls;
        }

        public void DownloadImages(List<string> ImagesSrcs, string albumName, string baseLocation)
        {
            WebClient client = new WebClient();
            var subFolder = assistant.GetNextFolderName(baseLocation).ToString();
            var currentBase = "";


            for (int i = 0; i < ImagesSrcs.Count(); i++)
            {
                try
                {
                    var imageData = client.DownloadData(ImagesSrcs[i]);
                    if (i == 0)
                    {
                        Directory.CreateDirectory(baseLocation + "\\" + subFolder + "-" + albumName);
                        currentBase = baseLocation + "\\" + subFolder + "-" + albumName + "\\";
                    }

                    File.WriteAllBytes(currentBase + i + ".jpg", imageData);
                    string log = "Image => " + i + ".jpg has been saved to => " + currentBase;
                    File.AppendAllText("Save.log", log + "\n");
                }
                catch (Exception)
                {

                    Console.WriteLine($"error getting Image {i} Album 404 : {albumName}");
                }

            }

        }
    }
}
