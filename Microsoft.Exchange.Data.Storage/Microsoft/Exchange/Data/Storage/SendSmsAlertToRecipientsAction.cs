using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SendSmsAlertToRecipientsAction : RecipientAction
	{
		private SendSmsAlertToRecipientsAction(IList<Participant> participants, Rule rule) : base(ActionType.SendSmsAlertToRecipientsAction, participants, rule)
		{
		}

		public static SendSmsAlertToRecipientsAction Create(IList<Participant> participants, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule,
				participants
			});
			return new SendSmsAlertToRecipientsAction(participants, rule);
		}

		public override Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return Rule.ProviderIdEnum.Exchange14;
			}
		}

		internal override RuleAction BuildRuleAction()
		{
			List<AdrEntry> list = new List<AdrEntry>();
			for (int i = 0; i < base.Participants.Count; i++)
			{
				list.Add(Rule.AdrEntryFromParticipant(base.Participants[i]));
			}
			return new RuleAction.Forward(list.ToArray(), RuleAction.Forward.ActionFlags.SendSmsAlert);
		}
	}
}
