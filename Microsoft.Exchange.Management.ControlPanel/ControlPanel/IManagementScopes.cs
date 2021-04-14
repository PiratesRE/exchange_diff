using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ManagementScopes")]
	public interface IManagementScopes : IGetListService<ManagementScopeFilter, ManagementScopeRow>
	{
	}
}
