using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.IO;
using System.Transactions;

namespace Nee
{
    class Program
    {
        public ChromeDriver chrome = new ChromeDriver();
        public string BaseLocation = "D:\\AHMED EL ADLY\\Media\\Tabo\\";
        public string MainWindow = "";

        static void Main(string[] args)
        {
            Program pg = new Program();

            Console.WriteLine("\n 1- Get 1 By 1 \n 2- continue batching");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {

                Console.WriteLine("Enter a URL");
                string url = Console.ReadLine();

                pg.GoAndDownload(url);

            }
            else if (choice == 2)
            {
                Console.WriteLine("**********************************");
                Console.WriteLine("Enter the URL you want to download..");
                string userUrl = Console.ReadLine();
                Console.WriteLine("Enter the page number you want to start with..");
                int userPage = int.Parse(Console.ReadLine());

                //string baseUrl = "https://neek.info/posts/category/maharem?page=";
                string baseUrl = userUrl + "?page=";

                for (int page = userPage; page < 100; page++)
                {
                    pg.chrome.Navigate().GoToUrl(baseUrl + page);
                    pg.MainWindow = pg.chrome.CurrentWindowHandle;

                    Thread.Sleep(5000);

                    ReadOnlyCollection<IWebElement> anchorElements = pg.chrome.FindElements(By.XPath("//article/a"));
                    if (anchorElements.Count != 0)
                    {
                        var URLs = pg.GetURLsFromAnchor(anchorElements);
                        pg.GoAndDownload(URLs);
                    }
                    else
                    {
                        Console.WriteLine("Finished !!");
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("fuck you!");
            }

            pg.chrome.Quit();

        }


        private List<string> GetURLsFromAnchor(ReadOnlyCollection<IWebElement> anchorElements)
        {
            List<string> URLs = new List<string>();
            foreach (var anchorElement in anchorElements)
            {
                URLs.Add(anchorElement.GetAttribute("href"));
            }

            return URLs;
        }

        private void GoAndDownload(List<string> URLs)
        {

            foreach (var url in URLs)
            {
                this.chrome.SwitchTo().NewWindow(WindowType.Tab);
                this.chrome.Navigate().GoToUrl(url);


                bool IsNeedRefresh = IsElementExist(By.XPath("//body/div[@id='sf-resetcontent']/h1"));

                while (IsNeedRefresh)
                {
                    this.chrome.Navigate().Refresh();
                    Thread.Sleep(5000);
                    if (!IsElementExist(By.XPath("//body/div[@id='sf-resetcontent']/h1")))
                        break;
                }

                bool IsNeedSubscribe = IsElementExist(By.Id("button"));
                int imageCounter = this.chrome.FindElements(By.XPath("//div[@class='post-body']//div[@class='separator']/a/img")).Count;

                if (IsNeedSubscribe && imageCounter == 0)
                {
                    Console.WriteLine("Subscription is required will get you out of the shit");
                    this.chrome.Close();
                    this.chrome.SwitchTo().Window(this.MainWindow);
                }
                else
                {
                    var imagesElements = this.chrome.FindElements(By.XPath("//div[@class='post-body']//div[@class='separator']/a/img"));
                    string albumName = this.chrome.FindElement(By.XPath("//div[@class='post-img']/h3")).Text.ToString();
                    List<string> srcUrls = GetImagesURLs(imagesElements);
                    Thread.Sleep(5000);
                    this.chrome.Close();
                    this.chrome.SwitchTo().Window(this.MainWindow);

                    DownloadImages(srcUrls, albumName);
                }

            }

        }

        private void GoAndDownload(string URL)
        {
            this.chrome.Navigate().GoToUrl(URL);


            bool IsNeedRefresh = IsElementExist(By.XPath("//body/div[@id='sf-resetcontent']/h1"));

            while (IsNeedRefresh)
            {
                this.chrome.Navigate().Refresh();
                Thread.Sleep(5000);
                if (!IsElementExist(By.XPath("//body/div[@id='sf-resetcontent']/h1")))
                    break;
            }

            bool IsNeedSubscribe = IsElementExist(By.Id("button"));
            int imageCounter = this.chrome.FindElements(By.XPath("//div[@class='post-body']//div[@class='separator']/a/img")).Count;

            if (IsNeedSubscribe && imageCounter == 0)
            {
                Console.WriteLine("Subscription is required will get you out of the shit");
                this.chrome.Close();
                this.chrome.SwitchTo().Window(this.MainWindow);
            }
            else
            {
                var imagesElements = this.chrome.FindElements(By.XPath("//div[@class='post-body']//div[@class='separator']/a/img"));
                string albumName = this.chrome.FindElement(By.XPath("//div[@class='post-img']/h3")).Text.ToString();
                List<string> srcUrls = GetImagesURLs(imagesElements);
                Thread.Sleep(5000);
                DownloadImages(srcUrls, albumName);
                Thread.Sleep(5000);
                this.chrome.Quit();
            }


        }

        private List<string> GetImagesURLs(ReadOnlyCollection<IWebElement> ImagesElements)
        {
            List<string> srcUrls = new List<string>();
            foreach (var ImageElement in ImagesElements)
            {
                srcUrls.Add(ImageElement.GetAttribute("src"));
            }

            return srcUrls;
        }

        private void DownloadImages(List<string> ImagesSrcs, string albumName)
        {
            WebClient client = new WebClient();
            var subFolder = GetNextFolderName().ToString();
            var currentBase = "";


            for (int i = 0; i < ImagesSrcs.Count(); i++)
            {
                try
                {
                    var imageData = client.DownloadData(ImagesSrcs[i]);
                    if (i == 0)
                    {
                        Directory.CreateDirectory(this.BaseLocation + "\\" + subFolder + "-" + albumName);
                        currentBase = this.BaseLocation + "\\" + subFolder + "-" + albumName + "\\";
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

        private int GetNextFolderName()
        {
            int nextFolder = 0;
            string[] foldersPaths = Directory.GetDirectories(this.BaseLocation);
            List<string> folders = new List<string>();

            foreach (var path in foldersPaths)
            {
                folders.Add(path.Substring(path.LastIndexOf("\\") + 1));
            }

            if (folders.Count == 0)
            {
                nextFolder = 1;
            }
            else
            {
                for (int i = 0; i < folders.Count; i++)
                {
                    folders[i] = folders[i].Substring(0, folders[i].IndexOf("-"));
                }

                List<int> myInt = folders.Select(int.Parse).ToList();
                nextFolder = myInt.Max() + 1;
            }

            return nextFolder;
        }


        private bool IsElementExist(By by)
        {
            try
            {
                this.chrome.FindElement(by);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }





    }
}