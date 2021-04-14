using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ImapSubscriptions")]
	public interface IImapSubscriptions : INewObjectService<PimSubscriptionRow, NewImapSubscription>, IEditObjectService<ImapSubscription, SetImapSubscription>, IGetObjectService<ImapSubscription>
	{
	}
}
