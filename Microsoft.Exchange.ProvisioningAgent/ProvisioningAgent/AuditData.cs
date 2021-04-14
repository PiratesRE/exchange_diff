using System;
using Microsoft.Exchange.Data.Storage.Auditing;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class AuditData
	{
		public IAuditLogRecord AuditRecord { get; set; }

		public IAuditLog AuditLogger { get; set; }
	}
}
