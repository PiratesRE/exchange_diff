using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ApprovalRequestCondition : FormsCondition
	{
		private ApprovalRequestCondition(Rule rule, string[] text) : base(ConditionType.ApprovalRequestCondition, rule, text)
		{
		}

		public static ApprovalRequestCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new ApprovalRequestCondition(rule, new string[]
			{
				"IPM.Note.Microsoft.Approval.Request"
			});
		}
	}
}
