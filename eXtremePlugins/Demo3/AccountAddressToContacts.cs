using System.Collections.Generic;
using JonasPluginBase;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace eXtremePlugins
{
    // Plugin to synchronize the address of accounts to their contacts, for all contacts that do not have an explicitly
    // specified custom address.
    // The plugin shall be registered post update of account, triggering on the seven address attributes listed below.
    // The step should also have a pre-image registered with the same attributes to be able to determine which
    // contacts that has/had the same address as the account.
    public class AccountAddressToContacts : JonasPluginBase.JonasPluginBase
    {
        // Address fields to copy from account to contacts
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
            // This will enable extensive details about requests and results when compiling with debug configuration
            bag.TracingService.Verbose = true;
#endif
            var contacts = GetContactsInheritingAddress(bag);
            UpdateContacts(bag, contacts);
        }

        // Retrieve all contacts of the triggering account, that have the old address of the account or no address at all.
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

        // Update contacts with the address information of the account that triggered the plugin
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
                    else if (contact.Contains(attr))
                    {
                        bag.Trace("Attribute {0} not changed, removing from entity to update", attr);
                        contact.Attributes.Remove(attr);
                    }
                }
                bag.Service.Update(contact);
            }
            bag.TraceBlockEnd();
        }
    }
}
