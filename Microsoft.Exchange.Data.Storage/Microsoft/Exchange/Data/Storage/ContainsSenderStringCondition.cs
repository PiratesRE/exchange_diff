using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContainsSenderStringCondition : StringCondition
	{
		private ContainsSenderStringCondition(Rule rule, string[] text) : base(ConditionType.ContainsSenderStringCondition, rule, text)
		{
		}

		public static ContainsSenderStringCondition Create(Rule rule, string[] text)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				text
			});
			return new ContainsSenderStringCondition(rule, text);
		}

		internal override Restriction BuildRestriction()
		{
			byte[][] array = new byte[base.Text.Length][];
			for (int i = 0; i < base.Text.Length; i++)
			{
				string s = base.Text[i].ToUpperInvariant();
				array[i] = CTSGlobals.AsciiEncoding.GetBytes(s);
			}
			return Condition.CreateORSearchKeyContentRestriction(array, PropTag.SenderSearchKey, ContentFlags.SubString);
		}
	}
}
