using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMMailbox")]
	public interface IUMMailboxService : IEditObjectService<SetUMMailboxConfiguration, SetUMMailboxParameters>, IGetObjectService<SetUMMailboxConfiguration>, INewGetObjectService<UMMailboxFeatureInfo, NewUMMailboxParameters, RecipientRow>, INewObjectService<UMMailboxFeatureInfo, NewUMMailboxParameters>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[OperationContract]
		PowerShellResults<NewUMMailboxConfiguration> GetConfigurationForNewUMMailbox(Identity identity, UMEnableSelectedPolicyParameters properties);
	}
}
