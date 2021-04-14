using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class VoicemailCondition : FormsCondition
	{
		private VoicemailCondition(Rule rule, string[] text) : base(ConditionType.VoicemailCondition, rule, text)
		{
		}

		public static VoicemailCondition Create(Rule rule)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			string[] text = new string[]
			{
				"IPM.Note.Microsoft.Voicemail.UM.CA",
				"IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA",
				"IPM.Note.rpmsg.Microsoft.Voicemail.UM",
				"IPM.Note.Microsoft.Voicemail.UM",
				"IPM.Note.Microsoft.Missed.Voice"
			};
			return new VoicemailCondition(rule, text);
		}
	}
}
