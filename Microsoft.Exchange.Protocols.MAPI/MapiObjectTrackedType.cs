using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public enum MapiObjectTrackedType : uint
	{
		Session,
		Logon,
		Message,
		Attachment,
		Folder,
		Notify,
		Stream,
		MessageView,
		FolderView,
		AttachmentView,
		PermissionView,
		FastTransferSource,
		FastTransferDestination,
		UntrackedObject,
		MaxTrackedObjectType = 13U
	}
}
