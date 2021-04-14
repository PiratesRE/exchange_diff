using System;
using System.Xml;

namespace Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection
{
	internal sealed class OutlookProtectionRuleSerializer : RuleSerializer
	{
		protected override void SaveRuleSubElements(XmlWriter writer, Rule rule)
		{
			base.SaveRuleSubElements(writer, rule);
			OutlookProtectionRule outlookProtectionRule = (OutlookProtectionRule)rule;
			if (outlookProtectionRule.UserOverridable)
			{
				writer.WriteStartElement("userOverridable");
				writer.WriteEndElement();
			}
		}
	}
}
