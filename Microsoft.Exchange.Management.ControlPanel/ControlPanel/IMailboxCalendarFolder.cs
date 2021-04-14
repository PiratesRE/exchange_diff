using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MailboxCalendarFolder")]
	public interface IMailboxCalendarFolder : IEditObjectService<MailboxCalendarFolderRow, SetMailboxCalendarFolder>, IGetObjectService<MailboxCalendarFolderRow>
	{
		[OperationContract]
		PowerShellResults<MailboxCalendarFolderRow> StartPublishing(Identity identity, SetMailboxCalendarFolder properties);

		[OperationContract]
		PowerShellResults<MailboxCalendarFolderRow> StopPublishing(Identity identity, SetMailboxCalendarFolder properties);
	}
}
