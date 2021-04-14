using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public enum MapiObjectType : uint
	{
		Invalid,
		Attachment,
		Event,
		Folder,
		Logon,
		Message,
		EmbeddedMessage,
		Person,
		FastTransferContext,
		Notify,
		Stream,
		MessageView,
		FolderView,
		AttachmentView,
		IcsUploadContext,
		FastTransferStream
	}
}
