using JonasPluginBase;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace eXtremePlugins
{
    public class ThrowError : JonasPluginBase.JonasCodeActivityBase
    {
        public override void Execute(JonasPluginBag bag)
        {
            var error = GetCodeActivityParameter(Error);
            var code = GetCodeActivityParameter(Code);
            throw new InvalidPluginExecutionException(OperationStatus.Failed, code, error);
        }

        [Input("Error message")]
        [Default("<enter proper error message>")]
        public InArgument<string> Error { get; set; }

        [Input("Error code")]
        public InArgument<int> Code { get; set; }
    }
}
