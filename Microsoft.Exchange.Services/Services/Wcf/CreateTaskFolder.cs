using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class CreateTaskFolder : TaskFolderActionBase<TaskFolderActionFolderIdResponse>
	{
		public CreateTaskFolder(MailboxSession session, string newTaskFolderName, string parentGroupGuid) : base(session)
		{
			this.newTaskFolderName = ((newTaskFolderName != null) ? newTaskFolderName.Trim() : null);
			this.parentGroupGuid = parentGroupGuid;
		}

		public override TaskFolderActionFolderIdResponse Execute()
		{
			MailboxSession mailboxSession = base.MailboxSession;
			Microsoft.Exchange.Services.Core.Types.ItemId taskFolderEntryId = null;
			if (string.IsNullOrEmpty(this.newTaskFolderName))
			{
				ExTraceGlobals.CreateTaskFolderCallTracer.TraceError<string>((long)this.GetHashCode(), "TaskFolder name is invalid. Name: '{0}'", (this.newTaskFolderName == null) ? "is null" : this.newTaskFolderName);
				return new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionInvalidTaskFolderName);
			}
			Guid guid;
			if (!Guid.TryParse(this.parentGroupGuid, out guid))
			{
				ExTraceGlobals.CreateTaskFolderCallTracer.TraceError<string, string>((long)this.GetHashCode(), "Parent group id is invalid. TaskFolderName: '{0}'. ParentGroupId: '{1}'", this.newTaskFolderName, (this.parentGroupGuid == null) ? "is null" : this.parentGroupGuid);
				return new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionInvalidGroupId);
			}
			ExTraceGlobals.CreateTaskFolderCallTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "Creating TaskFolder with name: '{0}', ParentGroupId: '{1}'", this.newTaskFolderName, guid);
			FolderId folderIdFromStoreId;
			using (TaskGroup taskGroup = TaskGroup.Bind(mailboxSession, guid))
			{
				ExTraceGlobals.CreateTaskFolderCallTracer.TraceDebug((long)this.GetHashCode(), "Sucessfully bound to parent group. Creating task folder.");
				StoreObjectId objectId;
				byte[] taskFolderRecordKey;
				using (Folder folder = Folder.Create(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Tasks), StoreObjectType.TasksFolder, this.newTaskFolderName, CreateMode.CreateNew))
				{
					try
					{
						FolderSaveResult folderSaveResult = folder.Save();
						if (folderSaveResult.OperationResult != OperationResult.Succeeded)
						{
							ExTraceGlobals.CreateTaskFolderCallTracer.TraceError<string, Guid>((long)this.GetHashCode(), "Unable to create new task folder. Name: '{0}', ParentGroupId: '{1}'", this.newTaskFolderName, guid);
							return new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionUnableToCreateTaskFolder);
						}
					}
					catch (ObjectExistedException)
					{
						return new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionTaskFolderAlreadyExists);
					}
					ExTraceGlobals.CreateTaskFolderCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Successfully created task folder with Name: '{0}'.", this.newTaskFolderName);
					folder.Load();
					objectId = folder.Id.ObjectId;
					folderIdFromStoreId = IdConverter.GetFolderIdFromStoreId(folder.Id, new MailboxId(mailboxSession));
					taskFolderRecordKey = (folder[StoreObjectSchema.RecordKey] as byte[]);
				}
				ExTraceGlobals.CreateTaskFolderCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Creating TaskFolder group entry for TaskFolder. Adding to TaskFolder group. TaskFolderId: '{0}'", (objectId == null) ? "is null" : objectId.ToBase64String());
				using (TaskGroupEntry taskGroupEntry = TaskGroupEntry.Create(mailboxSession, objectId, taskGroup))
				{
					taskGroupEntry.FolderName = this.newTaskFolderName;
					taskGroupEntry.TaskFolderRecordKey = taskFolderRecordKey;
					ConflictResolutionResult conflictResolutionResult = taskGroupEntry.Save(SaveMode.NoConflictResolution);
					if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
					{
						ExTraceGlobals.CreateTaskFolderCallTracer.TraceError<string, string>((long)this.GetHashCode(), "Could not create task group entry. TaskFolderName: '{0}', TaskFolderId: '{1}'", this.newTaskFolderName, (objectId == null) ? "is null" : objectId.ToBase64String());
						AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
						{
							objectId
						});
						if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
						{
							ExTraceGlobals.CreateTaskFolderCallTracer.TraceError<string, string>((long)this.GetHashCode(), "Could not delete created task after creation of task group entry failed. TaskFolderName: '{0}', TaskFolderId: '{1}'", this.newTaskFolderName, (objectId == null) ? "is null" : objectId.ToBase64String());
						}
						return new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionUnableToCreateTaskFolderNode);
					}
					taskGroupEntry.Load();
					taskFolderEntryId = IdConverter.ConvertStoreItemIdToItemId(taskGroupEntry.Id, base.MailboxSession);
				}
			}
			return new TaskFolderActionFolderIdResponse(folderIdFromStoreId, taskFolderEntryId);
		}

		private readonly string newTaskFolderName;

		private readonly string parentGroupGuid;
	}
}
