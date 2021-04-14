using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ActionType
	{
		MoveToFolderAction = 1,
		DeleteAction,
		CopyToFolderAction,
		ForwardToRecipientsAction,
		ForwardAsAttachmentToRecipientsAction,
		RedirectToRecipientsAction,
		ServerReplyMessageAction,
		MarkImportanceAction,
		MarkSensitivityAction,
		StopProcessingAction,
		SendSmsAlertToRecipientsAction,
		AssignCategoriesAction,
		PermanentDeleteAction,
		FlagMessageAction,
		MarkAsReadAction
	}
}
