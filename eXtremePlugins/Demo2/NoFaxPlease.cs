using JonasPluginBase;
using Microsoft.Xrm.Sdk;
using System;

namespace eXtremePlugins
{
    public class NoFaxPlease : JonasPluginBase.JonasPluginBase
    {
        public override void Execute(JonasPluginBag bag)
        {
            // Read updated value for Fax from Target entity
            string fax = bag.TargetEntity.GetAttributeValue<string>("fax");

            if (!string.IsNullOrWhiteSpace(fax))
            {
                throw new InvalidPluginExecutionException($"Fax? Really? This is {DateTime.Now.Year}, you know... ");
            }

            // Get name of the record that triggered the plugin
            string recordName = bag.CompleteEntity.Name(bag, false);
            bag.Trace("No fax for {0}, this {1} is ready for the future.", recordName, bag.PluginContext.PrimaryEntityName);
        }
    }
}
