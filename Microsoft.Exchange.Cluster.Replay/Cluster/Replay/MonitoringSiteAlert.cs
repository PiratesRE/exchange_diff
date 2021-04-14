using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class MonitoringSiteAlert : MonitoringAlert
	{
		protected MonitoringSiteAlert(string identity, Guid identityGuid) : base(identity, identityGuid)
		{
		}

		protected override bool IsEnabled
		{
			get
			{
				return !RegistryParameters.DatabaseHealthCheckSiteAlertsDisabled;
			}
		}

		protected override bool IsValidationSuccessful(IHealthValidationResultMinimal result)
		{
			return result.IsSiteValidationSuccessful;
		}
	}
}
