using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeeDownloader.ViewModels
{
    public class ChromeOptionsViewModels
    {
        public bool WithVPN { get; set; } = false;
        public bool IsHidden{ get; set; } = false;
        public bool WithDevTool { get; set; } = false;
        

        public bool GetSummary()
        {
            var total = WithDevTool || IsHidden || WithVPN;
            return total;
        }

    }
}
