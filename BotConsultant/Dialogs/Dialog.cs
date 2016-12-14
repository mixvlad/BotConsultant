using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using com.valgut.libs.bots.Wit;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotConsultant.Dialogs
{
    [Serializable]
    public class Dialog : IDialog<object>
    {
        protected int count = 1;
        
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            var wit = new WitClient("TIFXIBFNFSADNG56RNDJQ7V6VOP5Q554");
            var msg = wit.Converse(message.From.Id, message.Text);
            var intentWord = string.Empty; double conf = 0;

            try
            {
                var intent = msg.entities["intent"].OrderByDescending(x => x.confidence).FirstOrDefault();
                if (intent != null)
                    intentWord = intent.value.ToString();
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {

            }

            await context.PostAsync($"{this.count++}: You said {message.Text}. Action is {intentWord}");
            context.Wait(MessageReceivedAsync);


            //if (message.Text == "reset")
            //{
            //    PromptDialog.Confirm(
            //        context,
            //        AfterResetAsync,
            //        "Are you sure you want to reset the count?",
            //        "Didn't get that!",
            //        promptStyle: PromptStyle.None);
            //}
            //else
            //{
            //    await context.PostAsync($"{this.count++}: You said {message.Text}");
            //    context.Wait(MessageReceivedAsync);
            //}
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}