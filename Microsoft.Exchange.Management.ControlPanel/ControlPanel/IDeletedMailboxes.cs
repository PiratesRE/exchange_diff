using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "DeletedMailboxes")]
	public interface IDeletedMailboxes : IGetListService<DeletedMailboxFilter, DeletedMailboxRow>
	{
	}
}
