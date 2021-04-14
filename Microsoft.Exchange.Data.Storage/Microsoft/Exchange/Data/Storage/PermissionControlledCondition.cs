using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PermissionControlledCondition : FormsCondition
	{
		private PermissionControlledCondition(Rule rule, string[] text) : base(ConditionType.PermissionControlledCondition, rule, text)
		{
		}

		public static PermissionControlledCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			string[] text = new string[]
			{
				"IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA",
				"IPM.Note.rpmsg.Microsoft.Voicemail.UM"
			};
			return new PermissionControlledCondition(rule, text);
		}
	}
}
