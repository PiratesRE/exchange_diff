using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Threading;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing
{
	internal class IncomingEntry : Entry
	{
		public IncomingEntry(ComplianceMessage message, bool outbound)
		{
			base.MessageId = (outbound ? message.MessageSourceId : message.MessageId);
			base.Message = (outbound ? null : message);
			base.CorrelationId = message.CorrelationId;
			this.Status = IncomingEntry.IncomingEntryStatus.Initialized;
			base.ExpiryTime = TaskDistributionSettings.IncomingEntryExpiryTime;
		}

		public ResultBase Aggregation
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		public IncomingEntry.IncomingEntryStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				if (this.status < value)
				{
					base.RetryCount = 0;
					this.status = value;
					this.EvaluateState(false);
				}
			}
		}

		public override string GetKey()
		{
			return string.Format("INCOMING:{0}:{1}", base.CorrelationId, base.MessageId);
		}

		public void RecordResult(ComplianceMessage message, Func<ResultBase, ResultBase> commitFunction)
		{
			ResultBase resultBase = this.result;
			while (Interlocked.CompareExchange<ResultBase>(ref this.result, commitFunction(resultBase), resultBase) != resultBase)
			{
				resultBase = this.result;
			}
		}

		public OutgoingEntry ReturnOutgoingEntry(ComplianceMessage message)
		{
			if (message.ComplianceMessageType == ComplianceMessageType.RecordResult)
			{
				OutgoingEntry outgoingEntry = this.AddOutgoingEntry(message);
				if (outgoingEntry != null)
				{
					outgoingEntry.Status = OutgoingEntry.OutgoingEntryStatus.Returned;
					return outgoingEntry;
				}
			}
			return null;
		}

		public OutgoingEntry AddOutgoingEntry(ComplianceMessage message)
		{
			OutgoingEntry outgoingEntry = this.GetOutgoingEntry(message, false);
			if (outgoingEntry != null && outgoingEntry.Status != OutgoingEntry.OutgoingEntryStatus.Completed)
			{
				int value = 0;
				if (this.outgoingKeys.TryAdd(outgoingEntry.GetKey(), value))
				{
					outgoingEntry.EvaluateState(false);
				}
			}
			return outgoingEntry;
		}

		public void CompleteOutgoingEntry(ComplianceMessage message)
		{
			OutgoingEntry outgoingEntry = this.GetOutgoingEntry(message, true);
			if (outgoingEntry != null)
			{
				int num;
				this.outgoingKeys.TryRemove(outgoingEntry.GetKey(), out num);
				outgoingEntry.Status = OutgoingEntry.OutgoingEntryStatus.Completed;
			}
			if (this.outgoingKeys.Count == 0 && this.Status == IncomingEntry.IncomingEntryStatus.Processed)
			{
				this.Status = IncomingEntry.IncomingEntryStatus.Returned;
			}
		}

		public OutgoingEntry GetOutgoingEntry(ComplianceMessage message, bool onlyIfExists = false)
		{
			OutgoingEntry outgoingEntry = new OutgoingEntry(message);
			if (!onlyIfExists)
			{
				return outgoingEntry.UpdateCache(RoutingCache.Instance.RoutingTable) as OutgoingEntry;
			}
			CacheItem cacheItem = RoutingCache.Instance.RoutingTable.GetCacheItem(outgoingEntry.GetKey(), null);
			if (cacheItem != null)
			{
				return cacheItem.Value as OutgoingEntry;
			}
			return null;
		}

		public void RequestReissued()
		{
			this.ReturnMessage();
		}

		public override void EvaluateState(bool expired)
		{
			if (this.Status != IncomingEntry.IncomingEntryStatus.Failed && ExceptionHandler.IsFaulted(base.Message))
			{
				this.Status = IncomingEntry.IncomingEntryStatus.Failed;
				return;
			}
			switch (this.Status)
			{
			case IncomingEntry.IncomingEntryStatus.Initialized:
				if (base.RetryCount > TaskDistributionSettings.IncomingEntryRetriesToFailure)
				{
					ExceptionHandler.FaultMessage(base.Message, FaultDefinition.FromErrorString("Processing Failed", "EvaluateState", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\IncomingEntry.cs", 278), true);
					this.Status = IncomingEntry.IncomingEntryStatus.Failed;
				}
				break;
			case IncomingEntry.IncomingEntryStatus.Processed:
				if (base.RetryCount > TaskDistributionSettings.IncomingEntryRetriesToFailure)
				{
					ExceptionHandler.FaultMessage(base.Message, FaultDefinition.FromErrorString("Ougoing messages not retunred", "EvaluateState", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\IncomingEntry.cs", 287), true);
					this.Status = IncomingEntry.IncomingEntryStatus.Failed;
				}
				if (this.outgoingKeys.Count == 0)
				{
					this.Status = IncomingEntry.IncomingEntryStatus.Returned;
				}
				break;
			case IncomingEntry.IncomingEntryStatus.Returned:
				if (base.RetryCount > TaskDistributionSettings.IncomingEntryRetriesToFailure)
				{
					ExceptionHandler.FaultMessage(base.Message, FaultDefinition.FromErrorString("Recording of results Failed", "EvaluateState", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\IncomingEntry.cs", 303), true);
					this.Status = IncomingEntry.IncomingEntryStatus.Failed;
				}
				this.ReturnMessage();
				break;
			case IncomingEntry.IncomingEntryStatus.Failed:
				if (!ExceptionHandler.IsFaulted(base.Message))
				{
					ExceptionHandler.FaultMessage(base.Message, FaultDefinition.FromErrorString("Unknown failure", "EvaluateState", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\IncomingEntry.cs", 313), true);
				}
				this.FailMessage();
				break;
			}
			if (base.RetryCount > TaskDistributionSettings.IncomingEntryRetriesToAbandon)
			{
				base.KeepAlive = false;
				return;
			}
			base.KeepAlive = true;
		}

		protected override void UpdateExistingEntry(Entry existing)
		{
			IncomingEntry incomingEntry = (IncomingEntry)existing;
			if (incomingEntry.Message == null && base.Message != null)
			{
				incomingEntry.Message = base.Message;
				incomingEntry.EvaluateState(false);
			}
		}

		private void ReturnMessage()
		{
			if (base.Message != null)
			{
				if (ExceptionHandler.IsFaulted(base.Message))
				{
					this.FailMessage();
					return;
				}
				if (this.result != null)
				{
					ComplianceMessage complianceMessage = base.Message.Clone();
					Target messageTarget = complianceMessage.MessageTarget;
					complianceMessage.MessageTarget = complianceMessage.MessageSource;
					complianceMessage.MessageSource = messageTarget;
					complianceMessage.ComplianceMessageType = ComplianceMessageType.RecordResult;
					complianceMessage.ProtocolContext.DispatchData = this;
					complianceMessage.Payload = ComplianceSerializer.Serialize<WorkPayload>(WorkPayload.Description, new WorkPayload
					{
						WorkDefinition = this.result.GetSerializedResult(),
						WorkDefinitionType = complianceMessage.WorkDefinitionType
					});
					RoutingCache.Instance.QueueDispatch(complianceMessage);
				}
			}
		}

		private void FailMessage()
		{
			if (ExceptionHandler.IsFaulted(base.Message))
			{
				ComplianceMessage complianceMessage = base.Message.Clone();
				Target messageTarget = complianceMessage.MessageTarget;
				complianceMessage.MessageTarget = complianceMessage.MessageSource;
				complianceMessage.MessageSource = messageTarget;
				complianceMessage.ComplianceMessageType = ComplianceMessageType.RecordResult;
				complianceMessage.ProtocolContext.DispatchData = this;
				complianceMessage.Payload = ComplianceSerializer.Serialize<WorkPayload>(WorkPayload.Description, ExceptionHandler.GetFaultDefinition(base.Message).ToPayload());
				RoutingCache.Instance.QueueDispatch(complianceMessage);
			}
		}

		private IncomingEntry.IncomingEntryStatus status;

		private ResultBase result;

		private ConcurrentDictionary<string, int> outgoingKeys = new ConcurrentDictionary<string, int>();

		public enum IncomingEntryStatus
		{
			Initialized,
			Processed,
			Returned,
			Failed,
			Completed
		}
	}
}
