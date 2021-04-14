using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "NonOwnerAccess")]
	public interface INonOwnerAccess : IGetListService<NonOwnerAccessFilter, NonOwnerAccessResultRow>
	{
	}
}
