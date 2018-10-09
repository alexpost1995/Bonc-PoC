using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.ComponentModel.DataAnnotations;


namespace Bonc_start.Dialogs
{
    [Serializable]
    public class WelcomeBackDialog : IDialog<object>
    {
        private string welcomeMessage = "Welkom terug bij Bonc!";
        private string promptText = "Ik kan een aantal taken uitvoeren, wat zou je willen doen?";
        private string promptTextFail = "Kies een van de bovenstaande opties";

        public async Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync(welcomeMessage);
            PromptDialog.Choice(
                context: context,
                resume: ChoiceReceivedAsync,
                options: (IEnumerable<Options>)Enum.GetValues(typeof(Options)),
                prompt: promptText,
                retry: promptTextFail,
                promptStyle: PromptStyle.Auto
                );
        }
        
        public virtual async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<Options> activity)
        {
            Options response = await activity;
            if (response.ToString() == "NieuwePost")
            {
                context.Call<object>(new Dialogs.NewPostDialogs.NewPostDialog(), ChildDialogComplete);
            }
            if (response.ToString() == "Account")
            {
                context.Call<object>(new Dialogs.AccountDialogs.AccountDialog(), ChildDialogComplete);
            }
            if (response.ToString() == "BerichtenOverzicht")
            {
                context.Call<object>(new Dialogs.PostOverviewDialogs.PostOverviewDialog(), ChildDialogComplete);
            }
            if (response.ToString() == "Instructies")
            {
                context.Call<object>(new Dialogs.InstructionDialogs.InstructionDialog(), ChildDialogComplete);
            }
            if (response.ToString() == "Instellingen")
            {
                context.Call<object>(new Dialogs.SettingsDialogs.SettingsDialog(), ChildDialogComplete);
            }
        }

        public virtual async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> response)
        {
            context.Done(this);
        }

        public enum Options
        {
            NieuwePost,
            Account,
            BerichtenOverzicht,
            Instructies,
            Instellingen
        }
    }
}