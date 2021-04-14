using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "Organization")]
	public interface IOrganizationService : IAsyncService
	{
		[OperationContract]
		PowerShellResults EnableOrganizationCustomization();
	}
}
