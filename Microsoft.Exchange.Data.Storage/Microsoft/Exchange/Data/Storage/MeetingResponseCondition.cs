using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingResponseCondition : FormsCondition
	{
		private MeetingResponseCondition(Rule rule, string[] text) : base(ConditionType.MeetingResponseCondition, rule, text)
		{
		}

		public static MeetingResponseCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			string[] text = new string[]
			{
				"IPM.Schedule.Meeting.Resp.Pos",
				"IPM.Schedule.Meeting.Resp.Neg",
				"IPM.Schedule.Meeting.Resp.Tent"
			};
			return new MeetingResponseCondition(rule, text);
		}
	}
}
