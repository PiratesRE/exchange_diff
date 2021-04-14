using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IpSafeListings")]
	public interface IIpSafeListings : IEditObjectService<IpSafeListing, SetIpSafeListing>, IGetObjectService<IpSafeListing>
	{
	}
}
