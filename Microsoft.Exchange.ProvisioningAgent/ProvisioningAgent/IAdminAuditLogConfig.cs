using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal interface IAdminAuditLogConfig
	{
		MultiValuedProperty<string> AdminAuditLogParameters { get; }

		MultiValuedProperty<string> AdminAuditLogCmdlets { get; }

		MultiValuedProperty<string> AdminAuditLogExcludedCmdlets { get; }

		bool AdminAuditLogEnabled { get; }

		bool IsValidAuditLogMailboxAddress { get; }

		bool TestCmdletLoggingEnabled { get; }

		AuditLogLevel LogLevel { get; }
	}
}
