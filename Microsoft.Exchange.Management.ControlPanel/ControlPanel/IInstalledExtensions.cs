using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "InstalledExtensions")]
	public interface IInstalledExtensions : IGetListService<ExtensionFilter, ExtensionRow>, IGetObjectService<ExtensionRow>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
		[OperationContract]
		PowerShellResults<ExtensionRow> Disable(Identity[] identities, BaseWebServiceParameters parameters);

		[OperationContract]
		PowerShellResults<ExtensionRow> Enable(Identity[] identities, BaseWebServiceParameters parameters);
	}
}
