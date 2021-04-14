using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SoftDeletedMailboxes")]
	public interface ISoftDeletedMailboxes : IGetListService<SoftDeletedMailboxFilter, SoftDeletedMailboxRow>
	{
	}
}
