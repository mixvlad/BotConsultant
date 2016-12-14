using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using BotConsultant.Handlers;
using BotConsultant.Models;
using com.valgut.libs.bots.Wit;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Json;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;

namespace BotConsultant.Dialogs
{
    [Serializable]
    public class EnhancedDialog : IDialog<object>
    {

        protected MessageContext Context;

        protected OperationHandler OperationHandler;
        
        [NonSerialized]
        private WitConversation<MessageContext> _client;
        

        public EnhancedDialog()
        {
            Context = new MessageContext();

            OperationHandler = new OperationHandler();
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }


        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (_client == null)
            {
                _client = new WitConversation<MessageContext>("TIFXIBFNFSADNG56RNDJQ7V6VOP5Q554", message.From.Id, null, DoMerge, DoSay, DoAction, DoStop);
            }

            if (message.Text == "Сбросить")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Уверены что хотите сбросить счетчик?",
                    "Не понял!");
            }
            else if (message.Text == "Форма")
            {
                context.Call(JsonDialog.MakeJsonRootDialog(), this.ResumeAfterEnhanceDialog);
            }
            else
            {
                Task<bool> t = _client.SendMessageAsync(message.Text);
                t.Wait();

                await context.PostAsync($"{Context.Count++}: {Context.Message}");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterEnhanceDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }



        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                Context.Count = 1;
                await context.PostAsync("Сбросил счетчик.");
            }
            else
            {
                await context.PostAsync("Не сбросил.");
            }
            context.Wait(MessageReceivedAsync);
        }

        public MessageContext DoMerge(string conversationId, MessageContext context, Dictionary<string, List<com.valgut.libs.bots.Wit.Models.Entity>> entities, double confidence)
        {
            return context;
        }

        public void DoSay(string conversationId, MessageContext context, string msg, double confidence)
        {
            context.Message = msg;
        }

        public MessageContext DoAction(string conversationId, MessageContext context, string action, Dictionary<string, List<com.valgut.libs.bots.Wit.Models.Entity>> entities, double confidence)
        {
            try
            {
                var intent = entities["intent"].OrderByDescending(x => x.confidence).FirstOrDefault();
                if (intent != null)
                {
                    return OperationHandler.PerformOperation(intent.value.ToString(), context, entities);
                }

            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {

            }

            return OperationHandler.PerformOperation(null, context, entities);
        }

        public MessageContext DoStop(string conversationId, MessageContext context)
        {
            if (Context != null)
            {
                Context.Message = context.Message;
            }
            else
            {
                Context = context;
            }
            
            return context;
        }
    }
}