using NeeDownloader.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeeDownloader
{
    public class AssistantClass
    {
        public int GetNextFolderName(string baseLocation)
        {
            int nextFolder = 0;
            string[] foldersPaths = Directory.GetDirectories(baseLocation);
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


        public int GetNextVideoName(string baseLocation)
        {
            int nextVid = 0;
            DirectoryInfo dir = new DirectoryInfo(baseLocation);
            FileInfo[] vids = dir.GetFiles();

            if (vids.Count() == 0)
            {
                nextVid = 1;
            }
            else
            {
                List<string> filesNames = new List<string>();

                foreach (var vid in vids)
                {
                    filesNames.Add(vid.Name);
                }

                for (int i = 0; i < filesNames.Count; i++)
                {
                    filesNames[i] = filesNames[i].Substring(0, filesNames[i].IndexOf("-"));
                }

                List<int> myInt = filesNames.Select(int.Parse).ToList();
                nextVid = myInt.Max() + 1;
            }

            return nextVid;
        }

        public ChromeOptionsViewModels HandleChromeOptions()
        {
            ChromeOptionsViewModels options = new ChromeOptionsViewModels();

            Console.WriteLine("\n Do you need a VPN? \n 1- YES \n 2- No");
            options.WithVPN = Console.ReadLine() == "1" ? true : false;

            if (options.WithVPN)
            {
                options.IsHidden = false;
                Console.WriteLine("Your session will be visible automatically");
            }
            else
            {
                Console.WriteLine("\n your session type : \n 1- Visible \n 2- hidden \n ?");
                options.IsHidden = Console.ReadLine() == "2" ? true : false;
            }


            if (!options.IsHidden)
            {
                Console.WriteLine("\n Do you need developer tools? \n 1- YES \n 2- NO");
                options.WithDevTool = Console.ReadLine() == "1" ? true : false;
            }

            return options;
        }


    }
}
