using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface ITaskContext
	{
		HybridConfiguration HybridConfigurationObject { get; }

		IContextParameters Parameters { get; }

		ILogger Logger { get; }

		IOnPremisesSession OnPremisesSession { get; }

		ITenantSession TenantSession { get; }

		IList<LocalizedString> Errors { get; }

		IList<LocalizedString> Warnings { get; }

		IUserInterface UI { get; }
	}
}
