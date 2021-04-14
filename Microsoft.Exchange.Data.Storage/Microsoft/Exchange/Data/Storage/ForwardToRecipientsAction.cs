using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ForwardToRecipientsAction : RecipientAction
	{
		private ForwardToRecipientsAction(IList<Participant> participants, Rule rule) : base(ActionType.ForwardToRecipientsAction, participants, rule)
		{
		}

		public static ForwardToRecipientsAction Create(IList<Participant> participants, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule,
				participants
			});
			return new ForwardToRecipientsAction(participants, rule);
		}

		internal override RuleAction BuildRuleAction()
		{
			List<AdrEntry> list = new List<AdrEntry>();
			for (int i = 0; i < base.Participants.Count; i++)
			{
				list.Add(Rule.AdrEntryFromParticipant(base.Participants[i]));
			}
			return new RuleAction.Forward(list.ToArray(), RuleAction.Forward.ActionFlags.None);
		}
	}
}
