using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JonasPluginBase;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace eXtremePlugins
{
    public class GetRandomText : JonasPluginBase.JonasCodeActivityBase
    {
        private static Random random = new Random();

        public override void Execute(JonasPluginBag bag)
        {
            var length = GetCodeActivityParameter<int>(Length);
            var charset = GetCodeActivityParameter<string>(CharacterSet);
            bag.Trace("Generating random text with {0} characters from {1}", length, charset);

            var result = new StringBuilder();
            while (result.Length < length)
            {
                var pos = random.Next(charset.Length);
                bag.Trace("Got random position: {0}", pos);
                var chr = charset[pos];
                bag.Trace("That is character:   {0}", chr);
                result.Append(chr);
            }
            SetCodeActivityParameter<string>(RandomText, result.ToString());
            bag.Trace("Returning: {0}", result);
        }

        [Input("Length")]
        public InArgument<int> Length { get; set; }

        [Input("Character Set")]
        [Default("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")]
        public InArgument<string> CharacterSet { get; set; }

        [Output("Random Text")]
        public OutArgument<string> RandomText { get; set; }
    }
}
