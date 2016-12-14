using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BotConsultant.Models;
using Microsoft.Bot.Builder.Dialogs;

namespace BotConsultant.Handlers
{
    [Serializable]
    public class OperationHandler
    {
        private delegate MessageContext OperationDelegate(MessageContext context, Dictionary<string, List<com.valgut.libs.bots.Wit.Models.Entity>> entities);
        private readonly Dictionary<string, OperationDelegate> _operations;

        public OperationHandler()
        {
            _operations =
            new Dictionary<string, OperationDelegate>
            {
                { "hello", HelloHandler },
                { "thanks", ThanksHandler },
                { "question", QuestionHandler }
            };
        }

        public MessageContext PerformOperation(string op, MessageContext context, Dictionary<string, List<com.valgut.libs.bots.Wit.Models.Entity>> entities)
        {
            MessageContext result;
            if (op == null || !_operations.ContainsKey(op))
            {
                result = GetMoreInfo(context, entities);
            }

            else
            {
                result = _operations[op](context, entities);
            }

            return result;
        }


        private MessageContext HelloHandler(MessageContext context, Dictionary<string, List<com.valgut.libs.bots.Wit.Models.Entity>> entities)
        {
            var resultContext = new MessageContext { Message = ResponsesParser.GetResponse("hello") };

            return resultContext;
        }


        private MessageContext ThanksHandler(MessageContext context, Dictionary<string, List<com.valgut.libs.bots.Wit.Models.Entity>> entities)
        {
            var resultContext = new MessageContext { Message = ResponsesParser.GetResponse("thanks") };

            return resultContext;
        }


        private MessageContext QuestionHandler(MessageContext context, Dictionary<string, List<com.valgut.libs.bots.Wit.Models.Entity>> entities)
        {
            MessageContext resultContext;

            if (!entities.ContainsKey("platform"))
            {
                resultContext = new MessageContext { PlatformMissed = "true" };
            }
            else
            {
                resultContext = new MessageContext { Message = "Что?" };
            }


            return resultContext;
        }


        private MessageContext GetMoreInfo(MessageContext context, Dictionary<string, List<com.valgut.libs.bots.Wit.Models.Entity>> entities)
        {
            var resultContext = new MessageContext { Message = ResponsesParser.GetResponse("DoNotUnderstand") };

            return resultContext;
        }
    }
}