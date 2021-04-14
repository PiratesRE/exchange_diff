using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingMessageCondition : FormsCondition
	{
		private MeetingMessageCondition(Rule rule, string[] text) : base(ConditionType.MeetingMessageCondition, rule, text)
		{
		}

		public static MeetingMessageCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			string[] text = new string[]
			{
				"IPM.Schedule.Meeting.Request",
				"IPM.Schedule.Meeting.Canceled"
			};
			return new MeetingMessageCondition(rule, text);
		}
	}
}
