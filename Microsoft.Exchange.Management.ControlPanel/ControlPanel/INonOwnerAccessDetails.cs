using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "NonOwnerAccessDetails")]
	public interface INonOwnerAccessDetails : IGetObjectService<NonOwnerAccessDetailRow>
	{
	}
}
