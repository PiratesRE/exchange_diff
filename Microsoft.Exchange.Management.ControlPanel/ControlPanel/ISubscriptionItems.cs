using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SubscriptionItems")]
	public interface ISubscriptionItems : IGetListService<SubscriptionItemFilter, SubscriptionItem>
	{
	}
}
