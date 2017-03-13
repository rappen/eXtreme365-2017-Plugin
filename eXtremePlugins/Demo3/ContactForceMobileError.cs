using JonasPluginBase;

namespace eXtremePlugins
{
    public class ContactForceMobileError : JonasPluginBase.JonasPluginBase
    {
        public override void Execute(JonasPluginBag bag)
        {
            var mobilephone = bag.CompleteEntity.GetAttributeValue<string>("mobilephone") ?? string.Empty;
            bag.Trace("Mobilephone was: {0}", mobilephone);
            mobilephone = mobilephone.PadRight(500, '.');
            bag.Trace("Setting mobilephone={0}", mobilephone);
            bag.TargetEntity["mobilephone"] = mobilephone;
        }
    }
}
