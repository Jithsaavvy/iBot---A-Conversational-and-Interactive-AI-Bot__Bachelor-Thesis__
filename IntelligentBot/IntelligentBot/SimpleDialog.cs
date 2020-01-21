using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace IntelligentBot
{
    [Serializable]
    public class SimpleDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ActivityReceivedAsync);
        }

        private async Task ActivityReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (activity.Text.Contains("hi") || activity.Text.Contains("hello") || activity.Text.Contains("who are you"))
            {
                
                await context.PostAsync($"Welcome! I m iBot.I m here to do some cool stuff for you..");
                

            }
            if(activity.Text.Contains("what is the time") || activity.Text.Contains("tell me the time") || activity.Text.Contains("time"))
            {
                var time = DateTime.Now;
                await context.PostAsync($"Got it. \nCurrent time with the date is - {time}..");

            }
            context.Wait(ActivityReceivedAsync);
        }
    }
}