using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;

namespace IntelligentBot
{
    [LuisModel("********************************", "***********************************")]
    [Serializable]
    public class Luisdialog : LuisDialog<object>
    {

        
        [LuisIntent("Weather")]
        public async Task getweather(IDialogContext context, LuisResult result)
        {

            string city, weather, temp, wind;
            XmlDocument doc = new XmlDocument();
            EntityRecommendation rec;
            if (result.TryFindEntity("Location", out rec))
            {
                var reply = context.MakeMessage();
                reply.Attachments = new List<Microsoft.Bot.Connector.Attachment>();

                city = rec.Entity;
                string url = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&mode=xml&units=metric&appid=5cef78776888888****74";
                var xml = await new WebClient().DownloadStringTaskAsync(new Uri(url));
                doc.LoadXml(xml);

                weather = doc.DocumentElement.SelectSingleNode("weather").Attributes["value"].Value;
                temp = doc.DocumentElement.SelectSingleNode("temperature").Attributes["value"].Value;
                wind = doc.DocumentElement.SelectSingleNode("wind").SelectSingleNode("speed").Attributes["name"].Value;
                await context.PostAsync($"Here it is!");
                //   await context.PostAsync($"It's {weather} in {city} with a Temperature of {temp}°C and Wind type is {wind} ");


                HeroCard card = new HeroCard()
                {
                    Subtitle = $"Its {weather} in {city} with a temperature of {temp}°C and wind type is {wind}",
            };
                List<CardImage> image = new List<CardImage>();
                CardImage cardimage = new CardImage("http://icons.iconarchive.com/icons/wineass/ios7-redesign/512/Weather-icon.png");
                image.Add(cardimage);
                card.Images = image;

                reply.Attachments.Add(card.ToAttachment());
                await context.PostAsync(reply);

            }
            else
            {
                await context.PostAsync($"Sorry cannot find the weather !!");
            }
            context.Wait(MessageReceived);
     }
        
        [LuisIntent("Synonym")]
        public async Task getwords(IDialogContext context, LuisResult result)
        {
            EntityRecommendation rec;
            string typedtext = null;
            if(result.TryFindEntity("text",out rec))
            {
                typedtext = rec.Entity;
                string url = "https://dictionary.yandex.net/api/v1/dicservice.json/lookup?key=dict.1.1.2*************************8.95e787b644992fabba9bce97545094247a1b5621d&lang=en-en&text="+typedtext;
                HttpClient http = new HttpClient();
                var response = await http.GetStringAsync(new Uri(url));
                JObject jo = JObject.Parse(response);
                JToken tok = jo["def"].First["pos"];               
               await context.PostAsync($"I found it. {typedtext} is a {tok}");                
                await context.PostAsync($"Here is the Synonyms of {typedtext} ");
                foreach (var item in jo.SelectToken("def[0].tr[0].syn"))
                {
                    
                  await context.PostAsync($" {item["text"]}");
                }


                context.Wait(MessageReceived);
            }

            else
            {
                await context.PostAsync($"Sorry cannot find the relevant match");
            }
        }

        [LuisIntent("News")]
        public async Task getnews(IDialogContext context, LuisResult result)
        {
            List<string> mylist = new List<string> { "Technology", "sport", "General", "Gaming", "Business", "Entertainment" };
            PromptOptions<string> options = new PromptOptions<string>("Which category of news would you prefer?", "Sorry ! Can you please try again", "I give up on myself", mylist, 2);
           PromptDialog.Choice<string>(context,getcategorynews,options);           
           
        }

        private async Task getcategorynews(IDialogContext context, IAwaitable<string> result)
        {
           
            string cate = await result;
            HttpClient http = new HttpClient();
            var url = "https://newsapi.org/v1/sources?language=en&category=" + cate; 
                var resp = await http.GetStringAsync(new Uri(url));
                JObject o = JObject.Parse(resp);
                foreach (var tok in o["sources"])
                {
                await context.PostAsync($"News are : {tok["description"]}");
                await context.PostAsync($"Refer for more details : {tok["url"]}");
            }
                context.Wait(MessageReceived);

            //    JToken token = o["articles"].First["title"];
            //  JToken token1 = o["articles"].First["urlToImage"];

        }


        [LuisIntent("Caption")]
        public async Task imagecaption(IDialogContext context, LuisResult result)
        {

            PromptDialog.Text(context, imagestreamm, "Give me an image");
           
        }

        private async Task imagestreamm(IDialogContext context, IAwaitable<string> result)
        {
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();
            var url = await result;
            reply.Attachments.Add(new Attachment()
                {
                ContentType = "image/png",
                ContentUrl = url,
                Name="Your Image"
            });


          //  Activity activity = new Activity();
            //Stream image = await getimage(activity);
            string message = await getcaption(url);
            await context.PostAsync(reply);
            await context.PostAsync($"It is {message}");
            context.Wait(this
                .MessageReceived);
        }

        public async Task<Stream> getimage(Activity activity)
        {
          
            var imageAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Contains("image"));
            var uri = new Uri(imageAttachment.ContentUrl);
            HttpClient http = new HttpClient();
            var stream = http.GetStreamAsync(uri);
            return await stream;
        }

        public async Task<string> getcaption(String url)
        {
            VisionServiceClient vision = new VisionServiceClient("****************************************");


            VisualFeature[] visual = new VisualFeature[] { VisualFeature.Description };
            AnalysisResult analysis = await vision.AnalyzeImageAsync(url,visual);
            string message = analysis?.Description?.Captions.FirstOrDefault()?.Text;
            return message;

            
        }


        

        [LuisIntent("Intro")]
        public async Task aboutbot(IDialogContext context, LuisResult result)
        {
            var reply1 = context.MakeMessage();
            reply1.Attachments = new List<Microsoft.Bot.Connector.Attachment>();

            List<CardImage> image = new List<CardImage>();
            CardImage cardimage = new CardImage("http://millennialceo.com/wp-content/uploads/2014/11/Social-Network-Bot.jpg");
            image.Add(cardimage);

            ThumbnailCard card = new ThumbnailCard()
            {
                Title = "I m Ibot ",
                Subtitle = "Hello! I can do some cool stuff for you by automating your tasks",
                Images = image
            };

            reply1.Attachments.Add(card.ToAttachment());
            await context.PostAsync(reply1);
            context.Wait(MessageReceived);
        }


        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sorry! I am just a prototype.I dont have any idea about that....");
            context.Wait(MessageReceived);
        }
    }

   
}
