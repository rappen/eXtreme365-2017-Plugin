using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JonasPluginBase;
using Microsoft.Xrm.Sdk;

namespace eXtremePlugins
{
    public class NoFaxPlease : JonasPluginBase.JonasPluginBase
    {
        public override void Execute(JonasPluginBag bag)
        {
            if (!string.IsNullOrWhiteSpace(bag.TargetEntity.GetAttributeValue<string>("fax")))
            {
                throw new InvalidPluginExecutionException($"Fax? Really? This is {DateTime.Now.Year}, you know... ");
            }
            else
            {
                bag.Trace("No fax for {0}, this {1} is ready for the future.", bag.CompleteEntity.Name(bag, false), bag.PluginContext.PrimaryEntityName);
            }
        }
    }
}
