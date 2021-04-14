using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks.Dataflow;
using Microsoft.Exchange.Compliance.TaskDistributionCommon;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Utility;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing
{
	internal class RoutingCache
	{
		private RoutingCache()
		{
		}

		public static RoutingCache Instance
		{
			get
			{
				return RoutingCache.instance;
			}
		}

		public MemoryCache RoutingTable
		{
			get
			{
				if (this.routingTable == null)
				{
					FaultDefinition faultDefinition;
					Registry.Instance.TryGetInstance<MemoryCache>(RegistryComponent.Common, CommonComponent.CriticalCache, out this.routingTable, out faultDefinition, "RoutingTable", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\RoutingCache.cs", 84);
				}
				return this.routingTable;
			}
		}

		public MemoryCache DispatchQueue
		{
			get
			{
				if (this.dispatchQueue == null)
				{
					FaultDefinition faultDefinition;
					Registry.Instance.TryGetInstance<MemoryCache>(RegistryComponent.Common, CommonComponent.CriticalCache, out this.dispatchQueue, out faultDefinition, "DispatchQueue", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\RoutingCache.cs", 104);
				}
				return this.dispatchQueue;
			}
		}

		public IncomingEntry ReceiveMessage(ComplianceMessage message, out bool shouldProcess)
		{
			shouldProcess = true;
			message.ProtocolContext.Direction = ProtocolContext.MessageDirection.Incoming;
			IncomingEntry incomingEntry = this.GetIncomingEntry(message, false);
			if (incomingEntry != null)
			{
				if (incomingEntry.Status == IncomingEntry.IncomingEntryStatus.Processed)
				{
					shouldProcess = false;
				}
				else if (incomingEntry.Status == IncomingEntry.IncomingEntryStatus.Completed)
				{
					shouldProcess = false;
					incomingEntry.RequestReissued();
				}
			}
			OutgoingEntry outgoingEntry = incomingEntry.ReturnOutgoingEntry(message);
			if (outgoingEntry != null && outgoingEntry.Status != OutgoingEntry.OutgoingEntryStatus.Completed)
			{
				message.ProtocolContext.Direction = ProtocolContext.MessageDirection.Return;
				if (!shouldProcess)
				{
					shouldProcess = true;
				}
			}
			return incomingEntry;
		}

		public OutgoingEntry SendMessage(ComplianceMessage message)
		{
			message.ProtocolContext.Direction = ProtocolContext.MessageDirection.Outgoing;
			IncomingEntry incomingEntry = this.GetIncomingEntry(message, true);
			return incomingEntry.AddOutgoingEntry(message);
		}

		public IncomingEntry ReturnMessage(ComplianceMessage message)
		{
			IncomingEntry incomingEntry = this.GetIncomingEntry(message, true);
			incomingEntry.CompleteOutgoingEntry(message);
			return incomingEntry;
		}

		public IncomingEntry ProcessedMessage(ComplianceMessage message)
		{
			IncomingEntry incomingEntry = this.GetIncomingEntry(message, false);
			if (message.ComplianceMessageType != ComplianceMessageType.RecordResult)
			{
				incomingEntry.Status = IncomingEntry.IncomingEntryStatus.Processed;
			}
			return incomingEntry;
		}

		public void DispatchedMessage(ComplianceMessage message)
		{
			IncomingEntry incomingEntry = message.ProtocolContext.DispatchData as IncomingEntry;
			if (incomingEntry != null)
			{
				incomingEntry.Status = IncomingEntry.IncomingEntryStatus.Completed;
				return;
			}
			OutgoingEntry outgoingEntry = message.ProtocolContext.DispatchData as OutgoingEntry;
			if (outgoingEntry != null)
			{
				outgoingEntry.Status = OutgoingEntry.OutgoingEntryStatus.Delivered;
			}
		}

		public void RecordResult(ComplianceMessage message, Func<ResultBase, ResultBase> commitFunction)
		{
			IncomingEntry incomingEntry = this.GetIncomingEntry(message, true);
			incomingEntry.RecordResult(message, commitFunction);
		}

		public void QueueDispatch(ComplianceMessage message)
		{
			string routingKey = MessageHelper.GetRoutingKey(message);
			CacheItemPolicy policy = new CacheItemPolicy
			{
				RemovedCallback = new CacheEntryRemovedCallback(this.DispatchQueueExpiry),
				SlidingExpiration = TaskDistributionSettings.DispatchQueueTime
			};
			ConcurrentBag<ComplianceMessage> concurrentBag = new ConcurrentBag<ComplianceMessage>();
			if (TaskDistributionSettings.EnableDispatchQueue && this.DispatchQueue != null)
			{
				ConcurrentBag<ComplianceMessage> concurrentBag2 = (this.dispatchQueue.AddOrGetExisting(routingKey, concurrentBag, policy, null) as ConcurrentBag<ComplianceMessage>) ?? concurrentBag;
				concurrentBag2.Add(message);
				return;
			}
			concurrentBag.Add(message);
			DataflowBlock.SendAsync<IEnumerable<ComplianceMessage>>(this.dispatchBlock, concurrentBag);
		}

		private void DispatchQueueExpiry(CacheEntryRemovedArguments e)
		{
			ConcurrentBag<ComplianceMessage> concurrentBag = e.CacheItem.Value as ConcurrentBag<ComplianceMessage>;
			DataflowBlock.SendAsync<IEnumerable<ComplianceMessage>>(this.dispatchBlock, concurrentBag);
		}

		private IncomingEntry GetIncomingEntry(ComplianceMessage message, bool outbound)
		{
			if (!outbound && message.ComplianceMessageType == ComplianceMessageType.RecordResult)
			{
				outbound = true;
			}
			IncomingEntry incomingEntry = new IncomingEntry(message, outbound);
			return incomingEntry.UpdateCache(RoutingCache.Instance.RoutingTable) as IncomingEntry;
		}

		private static RoutingCache instance = new RoutingCache();

		private MemoryCache routingTable;

		private MemoryCache dispatchQueue;

		private ITargetBlock<IEnumerable<ComplianceMessage>> dispatchBlock = new DispatchBlock().GetDataflowBlock(null);
	}
}
