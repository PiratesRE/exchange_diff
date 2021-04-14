using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ForwardAsAttachmentToRecipientsAction : RecipientAction
	{
		private ForwardAsAttachmentToRecipientsAction(IList<Participant> participants, Rule rule) : base(ActionType.ForwardAsAttachmentToRecipientsAction, participants, rule)
		{
		}

		public static ForwardAsAttachmentToRecipientsAction Create(IList<Participant> participants, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule,
				participants
			});
			return new ForwardAsAttachmentToRecipientsAction(participants, rule);
		}

		internal override RuleAction BuildRuleAction()
		{
			List<AdrEntry> list = new List<AdrEntry>();
			for (int i = 0; i < base.Participants.Count; i++)
			{
				list.Add(Rule.AdrEntryFromParticipant(base.Participants[i]));
			}
			return new RuleAction.Forward(list.ToArray(), RuleAction.Forward.ActionFlags.ForwardAsAttachment);
		}
	}
}
