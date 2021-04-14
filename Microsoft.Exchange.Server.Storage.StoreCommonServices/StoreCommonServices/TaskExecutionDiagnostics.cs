using System;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class TaskExecutionDiagnostics : ExecutionDiagnostics
	{
		public TaskExecutionDiagnostics(TaskTypeId taskTypeId, Guid clientActivityId, string clientComponentName, string clientProtocolName, string clientActionString)
		{
			ILogTransactionInformation logTransactionInformationBlock = new LogTransactionInformationTask(taskTypeId);
			base.LogTransactionInformationCollector.AddLogTransactionInformationBlock(logTransactionInformationBlock);
			this.taskTypeId = taskTypeId;
			base.SetClientActivityInfo(clientActivityId, clientComponentName, clientProtocolName, clientActionString);
		}

		public override byte OpNumber
		{
			get
			{
				return (byte)this.taskTypeId;
			}
		}

		public TaskTypeId TaskTypeId
		{
			get
			{
				return this.taskTypeId;
			}
		}

		public override uint TypeIdentifier
		{
			get
			{
				return 1U;
			}
		}

		internal TaskExMonLogger TaskExMonLogger
		{
			get
			{
				return this.taskExmonLogger;
			}
			set
			{
				this.taskExmonLogger = value;
			}
		}

		protected override void FormatOperationInformation(TraceContentBuilder cb, int indentLevel)
		{
			base.FormatOperationInformation(cb, indentLevel);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "TaskId: " + this.taskTypeId);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ "Task".GetHashCode() ^ this.taskTypeId.GetHashCode();
		}

		internal virtual void OnBeforeTask(IRopSummaryCollector ropSummaryCollector)
		{
			base.SummaryCollector = ((ropSummaryCollector != null) ? ropSummaryCollector : RopSummaryCollector.Null);
			base.OnBeginOperation();
		}

		internal void OnTaskEnd()
		{
			base.OnEndOperation(OperationType.Task, ClientActivityStrings.Task, (byte)this.taskTypeId, 0U, false);
			base.DumpDiagnosticIfNeeded();
		}

		protected override void GetSummaryInformation(Guid correlationId, ref ExecutionDiagnostics.LongOperationSummary summary)
		{
			base.GetSummaryInformation(correlationId, ref summary);
			summary.OperationType = "Task";
			summary.OperationName = this.taskTypeId.ToString();
		}

		private readonly TaskTypeId taskTypeId;

		private TaskExMonLogger taskExmonLogger;
	}
}
