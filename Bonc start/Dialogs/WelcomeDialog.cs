using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.FormFlow;
using System.Data.SqlClient;

namespace Bonc_start.Dialogs
{
    [Serializable]
    public class WelcomeDialog : IDialog<object>
    {
        private string welcomeMessage = "Hallo, ik ben Bonc, je persoonlijke social media assistent. Ik kan je leven een stuk " +
            "gemakkelijker maken op het gebied van social media, zo kan ik berichten voor je plaatsen, berichten schedulen en " +
            "je suggesties geven over wat je zou kunnen posten op je social media accounts.";
        private string promptText = "Heb je al een account of zou je er een willen maken?";
        private string promptTextFail = "Kies een van de bovenstaande opties";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(welcomeMessage);
            context.Wait(this.OptionsPrompt);
        }

        public virtual async Task OptionsPrompt(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;

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

            if (response.ToString() == "LogIn")
            {
                context.Call<object>(new Dialogs.WelcomeBackDialog(), ChildDialogComplete);
            }
            if (response.ToString() == "NewAccount")
            {
                context.Call<object>(new Dialogs.NewAccountDialog(), ChildDialogComplete);
            }
        }

        public virtual async Task ChildDialogComplete(IDialogContext context, IAwaitable<object> response)
        {
            context.Done(this);
        }

        public enum Options
        {
            LogIn,
            NewAccount
        }
    }
}