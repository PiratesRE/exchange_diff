using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SentToRecipientsCondition : RecipientCondition
	{
		private SentToRecipientsCondition(Rule rule, IList<Participant> participants) : base(ConditionType.SentToRecipientsCondition, rule, participants)
		{
		}

		public static SentToRecipientsCondition Create(Rule rule, IList<Participant> participants)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				participants
			});
			RecipientCondition.CheckParticipants(rule, participants);
			return new SentToRecipientsCondition(rule, participants);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateRecipientRestriction(base.Participants);
		}
	}
}
