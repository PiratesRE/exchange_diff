using System;
using System.Linq;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	public class Search
	{
		public string UserPrincipalName { get; set; }

		public AuditSearchKind Kind { get; set; }

		public Guid Identity { get; set; }

		public bool Result { get; set; }

		public int RetryAttempt { get; set; }

		public string DiagnosticContext { get; set; }

		public string TenantAcceptedDomain
		{
			get
			{
				if (this.UserPrincipalName == null)
				{
					return null;
				}
				return this.UserPrincipalName.Split(new char[]
				{
					'@'
				}).LastOrDefault<string>();
			}
			set
			{
			}
		}

		public string LastProcessedMailbox { get; set; }

		public ExceptionDetails ExceptionDetails { get; set; }

		public int MailboxCount { get; set; }

		public int ResultCount { get; set; }
	}
}
