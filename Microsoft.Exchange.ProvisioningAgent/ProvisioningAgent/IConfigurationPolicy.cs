using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal interface IConfigurationPolicy
	{
		ArbitrationMailboxStatus CheckArbitrationMailboxStatus(out Exception initialError);

		IAuditLog CreateLogger(ArbitrationMailboxStatus mailboxStatus);

		IAdminAuditLogConfig GetAdminAuditLogConfig();

		bool RunningOnDataCenter { get; }

		OrganizationId OrganizationId { get; }

		IExchangePrincipal ExchangePrincipal { get; }
	}
}
