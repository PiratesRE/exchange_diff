using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContainsRecipientStringCondition : StringCondition
	{
		private ContainsRecipientStringCondition(Rule rule, string[] text) : base(ConditionType.ContainsRecipientStringCondition, rule, text)
		{
		}

		public static ContainsRecipientStringCondition Create(Rule rule, string[] text)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				text
			});
			return new ContainsRecipientStringCondition(rule, text);
		}

		public override Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return Rule.ProviderIdEnum.Exchange14;
			}
		}

		internal override Restriction BuildRestriction()
		{
			byte[][] array = new byte[base.Text.Length][];
			for (int i = 0; i < base.Text.Length; i++)
			{
				string s = base.Text[i].ToUpperInvariant();
				array[i] = CTSGlobals.AsciiEncoding.GetBytes(s);
			}
			Restriction restriction = Condition.CreateORSearchKeyContentRestriction(array, PropTag.SearchKey, ContentFlags.SubString);
			return new Restriction.RecipientRestriction(restriction);
		}
	}
}
