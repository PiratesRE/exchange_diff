using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ActionOrder
	{
		ServerReplyMessageAction = 1,
		MarkImportanceAction,
		MarkSensitivityAction,
		AssignCategoriesAction,
		FlagMessageAction,
		MarkAsReadAction,
		ForwardToRecipientsAction,
		RedirectToRecipientsAction,
		ForwardAsAttachmentToRecipientsAction,
		SendSmsAlertToRecipientsAction,
		StopProcessingAction,
		CopyToFolderAction,
		MoveToFolderAction,
		DeleteAction,
		PermanentDeleteAction
	}
}
