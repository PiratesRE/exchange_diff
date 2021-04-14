using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class CreateTaskFolderCommand : ServiceCommand<TaskFolderActionFolderIdResponse>
	{
		public CreateTaskFolderCommand(CallContext callContext, string newTaskFolderName, string parentGroupGuid) : base(callContext)
		{
			this.newTaskFolderName = newTaskFolderName;
			this.parentGroupGuid = parentGroupGuid;
		}

		protected override TaskFolderActionFolderIdResponse InternalExecute()
		{
			TaskFolderActionFolderIdResponse result;
			try
			{
				result = new CreateTaskFolder(base.MailboxIdentityMailboxSession, this.newTaskFolderName, this.parentGroupGuid).Execute();
			}
			catch (StoragePermanentException ex)
			{
				ExTraceGlobals.CreateTaskFolderCallTracer.TraceError((long)this.GetHashCode(), "StoragePermanentException thrown while trying to create TaskFolder with name: {0}. ParentGroupGuid: {1}, ExceptionInfo: {2}. CallStack: {3}", new object[]
				{
					(this.newTaskFolderName == null) ? "is null" : this.newTaskFolderName,
					(this.parentGroupGuid == null) ? "is null" : this.parentGroupGuid,
					ex.Message,
					ex.StackTrace
				});
				result = new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionUnableToCreateTaskFolder);
			}
			catch (StorageTransientException ex2)
			{
				ExTraceGlobals.CreateTaskFolderCallTracer.TraceError((long)this.GetHashCode(), "StorageTransientException thrown while trying to create TaskFolder with name: {0}. ParentGroupGuid: {1}, ExceptionInfo: {2}. CallStack: {3}", new object[]
				{
					(this.newTaskFolderName == null) ? "is null" : this.newTaskFolderName,
					(this.parentGroupGuid == null) ? "is null" : this.parentGroupGuid,
					ex2.Message,
					ex2.StackTrace
				});
				result = new TaskFolderActionFolderIdResponse(TaskFolderActionError.TaskFolderActionUnableToCreateTaskFolder);
			}
			return result;
		}

		private readonly string newTaskFolderName;

		private readonly string parentGroupGuid;
	}
}
