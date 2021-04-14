using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	public class LocalCommitAcknowledger
	{
		public LocalCommitAcknowledger(DxStoreInstance instance)
		{
			this.instance = instance;
			this.OldestItemTime = DateTimeOffset.Now;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.CommitAckTracer;
			}
		}

		public DateTimeOffset OldestItemTime { get; set; }

		public void AddCommand(DxStoreCommand command, WriteOptions options)
		{
			lock (this.locker)
			{
				int count = this.instance.StateMachine.Paxos.ConfigurationHint.Acceptors.Count;
				LocalCommitAcknowledger.Container container = new LocalCommitAcknowledger.Container(command.CommandId, command.TimeInitiated, count, options);
				LocalCommitAcknowledger.Tracer.TraceDebug((long)this.instance.IdentityHash, "{0}: Adding command {1} - Id# {2} (Initiator: {3}, TotalNodes: {4}, MinimumRequired: {5})", new object[]
				{
					this.instance.Identity,
					command.GetTypeId(),
					command.CommandId,
					command.Initiator,
					container.TotalNodesCount,
					container.MinimumNodesRequired
				});
				LinkedListNode<LocalCommitAcknowledger.Container> value = this.containerList.AddFirst(container);
				this.containerMap[command.CommandId] = value;
				this.UpdateOldestItemTime();
			}
		}

		public void HandleAcknowledge(Guid commandId, string sender)
		{
			lock (this.locker)
			{
				LocalCommitAcknowledger.Tracer.TraceDebug<string, Guid, string>((long)this.instance.IdentityHash, "{0}: Received ack for Id# {1} from {2}", this.instance.Identity, commandId, sender);
				bool flag2 = false;
				bool flag3 = false;
				LocalCommitAcknowledger.Container container = null;
				LinkedListNode<LocalCommitAcknowledger.Container> linkedListNode;
				if (this.containerMap.TryGetValue(commandId, out linkedListNode))
				{
					container = linkedListNode.Value;
					if (container != null)
					{
						container.CompletionTimes[sender] = (int)(DateTimeOffset.Now - container.InitiatedTime).TotalMilliseconds;
						if (container.CompletionTimes.Count >= container.MinimumNodesRequired)
						{
							flag2 = true;
						}
						if (container.WaitingForAck.Count == 0)
						{
							flag3 = true;
						}
						if (flag2 && flag3 && !container.IsCompleted)
						{
							container.IsCompleted = true;
							try
							{
								container.CompletionEvent.Set();
							}
							catch (ObjectDisposedException)
							{
								LocalCommitAcknowledger.Tracer.TraceError<string, Guid>((long)this.instance.IdentityHash, "{0}: Ack event for Id# {1} is already disposed", this.instance.Identity, commandId);
							}
						}
					}
				}
				if (container == null)
				{
					LocalCommitAcknowledger.Tracer.TraceWarning<string, Guid>((long)this.instance.IdentityHash, "{0}: Container for Id# {1} does not exist. Possibly timed out and removed", this.instance.Identity, commandId);
				}
				else
				{
					LocalCommitAcknowledger.Tracer.TraceDebug((long)this.instance.IdentityHash, "{0}: Finished processing ack for Id# {1} - Roundtrip {2}ms. (IsMajority: {3}, IsRequiredNodesComplete: {4}, IsComplete: {5})", new object[]
					{
						this.instance.Identity,
						commandId,
						container.CompletionTimes[sender],
						flag2,
						flag3,
						container.IsCompleted
					});
				}
			}
		}

		public WriteResult.ResponseInfo[] RemoveCommand(Guid commandId)
		{
			WriteResult.ResponseInfo[] result = null;
			lock (this.locker)
			{
				LocalCommitAcknowledger.Tracer.TraceDebug<string, Guid>((long)this.instance.IdentityHash, "{0}: Removing command Id# {1} from wait list", this.instance.Identity, commandId);
				LocalCommitAcknowledger.Container container = null;
				LinkedListNode<LocalCommitAcknowledger.Container> linkedListNode;
				if (this.containerMap.TryGetValue(commandId, out linkedListNode))
				{
					container = linkedListNode.Value;
					this.containerMap.Remove(commandId);
					this.containerList.Remove(linkedListNode);
					this.UpdateOldestItemTime();
					if (container != null)
					{
						result = (from kvp in container.CompletionTimes
						select new WriteResult.ResponseInfo
						{
							Name = kvp.Key,
							LatencyInMs = kvp.Value
						}).ToArray<WriteResult.ResponseInfo>();
						if (container.CompletionEvent != null)
						{
							container.CompletionEvent.Dispose();
						}
					}
				}
				if (container == null)
				{
					LocalCommitAcknowledger.Tracer.TraceWarning<string, Guid>((long)this.instance.IdentityHash, "{0}: Remove ignored for command Id# {1} since it might have already been removed", this.instance.Identity, commandId);
				}
			}
			return result;
		}

		public bool WaitForExecution(Guid commandId, TimeSpan timeout)
		{
			ManualResetEvent manualResetEvent = null;
			LocalCommitAcknowledger.Tracer.TraceDebug<string, Guid>((long)this.instance.IdentityHash, "{0}: Waiting for command Id# {1} to satisfy write constraints", this.instance.Identity, commandId);
			lock (this.locker)
			{
				LinkedListNode<LocalCommitAcknowledger.Container> linkedListNode;
				if (this.containerMap.TryGetValue(commandId, out linkedListNode) && linkedListNode != null && linkedListNode.Value != null)
				{
					manualResetEvent = linkedListNode.Value.CompletionEvent;
				}
			}
			if (manualResetEvent == null)
			{
				LocalCommitAcknowledger.Tracer.TraceWarning<string, Guid>((long)this.instance.IdentityHash, "{0}: Completion event does not exist for command Id# {1}", this.instance.Identity, commandId);
				return true;
			}
			WaitHandle[] waitHandles = new WaitHandle[]
			{
				manualResetEvent,
				this.instance.StopEvent
			};
			if (WaitHandle.WaitAny(waitHandles, timeout) == 0)
			{
				LocalCommitAcknowledger.Tracer.TraceDebug<string, Guid>((long)this.instance.IdentityHash, "{0}: Completion event triggered command Id# {1}", this.instance.Identity, commandId);
				return true;
			}
			LocalCommitAcknowledger.Tracer.TraceError<string, Guid>((long)this.instance.IdentityHash, "{0}: Completion event timedout for command Id# {1}", this.instance.Identity, commandId);
			return false;
		}

		private void UpdateOldestItemTime()
		{
			LinkedListNode<LocalCommitAcknowledger.Container> last = this.containerList.Last;
			if (last != null && last.Value != null)
			{
				this.OldestItemTime = last.Value.InitiatedTime;
				return;
			}
			this.OldestItemTime = DateTimeOffset.Now;
		}

		private readonly LinkedList<LocalCommitAcknowledger.Container> containerList = new LinkedList<LocalCommitAcknowledger.Container>();

		private readonly Dictionary<Guid, LinkedListNode<LocalCommitAcknowledger.Container>> containerMap = new Dictionary<Guid, LinkedListNode<LocalCommitAcknowledger.Container>>();

		private readonly object locker = new object();

		private readonly DxStoreInstance instance;

		internal class Container
		{
			internal Container(Guid id, DateTimeOffset initiatedTime, int totalNodesCount, WriteOptions options)
			{
				this.Id = id;
				this.InitiatedTime = initiatedTime;
				this.Options = options;
				this.TotalNodesCount = totalNodesCount;
				this.CompletionEvent = new ManualResetEvent(false);
				this.CompletionTimes = new Dictionary<string, int>();
				this.WaitingForAck = new HashSet<string>();
				if (options != null)
				{
					this.MinimumNodesRequired = (int)Math.Ceiling((double)this.TotalNodesCount * options.PercentageOfNodesToSucceed / 100.0);
					if (options.WaitForNodes != null)
					{
						EnumerableEx.ForEach<string>(options.WaitForNodes, delegate(string node)
						{
							this.WaitingForAck.Add(node);
						});
					}
				}
			}

			public Guid Id { get; set; }

			public DateTimeOffset InitiatedTime { get; set; }

			public WriteOptions Options { get; set; }

			public bool IsCompleted { get; set; }

			public ManualResetEvent CompletionEvent { get; set; }

			public Dictionary<string, int> CompletionTimes { get; set; }

			public int TotalNodesCount { get; set; }

			public int MinimumNodesRequired { get; set; }

			public HashSet<string> WaitingForAck { get; set; }
		}
	}
}
