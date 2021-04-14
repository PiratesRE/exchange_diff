using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection
{
	internal sealed class StringProperty : Property
	{
		public StringProperty(string name) : base(name, typeof(string))
		{
		}

		protected override object OnGetValue(RulesEvaluationContext context)
		{
			throw new NotSupportedException("Outlook Protection rules are only executed on Outlook.");
		}
	}
}
