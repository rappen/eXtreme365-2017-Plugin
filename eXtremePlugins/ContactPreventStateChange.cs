using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JonasPluginBase;
using Microsoft.Xrm.Sdk;

namespace eXtremePlugins
{
    // Automatic update of State should not be allowed
    public class ContactPreventStateChange : JonasPluginBase.JonasPluginBase
    {
        public override void Execute(JonasPluginBag bag)
        {
            if (bag.PluginContext.ParentContext == null)
            {
                bag.Trace("No parent, manual update is allowed.");
                return;
            }
            if (bag.PluginContext.ParentContext.PrimaryEntityName != "account")
            {
                bag.Trace("Did not originate from Account, we're good.");
                return;
            }
            var newstate = bag.TargetEntity.GetAttributeValue<string>("address1_stateorprovince") ?? string.Empty;
            bag.Trace("New state: {0}", newstate);
            var oldstate = bag.PreImage?.GetAttributeValue<string>("address1_stateorprovince") ?? string.Empty;
            bag.Trace("Old state: {0}", oldstate);
            if (!newstate.Equals(oldstate) && !oldstate.Equals(string.Empty))
            {
                var contactname = bag.CompleteEntity.Name(bag, false);
                throw new InvalidPluginExecutionException($"Automatic update of State is not allowed. {contactname} must be updated manually to handle this.");
            }
            bag.Trace("State does not appear to have been changed :)");
        }
    }
}
