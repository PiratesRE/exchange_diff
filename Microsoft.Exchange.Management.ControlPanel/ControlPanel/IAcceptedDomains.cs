using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "AcceptedDomains")]
	public interface IAcceptedDomains : IGetListService<AcceptedDomainFilter, AcceptedDomain>
	{
	}
}
