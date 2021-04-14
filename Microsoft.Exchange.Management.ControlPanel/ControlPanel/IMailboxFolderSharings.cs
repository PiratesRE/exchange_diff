using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MailboxFolderSharings")]
	public interface IMailboxFolderSharings : IGetListService<MailboxFolderPermissionFilter, MailboxFolderPermissionRow>, IEditObjectService<UserMailboxFolderPermission, SetUserMailboxFolderPermission>, IGetObjectService<UserMailboxFolderPermission>, IRemoveObjectsService, IRemoveObjectsService<BaseWebServiceParameters>
	{
	}
}
