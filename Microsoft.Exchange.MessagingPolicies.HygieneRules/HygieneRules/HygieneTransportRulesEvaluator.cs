using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.HygieneRules
{
	internal sealed class HygieneTransportRulesEvaluator : RulesEvaluator
	{
		public HygieneTransportRulesEvaluator(HygieneTransportRulesEvaluationContext context) : base(context)
		{
			this.context = context;
		}

		protected override ExecutionControl EnterRule()
		{
			HygieneTransportRule hygieneTransportRule = (HygieneTransportRule)this.context.CurrentRule;
			RuleCollection rules = this.context.Rules;
			if (hygieneTransportRule.Fork != null && hygieneTransportRule.Fork.Count > 0)
			{
				MailItem mailItem = this.context.MailItem;
				List<EnvelopeRecipient> list = null;
				foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
				{
					string recipient = envelopeRecipient.Address.ToString();
					if (this.RecipientMatchesForkInfo(hygieneTransportRule, recipient, this.context.Server))
					{
						if (list == null)
						{
							list = new List<EnvelopeRecipient>();
						}
						list.Add(envelopeRecipient);
					}
				}
				if (list == null)
				{
					this.ExitRule();
					return ExecutionControl.SkipThis;
				}
				if (mailItem.Recipients.Count != list.Count)
				{
					this.context.MatchedRecipients = list;
				}
			}
			return ExecutionControl.Execute;
		}

		protected override bool EnterRuleActionBlock()
		{
			if (this.context.MatchedRecipients != null && this.context.MatchedRecipients.Count > 0)
			{
				if (this.context.EventSource != null)
				{
					this.context.EventSource.Fork(this.context.MatchedRecipients);
				}
				this.context.MatchedRecipients = null;
			}
			return true;
		}

		private bool RecipientMatchesForkInfo(HygieneTransportRule rule, string recipient, SmtpServer server)
		{
			bool flag = true;
			for (int i = 0; i < rule.Fork.Count; i++)
			{
				if (!rule.Fork[i].Exception)
				{
					BifurcationInfo bifInfo = rule.Fork[i];
					if (!this.MatchesSingleBifurcationInfo(recipient, bifInfo, server))
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				for (int j = 0; j < rule.Fork.Count; j++)
				{
					if (rule.Fork[j].Exception)
					{
						BifurcationInfo bifInfo2 = rule.Fork[j];
						if (this.MatchesSingleBifurcationInfo(recipient, bifInfo2, server))
						{
							flag = false;
						}
					}
				}
			}
			return flag;
		}

		private bool MatchesSingleBifurcationInfo(string recipient, BifurcationInfo bifInfo, SmtpServer server)
		{
			foreach (string x in bifInfo.Recipients)
			{
				if (this.context.UserComparer.Equals(x, recipient))
				{
					return true;
				}
			}
			foreach (string text in bifInfo.Lists)
			{
				if (recipient.Equals(text, StringComparison.InvariantCultureIgnoreCase) || this.context.MembershipChecker.Equals(recipient, text))
				{
					return true;
				}
			}
			foreach (string domain in bifInfo.RecipientDomainIs)
			{
				string domain2 = new SmtpAddress(recipient).Domain;
				if (!string.IsNullOrEmpty(domain2) && DomainIsPredicate.IsSubdomainOf(domain, domain2))
				{
					return true;
				}
			}
			return false;
		}

		private readonly HygieneTransportRulesEvaluationContext context;
	}
}
