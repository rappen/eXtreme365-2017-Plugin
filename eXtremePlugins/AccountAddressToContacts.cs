using System.Collections.Generic;
using JonasPluginBase;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace eXtremePlugins
{
    public class AccountAddressToContacts : JonasPluginBase.JonasPluginBase
    {

        private static List<string> addressAttributes = new List<string>(new string[] {
            "address1_line1",
            "address1_line2",
            "address1_line3",
            "address1_city",
            "address1_stateorprovince",
            "address1_postalcode",
            "address1_country"
        });

        public override void Execute(JonasPluginBag bag)
        {
            if (!bag.PluginContext.PrimaryEntityName.Equals("account"))
            {
                bag.Trace("Expected entity account, got {0}", bag.PluginContext.PrimaryEntityName);
                return;
            }
            if (!bag.PluginContext.MessageName.Equals("Update"))
            {
                bag.Trace("Expected Update message, got {0}", bag.PluginContext.MessageName);
                return;
            }
#if DEBUG
            bag.Service.TraceDetails = true;
#endif
            var contacts = GetContactsInheritingAddress(bag);
            UpdateContacts(bag, contacts);
        }

        private static EntityCollection GetContactsInheritingAddress(JonasPluginBag bag)
        {
            bag.TraceBlockStart();
            var qry = new QueryExpression("contact");
            qry.ColumnSet.AddColumn("fullname");
            qry.ColumnSet.AddColumns(addressAttributes.ToArray());

            qry.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, bag.PluginContext.PrimaryEntityId);

            var fltAddress = qry.Criteria.AddFilter(LogicalOperator.Or);
            var fltNoAddress = fltAddress.AddFilter(LogicalOperator.And);
            var fltSameAddress = fltAddress.AddFilter(LogicalOperator.And);

            foreach (var attr in addressAttributes)
            {
                fltNoAddress.AddCondition(attr, ConditionOperator.Null);
            }

            foreach (var attr in addressAttributes)
            {
                var prevalue = bag.PreImage.GetAttributeValue<string>(attr);
                bag.Trace("Adding filter for {0} = {1}", attr, prevalue);
                if (prevalue == null)
                {
                    fltSameAddress.AddCondition(attr, ConditionOperator.Null);
                }
                else
                {
                    fltSameAddress.AddCondition(attr, ConditionOperator.Equal, prevalue);
                }
            }
            bag.TraceBlockEnd();
            return bag.Service.RetrieveMultiple(qry);
        }

        private static void UpdateContacts(JonasPluginBag bag, EntityCollection contacts)
        {
            bag.TraceBlockStart();
            foreach (var contact in contacts.Entities)
            {
                bag.Trace("Updating contact: {0}", contact.Name(bag, false));
                foreach (var attr in addressAttributes)
                {
                    bag.Trace("Checking: {0}", attr);
                    var newvalue = bag.CompleteEntity.GetAttributeValue<string>(attr) ?? string.Empty;
                    var oldvalue = contact.GetAttributeValue<string>(attr) ?? string.Empty;
                    if (!oldvalue.Equals(newvalue))
                    {
                        bag.Trace("Setting {0} = {1}", attr, newvalue);
                        contact[attr] = newvalue.Equals(string.Empty) ? null : newvalue;
                    }
                }
                bag.Service.Update(contact);
            }
            bag.TraceBlockEnd();
        }
    }
}
