using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelligentBot
{
    public class translatetext
    {
        [Prompt("Enter a text to translate :")]
        public string name { get; set; }

        public static IForm<translatetext> buildform()
        {

            return new FormBuilder<translatetext>().Field("name").AddRemainingFields().Build();
        }
    }
}