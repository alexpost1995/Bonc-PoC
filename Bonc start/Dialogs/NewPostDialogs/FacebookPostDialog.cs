﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Bonc_start.Dialogs.NewPostDialogs
{
    [Serializable]
    public class FacebookPostDialog : IDialog<object>
    {
        private string welcomeMessage = "Ik zal je een aantal stappen laten doorlopen om een bericht te plaatsen op Facebook.";
        private string promptText = "Wat is de tekst van het bericht dat je zou willen posten?";
        private string promptTextFail = "Kies een van de bovenstaande opties";

        private string textToPost;
        private string imageToPost;
     
        public class Tag
        {
            public string name { get; set; }
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(welcomeMessage);

            PromptDialog.Text(
                context: context,
                resume: ChoiceReceivedAsync,
                prompt: promptText,
                retry: promptTextFail
                );
        }

        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<string> text)
        {
            textToPost = await text;
            await context.PostAsync($"Ik heb de volgende tekst doorgekregen: '{textToPost}'");

            PromptDialog.Text(
                context: context,
                resume: ChildDialogComplete,
                prompt: "Plak de url van de toe te voegen afbeeling in het tekstvak.",
                retry: promptTextFail
                );
        }

        public virtual async Task ChildDialogComplete(IDialogContext context, IAwaitable<string> image)
        {
            imageToPost = await image;

            await context.PostAsync("Ik heb de volgende Facebook-post voor je samengesteld:");
            var message = context.MakeMessage();
            var attachment = FacebookPostCard("", textToPost, imageToPost);
            message.Attachments.Add(attachment);

            await context.PostAsync(message);

            await MakeAnalysisRequest(imageToPost, context);
        }

        public enum Options
        {
            Facebook,
            Instagram,
            LinkedIn,
            Twitter
        }

        private static Attachment FacebookPostCard(string title, string text, string imageUrl)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = "",
                Text = text,
                Images = new List<CardImage> { new CardImage(imageUrl) },
            };
            return heroCard.ToAttachment();
        }

        static async Task MakeAnalysisRequest(string imageFilePath, IDialogContext context)
        {
            const string subscriptionKey = "31495f44467e40e5aa8b017852219356";

            const string uriBase =
                "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/analyze";

            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);
                
                string requestParameters =
                    "visualFeatures=Tags";
                //"visualFeatures=Categories,Description,Color";

                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                byte[] byteData = GetImageAsByteArray(imageFilePath);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    response = await client.PostAsync(uri, content);
                }

                string contentString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("\nResponse:\n\n{0}\n",
                    JToken.Parse(contentString).ToString());
                //Tag newTag = JsonConvert.DeserializeObject<Tag>(JToken.Parse(contentString).ToString());
                //await context.PostAsync(newTag.name.ToString());

                await context.PostAsync($"De geuploade afbeelding is getagd met de volgende keywords: {JToken.Parse(contentString).ToString()}");

                //JsonSerializer serializer = new JsonSerializer();

                //Tag tag = new Tag();
                //using (StreamWriter sw = new StreamWriter(JToken.Parse(contentString).ToString()))
                //using (JsonWriter writer = new JsonTextWriter(sw))
                //{
                //    serializer.Serialize(writer, tag);
                //}
                //await context.PostAsync("abc" + tag.name);

            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(imageFilePath);
                return imageBytes;
            }
        }
    }
}