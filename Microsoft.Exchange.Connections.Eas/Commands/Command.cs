using System;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	internal enum Command
	{
		Autodiscover,
		Connect,
		CreateCollection,
		DeleteCollection,
		Disconnect,
		FolderSync,
		FolderCreate,
		FolderDelete,
		FolderUpdate,
		GetAttachment,
		GetHierarchy,
		GetItemEstimate,
		ItemOperations,
		MeetingResponse,
		MoveCollection,
		MoveItems,
		Options,
		Ping,
		Provision,
		ResolveRecipients,
		Search,
		SendMail,
		Settings,
		SmartForward,
		SmartReply,
		Sync,
		ValidateCert,
		InvalidCommand
	}
}
