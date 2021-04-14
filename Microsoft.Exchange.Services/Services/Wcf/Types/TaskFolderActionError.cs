using System;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	public enum TaskFolderActionError
	{
		None,
		TaskFolderActionInvalidGroupId,
		TaskFolderActionCannotSaveGroup,
		TaskFolderActionFolderIdNotTaskFolderFolder,
		TaskFolderActionInvalidTaskFolderName,
		TaskFolderActionUnableToCreateTaskFolder,
		TaskFolderActionUnableToRenameTaskFolder,
		TaskFolderActionTaskFolderAlreadyExists,
		TaskFolderActionUnableToCreateTaskFolderNode,
		TaskFolderActionInvalidItemId,
		TaskFolderActionFolderIdIsDefaultTaskFolder,
		TaskFolderActionCannotRename,
		TaskFolderActionCannotRenameTaskFolderNode,
		TaskFolderActionCannotDeleteTaskFolder,
		TaskFolderActionInvalidTaskFolderNodeOrder,
		TaskFolderActionUnableToUpdateTaskFolderNode,
		TaskFolderActionUnableToFindGroupWithId
	}
}
