using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntelligentBot
{
    [LuisModel("1943292b-272b-43d4-8644-bc088cf2a9cd", "e44ae558274344a5b5627214291f14bb")]
    [Serializable]
    public class Luiss : LuisDialog<object>
    {
        [LuisIntent("Teamcount")]
        public async Task getteam(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"There are 20 teams");
            context.Wait(MessageReceived);

        }

        [LuisIntent("remove")]
        public async Task removeteam(IDialogContext context, LuisResult result)
        {
            string teamn = "";
            EntityRecommendation rec;
            if(result.TryFindEntity("teamname",out rec))
            {
                teamn = rec.Entity;
                await context.PostAsync($"I removed the team {teamn}");
            }
            else
            {
                await context.PostAsync($"Team not found");
            }
            
            context.Wait(MessageReceived);

        }

       
    }

   
    
}