using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Json;
using Newtonsoft.Json.Linq;

namespace BotConsultant.Dialogs
{
    public class JsonDialog
    {
        public static IDialog<JObject> MakeJsonRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(BuildJsonForm))
                .Do(async (context, order) =>
                {
                    try
                    {
                        var completed = await order;
                        // Actually process the sandwich order...
                        await context.PostAsync("До новых встреч!");
                    }
                    catch (FormCanceledException<JObject> e)
                    {
                        string reply;
                        if (e.InnerException == null)
                        {
                            reply = $"You quit on {e.Last}--maybe you can finish next time!";
                        }
                        else
                        {
                            reply = "Sorry, I've had a short circuit.  Please try again.";
                        }
                        await context.PostAsync(reply);
                    }
                });
        }

        public static IForm<JObject> BuildJsonForm()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BotConsultant.bobrform.json"))
            {
                var schema = JObject.Parse(new StreamReader(stream).ReadToEnd());
                return new FormBuilderJson(schema)
                    .AddRemainingFields()
                    .Build();
            }
        }
    }
}