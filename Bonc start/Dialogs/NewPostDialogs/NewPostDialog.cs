using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bonc_start.Dialogs.NewPostDialogs
{
    [Serializable]
    public class NewPostDialog : IDialog<object>
    {
        private string welcomeMessage = "Je hebt gekozen om een nieuw bericht te plaatsen";
        private string promptText = "Ik zie dat je een nieuw bericht wil posten. Kies een van de volgende opties";
        private string promptTextFail = "Kies een van de bovenstaande opties";
 
    public async Task StartAsync(IDialogContext context)

        {
            await context.PostAsync(welcomeMessage);

            PromptDialog.Choice(
                context: context,
                resume: ChoiceReceivedAsync,
                options: (IEnumerable<Options>)Enum.GetValues(typeof(Options)),
                prompt: promptText,
                retry: promptTextFail,
                promptStyle: PromptStyle.Auto
                );
            //return Task.CompletedTask;
        }
          
        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<Options> activity)
        {
            Options response = await activity;
            if (response.ToString() == "Facebook")
            {
                context.Call<object>(new Dialogs.NewPostDialogs.FacebookPostDialog(), DialogComplete);
            }
            if (response.ToString() == "Instagram")
            {
                await context.PostAsync("Deze functionaliteit is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.NewPostDialogs.NewPostDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.NewPostDialogs.InstagramPostDialog(), ChildDialogComplete);
            }
            if (response.ToString() == "LinkedIn")
            {
                await context.PostAsync("Deze functionaliteit is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.NewPostDialogs.NewPostDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.NewPostDialogs.LinkedinPostDialog(), ChildDialogComplete);
            }
            if (response.ToString() == "Twitter")
            {
                await context.PostAsync("Deze functionaliteit is nog niet geimplementeerd.");
                context.Call<object>(new Dialogs.NewPostDialogs.NewPostDialog(), DialogComplete);
                //context.Call<object>(new Dialogs.NewPostDialogs.TwitterPostDialog(), ChildDialogComplete);
            }
        }

        public virtual async Task DialogComplete(IDialogContext context, IAwaitable<object> response)
        {
            context.Done(this);
        }

        public enum Options
        {
            Facebook,
            Instagram,
            LinkedIn,
            Twitter
        }
    }
}