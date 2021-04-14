﻿using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal enum AccessCheckOperation
	{
		PropertyGet,
		PropertySet,
		PropertyDelete,
		PropertyGetList,
		StreamOpen,
		StreamRead,
		StreamWrite,
		StreamSeek,
		StreamGetSize,
		StreamSetSize,
		FolderOpen,
		FolderCreate,
		FolderMoveMessageSource,
		FolderMoveMessageDestination,
		FolderCopyMessageSource,
		FolderCopyMessageDestination,
		FolderDeleteMessage,
		FolderSetReadFlag,
		FolderImportMessageMoveSource,
		FolderImportMessageMoveDestination,
		FolderGetMessageStatus,
		FolderShallowCopySource,
		FolderShallowCopyDestination,
		FolderMoveSource,
		FolderMoveDestination,
		FolderDelete,
		FolderSetSearchCriteria,
		FolderGetSearchCriteria,
		FolderCopyPropsSource,
		FolderCopyPropsDestination,
		FolderSetMessageStatus,
		FolderViewMessage,
		FolderViewHierarchy,
		MessageCreate,
		MessageOpen,
		MessageCreateEmbedded,
		MessageOpenEmbedded,
		MessageSubmit,
		MessageSaveChanges,
		MessageSetReadFlag,
		MessageSetMessageFlags,
		AttachmentCreate,
		AttachmentOpen,
		AttachmentSaveChanges,
		AttachmentOpenEmbeddedMessage
	}
}
