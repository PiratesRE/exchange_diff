using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "HotmailSubscriptions")]
	public interface IHotmailSubscriptions : IEditObjectService<HotmailSubscription, SetHotmailSubscription>, IGetObjectService<HotmailSubscription>
	{
	}
}
