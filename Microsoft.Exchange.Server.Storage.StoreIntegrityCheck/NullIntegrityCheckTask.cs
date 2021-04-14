using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreIntegrityCheck;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class NullIntegrityCheckTask : IntegrityCheckTaskBase
	{
		public NullIntegrityCheckTask(TaskId unsupportedTaskId, IJobExecutionTracker tracker) : base(tracker)
		{
			this.unsupportedTaskId = unsupportedTaskId;
		}

		public override string TaskName
		{
			get
			{
				return "NotSupportedIntegrityCheckTask";
			}
		}

		public override ErrorCode Execute(Context context, MailboxEntry mailboxEntry, bool detectOnly, Func<bool> shouldContinue)
		{
			if (ExTraceGlobals.OnlineIsintegTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.OnlineIsintegTracer.TraceDebug<Guid, TaskId>(0L, "Unsupported task invoked on mailbox {0}, task id= {1}", mailboxEntry.MailboxGuid, this.unsupportedTaskId);
			}
			TaskId taskId = this.unsupportedTaskId;
			switch (taskId)
			{
			case TaskId.FolderView:
			case TaskId.ProvisionedFolder:
			case TaskId.ReplState:
			case TaskId.MessagePtagCn:
				return ErrorCode.NoError;
			case TaskId.AggregateCounts:
				break;
			default:
				switch (taskId)
				{
				case TaskId.Extension1:
				case TaskId.Extension2:
				case TaskId.Extension3:
				case TaskId.Extension4:
				case TaskId.Extension5:
					return ErrorCode.NoError;
				}
				break;
			}
			return ErrorCode.CreateNotSupported((LID)44124U);
		}

		private readonly TaskId unsupportedTaskId;
	}
}
