using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MailboxFolders")]
	public interface IMailboxFolders : IGetListService<MailboxFolderFilter, MailboxFolder>, INewObjectService<MailboxFolder, NewMailboxFolder>
	{
	}
}
