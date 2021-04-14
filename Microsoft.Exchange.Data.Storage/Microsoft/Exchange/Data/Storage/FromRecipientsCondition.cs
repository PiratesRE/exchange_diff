using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FromRecipientsCondition : RecipientCondition
	{
		private FromRecipientsCondition(Rule rule, IList<Participant> participants) : base(ConditionType.FromRecipientsCondition, rule, participants)
		{
		}

		public static FromRecipientsCondition Create(Rule rule, IList<Participant> participants)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				participants
			});
			RecipientCondition.CheckParticipants(rule, participants);
			return new FromRecipientsCondition(rule, participants);
		}

		internal override Restriction BuildRestriction()
		{
			return Condition.CreateFromRestriction(base.Participants);
		}
	}
}
