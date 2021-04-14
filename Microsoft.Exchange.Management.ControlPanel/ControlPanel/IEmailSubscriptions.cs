using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "EmailSubscriptions")]
	public interface IEmailSubscriptions : INewObjectService<PimSubscription, NewSubscription>, IGetListService<EmailSubscriptionFilter, PimSubscriptionRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[OperationContract]
		PowerShellResults<PimSubscriptionRow> ResendPopVerificationEmail(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults<PimSubscriptionRow> ResendImapVerificationEmail(Identity[] identities, BaseWebServiceParameters parameters);
	}
}
