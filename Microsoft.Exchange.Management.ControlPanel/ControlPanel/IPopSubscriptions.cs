using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "PopSubscriptions")]
	public interface IPopSubscriptions : INewObjectService<PimSubscriptionRow, NewPopSubscription>, IEditObjectService<PopSubscription, SetPopSubscription>, IGetObjectService<PopSubscription>
	{
	}
}
