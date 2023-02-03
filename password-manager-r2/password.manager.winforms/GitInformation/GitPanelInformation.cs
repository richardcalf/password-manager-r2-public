using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password.manager.winforms.GitInformation
{
    public static class GitPanelInformation
    {
        public static string LastPullTime { get; set; }
        public static string ShaAndMessage { get; set; }

        public static void SetPanelInformation(string message)
        {
            LastPullTime = DateTime.Now.ToString("HH:mm");
            ShaAndMessage = message;
        }
    }
}
