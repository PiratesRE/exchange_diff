using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MailRoutingDomains")]
	public interface IMailRoutingDomains : IGetListService<MailRoutingDomainFilter, MailRoutingDomain>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectService<MailRoutingDomain, SetMailRoutingDomain>, IGetObjectService<MailRoutingDomain>
	{
	}
}
