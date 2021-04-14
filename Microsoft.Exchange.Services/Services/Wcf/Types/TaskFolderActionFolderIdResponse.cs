using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class TaskFolderActionFolderIdResponse : TaskFolderActionResponse
	{
		public TaskFolderActionFolderIdResponse(TaskFolderActionError errorCode) : base(errorCode)
		{
		}

		public TaskFolderActionFolderIdResponse(FolderId folderId, ItemId taskFolderEntryId)
		{
			this.NewFolderId = folderId;
			this.NewTaskFolderEntryId = taskFolderEntryId;
		}

		[DataMember]
		public FolderId NewFolderId { get; set; }

		[DataMember]
		public ItemId NewTaskFolderEntryId { get; set; }
	}
}
