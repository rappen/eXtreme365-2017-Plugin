using JonasPluginBase;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace eXtremePlugins
{
    public class AccountVerifyPrimaryContact : JonasPluginBase.JonasPluginBase
    {
        public override void Execute(JonasPluginBag bag)
        {
            var pcref = bag.TargetEntity.GetAttributeValue<EntityReference>("primarycontactid");
            if (pcref == null || pcref.Id.Equals(Guid.Empty))
            {
                bag.Trace("PC is null");
                return;
            }
#if DEBUG
            // This will enable extensive details about requests and results when compiling with debug configuration
            bag.TracingService.Verbose = true;
#endif
            bag.TraceBlockStart("Check other accounts");
            bag.Trace("Looking for other accounts with Primary Contact {0}", pcref.Id);
            var qry = new QueryExpression("account");
            qry.ColumnSet.AddColumn("name");
            qry.Criteria.AddCondition("accountid", ConditionOperator.NotEqual, bag.PluginContext.PrimaryEntityId);
            qry.Criteria.AddCondition("primarycontactid", ConditionOperator.Equal, pcref.Id);
            var otheraccounts = bag.Service.RetrieveMultiple(qry);
            bag.TraceBlockEnd();
            if (otheraccounts.Entities.Count > 0)
            {
                bag.TraceBlockStart("Throwing up");
                var acctnames = string.Join(", ", otheraccounts.Entities.Select(e => e.GetAttributeValue<string>("name") ?? "?"));
                bag.Trace("Other account(s): {0}", acctnames);
                var contact = bag.Service.Retrieve("contact", pcref.Id, new ColumnSet("fullname"));
                throw new InvalidPluginExecutionException($"{contact.Name(bag, false)} is already primary contact for account(s) :\n{acctnames}");
                bag.TraceBlockEnd();
            }
        }
    }
}
