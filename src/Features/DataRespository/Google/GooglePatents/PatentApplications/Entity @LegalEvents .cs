using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DxMLEngine.Features.GooglePatents
{
    internal class LegalEvents
    {
        internal class LevelEvent
        {
            public DateTime Date { set; get; }
            public string Code { set; get; }
            public string Title { set; get; }
            public string Description { set; get; }

            public LevelEvent(DateTime date, string code, string title, string description)
            {
                this.Date = date;
                this.Code = code;
                this.Title = title;
                this.Description = description;
            }
        }

        public LevelEvent[] Events { set; get; }

        public LegalEvents(LevelEvent[] events)
            => this.Events = events;
    }
}
