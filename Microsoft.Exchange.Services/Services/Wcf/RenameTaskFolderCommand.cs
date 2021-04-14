using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class RenameTaskFolderCommand : ServiceCommand<TaskFolderActionFolderIdResponse>
	{
		public RenameTaskFolderCommand(CallContext callContext, Microsoft.Exchange.Services.Core.Types.ItemId itemId, string newTaskFolderName) : base(callContext)
		{
			this.itemId = itemId;
			this.newTaskFolderName = newTaskFolderName;
		}

		protected override TaskFolderActionFolderIdResponse InternalExecute()
		{
			MailboxSession mailboxIdentityMailboxSession = base.MailboxIdentityMailboxSession;
			if (this.itemId == null || string.IsNullOrEmpty(this.itemId.Id) || string.IsNullOrEmpty(this.itemId.ChangeKey))
			{
				ExTraceGlobals.RenameTaskFolderCallTracer.TraceError<string, string>((long)this.GetHashCode(), "ItemId provided is invalid. ItemId.Id: '{0}' ItemId.ChangeKey: '{1}'", (this.itemId == null || this.itemId.Id == null) ? "is null" : this.itemId.Id, (this.itemId == null || this.itemId.ChangeKey == null) ? "is null" : this.itemId.ChangeKey);
				return new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionInvalidItemId);
			}
			TaskFolderActionFolderIdResponse result;
			try
			{
				IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadWrite(this.itemId);
				StoreId id = idAndSession.Id;
				result = new RenameTaskFolder(base.MailboxIdentityMailboxSession, id, this.newTaskFolderName).Execute();
			}
			catch (StoragePermanentException ex)
			{
				ExTraceGlobals.RenameTaskFolderCallTracer.TraceError((long)this.GetHashCode(), "StoragePermanentException thrown while trying to rename TaskFolder. NewName: {0}. ItemId.Id: {1}, ItemId.ChangeKey: {2} ExceptionInfo: {3}. CallStack: {4}", new object[]
				{
					(this.newTaskFolderName == null) ? "is null" : this.newTaskFolderName,
					this.itemId.Id,
					this.itemId.ChangeKey,
					ex.Message,
					ex.StackTrace
				});
				result = new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionUnableToRenameTaskFolder);
			}
			catch (StorageTransientException ex2)
			{
				ExTraceGlobals.RenameTaskFolderCallTracer.TraceError((long)this.GetHashCode(), "StorageTransientException thrown while trying to rename TaskFolder. NewName: {0}. ItemId.Id: {1}, ItemId.ChangeKey: {2} ExceptionInfo: {3}. CallStack: {4}", new object[]
				{
					(this.newTaskFolderName == null) ? "is null" : this.newTaskFolderName,
					this.itemId.Id,
					this.itemId.ChangeKey,
					ex2.Message,
					ex2.StackTrace
				});
				result = new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionUnableToRenameTaskFolder);
			}
			return result;
		}

		private readonly Microsoft.Exchange.Services.Core.Types.ItemId itemId;

		private readonly string newTaskFolderName;
	}
}
