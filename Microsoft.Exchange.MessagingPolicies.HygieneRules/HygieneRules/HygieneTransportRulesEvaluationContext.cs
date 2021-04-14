using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.HygieneRules
{
	internal class HygieneTransportRulesEvaluationContext : RulesEvaluationContext
	{
		public HygieneTransportRulesEvaluationContext(RuleCollection rules, SmtpServer server, QueuedMessageEventSource eventSource, MailItem mailItem) : base(rules)
		{
			this.MailItem = mailItem;
			this.Server = server;
			this.EventSource = eventSource;
			if (this.Server != null)
			{
				this.UserComparer = new UserComparer(this.Server.AddressBook);
				this.MembershipChecker = new MembershipChecker(this.Server.AddressBook);
			}
		}

		public MailItem MailItem { get; private set; }

		public SmtpServer Server { get; private set; }

		public List<EnvelopeRecipient> MatchedRecipients { get; set; }

		public QueuedMessageEventSource EventSource { get; private set; }

		public IStringComparer UserComparer { get; private set; }

		public IStringComparer MembershipChecker { get; private set; }
	}
}
