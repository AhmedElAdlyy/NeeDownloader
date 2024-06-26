﻿using OpenQA.Selenium;
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
using NeeDownloader;
using System.ComponentModel.Design;
using NeeDownloader.ViewModels;
using System.Xml.Linq;

namespace Nee
{
    class Program
    {

        public ChromeDriver chrome;
        public string BaseLocation = "D:\\AHMED EL ADLY\\Media\\";
        public string MainWindow = "";
        public AssistantClass assistant = new AssistantClass();
        public ImageHandlerClass img = new ImageHandlerClass();
        public VideoHandlerClass vid = new VideoHandlerClass();
        static void Main(string[] args)
        {
            Program pg = new Program();
            var opts = new ChromeOptions();



            ChromeOptionsViewModels chromeOpts = pg.assistant.HandleChromeOptions();

            if (chromeOpts.IsHidden)
                opts.AddArgument("--headless=new");

            if (chromeOpts.WithDevTool)
                opts.AddArgument("--auto-open-devtools-for-tabs");

            if (chromeOpts.WithVPN)
                opts.AddExtension("C:\\Users\\ahmed\\Desktop\\2.7.3_0.crx");


            if (chromeOpts.GetSummary())
            {
                pg.chrome = new ChromeDriver(opts);
            }
            else
            {
                pg.chrome = new ChromeDriver();
            }


            Console.WriteLine("\n 1- Get Images \n 2- Get Videos");
            string downloadType = Console.ReadLine();
            Console.WriteLine("\n 1- Get 1 By 1 \n 2- continue batching");
            int choice = int.Parse(Console.ReadLine());

            Console.WriteLine("Your are saving to AHMED EL ADly => Media \n " +
                "Enter a sub folder to creare and save to");

            string subFolder = Console.ReadLine();

            Directory.CreateDirectory(pg.BaseLocation + subFolder);

            pg.BaseLocation = pg.BaseLocation + subFolder + "\\";

            Console.WriteLine("**********************************");
            Console.WriteLine("Enter the URL you want to download..");
            string userUrl = Console.ReadLine();

            if (downloadType == "1")
            {

                if (choice == 1)
                {
                    pg.GoAndDownload(userUrl);
                }
                else if (choice == 2)
                {
                    Console.WriteLine("Enter the page number you want to start with..");
                    int userPage = int.Parse(Console.ReadLine());

                    string baseUrl = userUrl + "?page=";

                    for (int page = userPage; page < 100; page++)
                    {
                        pg.chrome.Navigate().GoToUrl(baseUrl + page);
                        pg.MainWindow = pg.chrome.CurrentWindowHandle;

                        Thread.Sleep(5000);

                        ReadOnlyCollection<IWebElement> anchorElements = pg.chrome.FindElements(By.XPath("//article/a"));
                        if (anchorElements.Count != 0)
                        {
                            var URLs = pg.img.GetURLsFromAnchor(anchorElements);
                            pg.GoAndDownload(URLs, downloadType);
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
                    Console.WriteLine("Wrong Choice");
                }

                pg.chrome.Quit();
            }
            else if (downloadType == "2")
            {
                if (choice == 1)
                {
                    pg.GoAndDownloadOneVideo(userUrl);

                }
                else if (choice == 2)
                {
                    Console.WriteLine("Enter the page number you want to start with..");
                    int userPage = int.Parse(Console.ReadLine());

                    string baseUrl = userUrl + "?page=";

                    for (int page = userPage; page < 100; page++)
                    {
                        pg.chrome.Navigate().GoToUrl(baseUrl + page);
                        pg.MainWindow = pg.chrome.CurrentWindowHandle;

                        Thread.Sleep(5000);

                        ReadOnlyCollection<IWebElement> articleElements = pg.chrome.FindElements(By.XPath("//article[@class='block']"));
                        if (articleElements.Count != 0)
                        {

                            List<VideoInformationViewModel> VideosInformations = pg.vid.GetURLsFromArticles(articleElements);
                            pg.GoAndDownload(VideosInformations);
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
                    Console.WriteLine("Wrong Choice");
                }

                pg.chrome.Quit();
            }

        }

        /***********************************************************************************************/
        private void GoAndDownloadOneVideo(string videoUri)
        {

            OneVideoAttributesToDownload attributes = new OneVideoAttributesToDownload();

            this.chrome.Navigate().GoToUrl(videoUri);

            bool IsNeedRefresh = IsElementExist(By.XPath("//body/div[@id='sf-resetcontent']/h1"));

            while (IsNeedRefresh)
            {
                this.chrome.Navigate().Refresh();
                Thread.Sleep(5000);
                if (!IsElementExist(By.XPath("//body/div[@id='sf-resetcontent']/h1")))
                    break;
            }

            bool IsNeedSubscribe = IsElementExist(By.Id("button"));

            if (IsNeedSubscribe)
            {
                Console.WriteLine("website subscription is required, resource can't be downloaded");
                this.chrome.Close();
                this.chrome.SwitchTo().Window(this.MainWindow);
            }

            attributes.VideoName = this.chrome.FindElement(By.TagName("meta")).GetDomAttribute("content");
            attributes.BaseLocation = BaseLocation;

            var iframeSrc = this.chrome.FindElement(By.TagName("iframe")).GetAttribute("src");

            if (iframeSrc != "")
            {
                attributes.VideoUrl = GetVideoUrlFromiFrame(iframeSrc, this.chrome.CurrentWindowHandle);
            }
            else
            {
                attributes.VideoUrl = this.chrome.FindElement(By.Id("video_player_html5_api")).GetAttribute("src");
            }

            this.chrome.Quit();

            vid.DownloadVideo(attributes);

        }


        /***********************************************************************************************/


        private void GoAndDownload(List<VideoInformationViewModel> videosInfo)
        {
            int maxAttempts = 50;
            int attempts = 0;
            foreach (var video in videosInfo)
            {
                while (attempts < maxAttempts)
                {
                    try
                    {
                        this.chrome.SwitchTo().NewWindow(WindowType.Tab);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Attempt {attempts + 1} failed: {e.Message}");
                        attempts++;
                        this.chrome.Close();
                        this.chrome.SwitchTo().Window(this.MainWindow);
                    }
                }

                this.chrome.Navigate().GoToUrl(video.VideoUrl);

                bool IsNeedRefresh = IsElementExist(By.XPath("//body/div[@id='sf-resetcontent']/h1"));

                while (IsNeedRefresh)
                {
                    this.chrome.Navigate().Refresh();
                    Thread.Sleep(5000);
                    if (!IsElementExist(By.XPath("//body/div[@id='sf-resetcontent']/h1")))
                        break;
                }

                bool IsNeedSubscribe = IsElementExist(By.Id("button"));

                if (IsNeedSubscribe)
                {
                    Console.WriteLine("Subscription is required will get you out of the shit");
                    this.chrome.Close();
                    this.chrome.SwitchTo().Window(this.MainWindow);
                }

                OneVideoAttributesToDownload attrubutes = new OneVideoAttributesToDownload
                {
                    VideoName = video.VideoName,
                    BaseLocation = BaseLocation
                };

                var iframeSrc = this.chrome.FindElement(By.TagName("iframe")).GetAttribute("src");

                if(iframeSrc != "")
                {
                    attrubutes.VideoUrl = GetVideoUrlFromiFrame(iframeSrc, this.chrome.CurrentWindowHandle);
                }
                else
                {
                    attrubutes.VideoUrl = this.chrome.FindElement(By.Id("video_player_html5_api")).GetAttribute("src");
                }

                this.chrome.Close();
                this.chrome.SwitchTo().Window(this.MainWindow);

                vid.DownloadVideo(attrubutes);

            }
        }



        private void GoAndDownload(List<string> URLs, string downloadType, List<string> VideosNames = null)
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

                if (downloadType == "1")
                {
                    int imageCounter = this.chrome.FindElements(By.XPath("//div[@class='post-body']//div[@class='separator']/a/img")).Count;

                    if (IsNeedSubscribe && imageCounter == 0)
                    {
                        Console.WriteLine("Website subscription is required, resource can't be downloaded");
                        this.chrome.Close();
                        this.chrome.SwitchTo().Window(this.MainWindow);
                    }
                    else
                    {
                        var imagesElements = this.chrome.FindElements(By.XPath("//div[@class='post-body']//div[@class='separator']/a/img"));
                        string albumName = this.chrome.FindElement(By.XPath("//div[@class='post-img']/h3")).Text.ToString();
                        List<string> srcUrls = img.GetImagesURLs(imagesElements);
                        Thread.Sleep(5000);
                        this.chrome.Close();
                        this.chrome.SwitchTo().Window(this.MainWindow);

                        img.DownloadImages(srcUrls, albumName, BaseLocation);
                    }
                }
                else if (downloadType == "2")
                {
                    if (IsNeedSubscribe)
                    {
                        Console.WriteLine("Website subscription is required, resource can't be downloaded");
                        this.chrome.Close();
                        this.chrome.SwitchTo().Window(this.MainWindow);
                    }


                    var iframeSrc = this.chrome.FindElement(By.TagName("iframe")).GetAttribute("src");

                    string videoUrl = GetVideoUrlFromiFrame(iframeSrc, this.chrome.CurrentWindowHandle);

                    this.chrome.Close();
                    this.chrome.SwitchTo().Window(this.MainWindow);


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
                Console.WriteLine("Website subscription is required, resource can't be downloaded");
                this.chrome.Close();
                this.chrome.SwitchTo().Window(this.MainWindow);
            }
            else
            {
                var imagesElements = this.chrome.FindElements(By.XPath("//div[@class='post-body']//div[@class='separator']/a/img"));
                string albumName = this.chrome.FindElement(By.XPath("//div[@class='post-img']/h3")).Text.ToString();
                List<string> srcUrls = img.GetImagesURLs(imagesElements);
                Thread.Sleep(5000);
                img.DownloadImages(srcUrls, albumName, BaseLocation);
                Thread.Sleep(5000);
                this.chrome.Quit();
            }
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

        private string GetVideoUrlFromiFrame(string iframeSrc, string backWindow)
        {
            int maxAttempts = 50;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                try
                {
                    this.chrome.SwitchTo().NewWindow(WindowType.Tab);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Attempt {attempts + 1} failed: {e.Message}");
                    attempts++;
                    this.chrome.Close();
                    this.chrome.SwitchTo().Window(backWindow);
                }
            }

            this.chrome.Navigate().GoToUrl(iframeSrc);

            string videoUrl = this.chrome.FindElement(By.TagName("video")).GetAttribute("src");

            this.chrome.Close();
            this.chrome.SwitchTo().Window(backWindow);

            return videoUrl;
        }





    }
}