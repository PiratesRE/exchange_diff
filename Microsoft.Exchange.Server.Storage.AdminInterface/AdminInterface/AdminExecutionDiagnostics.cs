using System;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.MapiDisp;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	internal class AdminExecutionDiagnostics : RpcExecutionDiagnostics
	{
		public AdminExecutionDiagnostics(AdminMethod methodId, int operationDetail)
		{
			this.methodId = methodId;
			base.OpSource = ExecutionDiagnostics.OperationSource.AdminRpc;
			base.OpDetail = operationDetail;
		}

		public override byte OpNumber
		{
			get
			{
				return (byte)this.methodId;
			}
		}

		public override uint TypeIdentifier
		{
			get
			{
				return 4U;
			}
		}

		internal AdminExMonLogger AdminExMonLogger
		{
			get
			{
				return this.adminExmonLogger;
			}
			set
			{
				this.adminExmonLogger = value;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ "Admin".GetHashCode() ^ this.methodId.GetHashCode();
		}

		internal virtual void OnBeforeRpc(Guid databaseGuid, IRopSummaryCollector ropSummaryCollector)
		{
			base.DatabaseGuid = databaseGuid;
			base.SummaryCollector = ((ropSummaryCollector != null) ? ropSummaryCollector : RopSummaryCollector.Null);
		}

		internal new void OnRpcBegin()
		{
			base.OnRpcBegin();
			if (this.adminExmonLogger != null)
			{
				JET_THREADSTATS threadStats;
				Factory.GetDatabaseThreadStats(out threadStats);
				this.adminExmonLogger.BeginRpcProcessing(threadStats);
			}
			ILogTransactionInformation logTransactionInformationBlock = new LogTransactionInformationAdmin(this.methodId);
			base.LogTransactionInformationCollector.AddLogTransactionInformationBlock(logTransactionInformationBlock);
		}

		internal void OnRpcEnd(ErrorCodeValue errorCode)
		{
			base.OnRpcEnd();
			if (this.adminExmonLogger != null)
			{
				JET_THREADSTATS threadStats;
				Factory.GetDatabaseThreadStats(out threadStats);
				this.adminExmonLogger.EndRpcProcessing((uint)this.methodId, threadStats, (uint)errorCode);
			}
			base.DumpDiagnosticIfNeeded();
			base.OnEndRpc(OperationType.Admin, ClientActivityStrings.Admin, (byte)this.methodId, (uint)errorCode, false);
		}

		internal override void EnablePerClientTypePerfCounterUpdate()
		{
			base.EnablePerClientTypePerfCounterUpdate();
			if (base.PerClientPerfInstance != null)
			{
				base.PerClientPerfInstance.AdminRPCsInProgress.Increment();
			}
		}

		internal override void DisablePerClientTypePerfCounterUpdate()
		{
			if (base.PerClientPerfInstance != null)
			{
				base.PerClientPerfInstance.AdminRPCsInProgress.Decrement();
				base.PerClientPerfInstance.AdminRPCsRateOfExecuteTask.Increment();
			}
			base.DisablePerClientTypePerfCounterUpdate();
		}

		protected override void FormatOperationInformation(TraceContentBuilder cb, int indentLevel)
		{
			base.FormatOperationInformation(cb, indentLevel);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "AdminMethod: " + this.methodId);
		}

		protected override void GetSummaryInformation(Guid correlationId, ref ExecutionDiagnostics.LongOperationSummary summary)
		{
			base.GetSummaryInformation(correlationId, ref summary);
			summary.OperationType = "Admin";
			summary.OperationName = this.methodId.ToString();
		}

		private readonly AdminMethod methodId;

		private AdminExMonLogger adminExmonLogger;
	}
}
