using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.HA;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	internal class MapiExecutionDiagnostics : RpcExecutionDiagnostics, IDiagnosticInfoProvider
	{
		internal MapiExMonLogger MapiExMonLogger
		{
			get
			{
				return this.mapiExmonLogger;
			}
			set
			{
				this.mapiExmonLogger = value;
			}
		}

		public override byte OpNumber
		{
			get
			{
				return (byte)this.ropId;
			}
		}

		protected override bool HasDataToLog
		{
			get
			{
				return base.HasDataToLog || (this.IsInstantSearchFolderView && ConfigurationSchema.DiagnosticsThresholdInstantSearchFolderView.Value <= base.ChunkStatistics.ElapsedTime);
			}
		}

		private bool IsInstantSearchFolderView
		{
			get
			{
				MapiViewMessage mapiViewMessage = this.mapiObject as MapiViewMessage;
				return mapiViewMessage != null && mapiViewMessage.CorrelationId != null;
			}
		}

		protected override bool HasClientActivityDataToLog
		{
			get
			{
				return base.HasClientActivityDataToLog || (base.ClientType != ClientType.MoMT && (this.mapiObject != null && this.mapiObject.Logon != null && !this.mapiObject.Logon.ClientActivity.ActivityReported && this.mapiObject.Logon.ClientActivity.NumberOfRpcCalls >= ConfigurationSchema.DiagnosticsThresholdHeavyActivityRpcCount.Value));
			}
		}

		internal int RpcBytesReceived
		{
			get
			{
				return this.rpcBytesReceived;
			}
		}

		internal int RpcBytesSent
		{
			get
			{
				return this.rpcBytesSent;
			}
		}

		internal int NumberOfRops
		{
			get
			{
				return this.numberOfRops;
			}
		}

		internal MapiBase MapiObject
		{
			set
			{
				this.mapiObject = value;
			}
		}

		public override uint TypeIdentifier
		{
			get
			{
				return 3U;
			}
		}

		public void GetDiagnosticData(long maxSize, out uint threadId, out uint requestId, out DiagnosticContextFlags flags, out byte[] data)
		{
			DiagnosticContext.TraceDwordAndString((LID)10786U, 0U, MapiExecutionDiagnostics.diagnosticInfoHeaderString);
			threadId = (uint)Environment.CurrentManagedThreadId;
			requestId = 0U;
			int maxSize2 = Math.Min((int)maxSize, 512);
			byte b;
			DiagnosticContext.ExtractInfo(maxSize2, out b, out data);
			flags = (DiagnosticContextFlags)b;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ "Rop".GetHashCode() ^ this.ropId.GetHashCode() ^ (this.operationDetail ?? string.Empty).GetHashCode();
		}

		internal new void OnRpcBegin()
		{
			base.OnRpcBegin();
		}

		internal new void OnRpcEnd()
		{
			base.OnRpcEnd();
		}

		internal void OnRopBegin(RopId ropId)
		{
			this.MapiExMonLogger.LogPrepareForRop(ropId);
			base.OnBeginOperation();
			this.ropId = ropId;
			ILogTransactionInformation logTransactionInformationBlock = new LogTransactionInformationMapi(this.ropId);
			base.LogTransactionInformationCollector.AddLogTransactionInformationBlock(logTransactionInformationBlock);
			base.ClearExceptionHistory();
			this.MapiExMonLogger.SetClientActivityInfo(base.ClientActivityId.ToString(), base.ClientComponentName, base.ClientProtocolName, base.ClientActionString);
		}

		internal void OnRopEnd(RopId ropId, ErrorCode errorCode)
		{
			this.numberOfRops++;
			JET_THREADSTATS threadStats;
			Factory.GetDatabaseThreadStats(out threadStats);
			bool isNewActivity = false;
			this.MapiExMonLogger.LogCompletedRop(ropId, errorCode, threadStats);
			if (this.mapiObject != null)
			{
				if (this.mapiObject.MapiObjectType == MapiObjectType.Stream && this.mapiObject.ParentObject != null)
				{
					base.OpDetail = (int)(this.mapiObject.ParentObject.MapiObjectType + 5000U);
				}
				else
				{
					base.OpDetail = (int)(this.mapiObject.MapiObjectType + 1000U);
				}
				if (this.mapiObject.Logon != null)
				{
					isNewActivity = this.mapiObject.Logon.ClientActivity.Update(base.ClientActivityId, base.RpcExecutionCookie, this.ropId);
				}
			}
			base.OnEndOperation(OperationType.Rop, base.ExpandedClientActionStringId, (byte)ropId, (uint)errorCode, isNewActivity);
			if (base.DumpClientActivityDiagnosticIfNeeded())
			{
				this.mapiObject.Logon.ClientActivity.ActivityReported = true;
			}
			this.ropId = RopId.None;
			if (this.mapiObject != null)
			{
				this.mapiObject.ClearDiagnosticInformation();
				this.mapiObject = null;
			}
			ResourceDigestStats activity = new ResourceDigestStats
			{
				ROPCount = 1,
				LdapSearches = base.OperationStatistics.DirectoryCount,
				TimeInServer = base.OperationStatistics.ElapsedTime
			};
			base.ActivityCollector.LogActivity(activity);
		}

		internal override void OnBeforeEndMailboxOperation()
		{
			base.OnBeforeEndMailboxOperation();
			StoreDatabase storeDatabase = Storage.FindDatabase(base.DatabaseGuid);
			if (storeDatabase != null)
			{
				JetHADatabase jetHADatabase = storeDatabase.PhysicalDatabase as JetHADatabase;
				if (jetHADatabase != null)
				{
					base.ReplicationThrottlingData.Update(jetHADatabase.ThrottlingData.DataProtectionHealth, jetHADatabase.ThrottlingData.DataAvailabilityHealth);
				}
			}
		}

		internal void UpdateRpcBytesReceived(int bytes)
		{
			this.rpcBytesReceived += bytes;
		}

		internal void UpdateRpcBytesSent(int bytes)
		{
			this.rpcBytesSent += bytes;
		}

		internal override void EnablePerClientTypePerfCounterUpdate()
		{
			base.EnablePerClientTypePerfCounterUpdate();
			this.rpcBytesReceived = 0;
			this.rpcBytesSent = 0;
			this.numberOfRops = 0;
			this.rpcExecutionStartTimeStamp = StopwatchStamp.GetStamp();
			if (base.PerClientPerfInstance != null)
			{
				base.PerClientPerfInstance.RPCRequests.Increment();
			}
		}

		internal override void DisablePerClientTypePerfCounterUpdate()
		{
			if (base.PerClientPerfInstance != null)
			{
				base.PerClientPerfInstance.RPCRequests.Decrement();
				base.PerClientPerfInstance.RPCPacketsRate.Increment();
				base.PerClientPerfInstance.RPCBytesInRate.IncrementBy((long)this.rpcBytesReceived);
				base.PerClientPerfInstance.RPCBytesOutRate.IncrementBy((long)this.rpcBytesSent);
				base.PerClientPerfInstance.RPCOperationRate.IncrementBy((long)this.numberOfRops);
				base.PerClientPerfInstance.RPCAverageLatencyBase.Increment();
				base.PerClientPerfInstance.RPCAverageLatency.IncrementBy((long)this.rpcExecutionStartTimeStamp.ElapsedTime.TotalMilliseconds);
			}
			base.DisablePerClientTypePerfCounterUpdate();
		}

		protected override void FormatOperationInformation(TraceContentBuilder cb, int indentLevel)
		{
			base.FormatOperationInformation(cb, indentLevel);
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Rop: " + this.ropId);
			if (this.mapiObject != null)
			{
				this.mapiObject.FormatDiagnosticInformation(cb, indentLevel);
			}
		}

		protected override void FormatClientActivityDiagnosticInformation(TraceContentBuilder cb, int indentLevel)
		{
			base.FormatClientActivityDiagnosticInformation(cb, indentLevel);
			if (this.mapiObject == null || this.mapiObject.Logon == null || this.mapiObject.Logon.ClientActivity == null || this.mapiObject.Logon.ClientActivity.RopsCount == null)
			{
				return;
			}
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Executed ROPs:");
			ushort[] ropsCount = this.mapiObject.Logon.ClientActivity.RopsCount;
			for (int i = 0; i < ropsCount.Length; i++)
			{
				if (ropsCount[i] != 0)
				{
					ExecutionDiagnostics.FormatLine(cb, indentLevel + 1, string.Format("{0}: {1}", (RopId)i, ropsCount[i]));
				}
			}
		}

		public void ExtractClientActivityFromAuxiliaryData(AuxiliaryData auxiliaryData)
		{
			if (auxiliaryData == null || auxiliaryData.Input == null)
			{
				return;
			}
			for (int i = 0; i < auxiliaryData.Input.Count; i++)
			{
				AuxiliaryBlock auxiliaryBlock = auxiliaryData.Input[i];
				if (auxiliaryBlock.Type == AuxiliaryBlockTypes.ClientActivity)
				{
					ClientActivityAuxiliaryBlock clientActivityAuxiliaryBlock = (ClientActivityAuxiliaryBlock)auxiliaryBlock;
					base.SetClientActivityInfo(clientActivityAuxiliaryBlock.ActivityId, clientActivityAuxiliaryBlock.ComponentName, clientActivityAuxiliaryBlock.ProtocolName, clientActivityAuxiliaryBlock.ActionString);
					base.UpdateTestCaseId((TestCaseId)((int)clientActivityAuxiliaryBlock.TestCaseId));
					return;
				}
			}
		}

		protected override void GetSummaryInformation(Guid correlationId, ref ExecutionDiagnostics.LongOperationSummary summary)
		{
			base.GetSummaryInformation(correlationId, ref summary);
			summary.OperationType = "Rop";
			summary.OperationName = this.ropId.ToString();
			if (this.mapiObject != null)
			{
				this.mapiObject.GetSummaryInformation(ref summary);
				this.operationDetail = summary.OperationDetail;
			}
		}

		protected override void GetClientActivitySummaryInformation(Guid correlationId, ref ExecutionDiagnostics.HeavyClientActivitySummary summary)
		{
			base.GetClientActivitySummaryInformation(correlationId, ref summary);
			summary.TotalRpcCalls = (uint)this.mapiObject.Logon.ClientActivity.NumberOfRpcCalls;
			uint num = 0U;
			foreach (ushort num2 in this.mapiObject.Logon.ClientActivity.RopsCount)
			{
				num += (uint)num2;
			}
			summary.TotalRops = num;
		}

		protected override void FormatThresholdInformation(TraceContentBuilder cb, int indentLevel)
		{
			if (this.IsInstantSearchFolderView)
			{
				long value = (long)base.ChunkStatistics.ElapsedTime.TotalMilliseconds;
				long threshold = (long)ConfigurationSchema.DiagnosticsThresholdInstantSearchFolderView.Value.TotalMilliseconds;
				ExecutionDiagnostics.FormatThresholdLine(cb, indentLevel, "Instant Search View", value, threshold, "ms");
			}
			base.FormatThresholdInformation(cb, indentLevel);
		}

		protected override void FormatClientActivityThresholdInformation(TraceContentBuilder cb, int indentLevel)
		{
			ExecutionDiagnostics.FormatLine(cb, indentLevel, "Total RPC calls: >=" + this.mapiObject.Logon.ClientActivity.NumberOfRpcCalls);
			base.FormatClientActivityThresholdInformation(cb, indentLevel);
		}

		private const int MaximalSupportedDiagnosticInfoSize = 512;

		private static string diagnosticInfoHeaderString = string.Format("{0}:{1}", "15.00.1497.015", MapiDispHelper.GetDnsHostName());

		private RopId ropId;

		private MapiBase mapiObject;

		private MapiExMonLogger mapiExmonLogger;

		private int rpcBytesReceived;

		private int rpcBytesSent;

		private int numberOfRops;

		private StopwatchStamp rpcExecutionStartTimeStamp;

		private string operationDetail;
	}
}
