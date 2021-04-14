using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[KnownType(typeof(GetUserAvailabilityRequest))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class BaseAvailabilityRequest : BaseRequest
	{
		internal override void UpdatePerformanceCounters(ServiceCommandBase serviceCommand, IExchangeWebMethodResponse response)
		{
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override bool ShouldReturnXDBMountedOn()
		{
			return false;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
