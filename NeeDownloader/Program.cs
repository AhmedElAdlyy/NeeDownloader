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
using NeeDownloader;

namespace Nee
{
    class Program
    {
        public ChromeDriver chrome = new ChromeDriver();
        public string BaseLocation = "D:\\AHMED EL ADLY\\Media\\";
        public string MainWindow = "";
        public AssistantClass assistant = new AssistantClass();
        public ImageHandlerClass img = new ImageHandlerClass();
        static void Main(string[] args)
        {
            Program pg = new Program();


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
            else if (downloadType == "2")
            {

            }



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
                    List<string> srcUrls = img.GetImagesURLs(imagesElements);
                    Thread.Sleep(5000);
                    this.chrome.Close();
                    this.chrome.SwitchTo().Window(this.MainWindow);

                    img.DownloadImages(srcUrls, albumName, BaseLocation);
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





    }
}