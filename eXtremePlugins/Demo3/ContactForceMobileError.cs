﻿using JonasPluginBase;

namespace eXtremePlugins
{
    public class ContactForceMobileError : JonasPluginBase.JonasPluginBase
    {
        public override void Execute(JonasPluginBag bag)
        {
            var mobilephone = (bag.CompleteEntity.GetAttributeValue<string>("mobilephone") ?? string.Empty).PadRight(500, '.');
            bag.Trace("Setting mobilephone={0}", mobilephone);
            bag.TargetEntity["mobilephone"] = mobilephone;
        }
    }
}
