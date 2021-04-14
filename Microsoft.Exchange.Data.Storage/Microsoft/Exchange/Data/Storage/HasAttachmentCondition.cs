using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class HasAttachmentCondition : Condition
	{
		private HasAttachmentCondition(Rule rule) : base(ConditionType.HasAttachmentCondition, rule)
		{
		}

		public static HasAttachmentCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new HasAttachmentCondition(rule);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateHasAttachmentRestriction();
		}
	}
}
