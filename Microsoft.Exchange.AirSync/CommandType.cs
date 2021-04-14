using System;

namespace Microsoft.Exchange.AirSync
{
	internal enum CommandType
	{
		Unknown,
		Options,
		GetHierarchy,
		Sync,
		GetItemEstimate,
		FolderSync,
		FolderUpdate,
		FolderDelete,
		FolderCreate,
		CreateCollection,
		MoveCollection,
		DeleteCollection,
		GetAttachment,
		MoveItems,
		MeetingResponse,
		SendMail,
		SmartReply,
		SmartForward,
		Search,
		Settings,
		Ping,
		ItemOperations,
		Provision,
		ResolveRecipients,
		ValidateCert,
		ProxyLogin
	}
}
