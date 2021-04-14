using System;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public enum ObjectType : byte
	{
		Invalid,
		Mailbox,
		Folder,
		Message,
		Attachment,
		EmbeddedMessage,
		Recipient,
		Conversation,
		FolderView,
		AttachmentView,
		PermissionView,
		Event,
		LocalDirectory,
		InferenceLog,
		ViewDefinition,
		IcsState,
		ResourceDigest,
		ProcessInfo,
		FastTransferStream,
		IsIntegJob,
		UserInfo,
		RestrictionView,
		Count
	}
}
