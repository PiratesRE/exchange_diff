using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal struct GccRuleEntry
	{
		public GccRuleEntry(Guid immutableId, string ruleName, SmtpAddress recipient, bool fullReport, DateTime? expiryDate, SmtpAddress journalEmailAddress)
		{
			this.ImmutableId = immutableId;
			this.Name = ruleName;
			this.Recipient = recipient;
			this.FullReport = fullReport;
			this.ExpiryDate = expiryDate;
			this.JournalEmailAddress = journalEmailAddress;
		}

		public Guid ImmutableId;

		public string Name;

		public SmtpAddress Recipient;

		public bool FullReport;

		public DateTime? ExpiryDate;

		public SmtpAddress JournalEmailAddress;
	}
}
