using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotConsultant.Models
{
    [Serializable]
    public class MessageContext
    {
        public int Count = 1;

        public string Intent { get; set; }

        public string Question { get; set; }

        public string Message { get; set; }

        public string PlatformMissed { get; set; }
    }
}