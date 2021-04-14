using System;

namespace Microsoft.Exchange.Servicelets.AuditLogSearch
{
	public class MailboxConnectivity : HealthHandlerResult
	{
		public string TenantAcceptedDomain { get; set; }

		public Guid ExchangeUserId { get; set; }

		public bool Success { get; set; }

		public string Exception { get; set; }
	}
}
