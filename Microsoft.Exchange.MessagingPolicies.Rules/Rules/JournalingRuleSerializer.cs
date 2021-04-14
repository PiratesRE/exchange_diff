using System;
using System.Xml;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class JournalingRuleSerializer : TransportRuleSerializer
	{
		public new static JournalingRuleSerializer Instance
		{
			get
			{
				return JournalingRuleSerializer.instance;
			}
		}

		protected override void SaveRuleAttributes(XmlWriter xmlWriter, Rule rule)
		{
			base.SaveRuleAttributes(xmlWriter, rule);
			JournalingRule journalingRule = rule as JournalingRule;
			if (journalingRule.GccRuleType != GccType.None)
			{
				xmlWriter.WriteAttributeString("gccType", JournalingRuleConstants.StringFromGccType(journalingRule.GccRuleType));
			}
		}

		private static readonly JournalingRuleSerializer instance = new JournalingRuleSerializer();
	}
}
