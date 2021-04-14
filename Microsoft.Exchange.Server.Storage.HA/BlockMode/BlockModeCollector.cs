using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.HA;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.BlockMode
{
	internal class BlockModeCollector
	{
		public BlockModeCollector(Guid dbGuid, string dbName)
		{
			this.DatabaseGuid = dbGuid;
			this.DatabaseName = dbName;
			this.PreallocateIoResources();
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.BlockModeCollectorTracer;
			}
		}

		public static bool IsDebugTraceEnabled
		{
			get
			{
				return ExTraceGlobals.BlockModeCollectorTracer.IsTraceEnabled(TraceType.DebugTrace);
			}
		}

		public Guid DatabaseGuid { get; private set; }

		public string DatabaseName { get; private set; }

		public ThrottlingData ThrottlingData
		{
			get
			{
				ThrottlingUpdater throttlingUpdater = this.throttling;
				if (throttlingUpdater != null)
				{
					return throttlingUpdater.ThrottlingData;
				}
				return null;
			}
		}

		private ISimpleBufferPool SocketStreamBufferPool { get; set; }

		private IPool<SocketStreamAsyncArgs> SocketStreamAsyncArgPool { get; set; }

		private BlockModeMessageStream MsgStream { get; set; }

		public void PrepareToMountAsActive(JET_INSTANCE jetInstance)
		{
			BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "PrepareToMountAsActive({0})", this.DatabaseName);
			using (LockManager.Lock(this.senderLock, LockManager.LockType.BlockModeSender))
			{
				this.HookupCallback(jetInstance);
				this.PrepareForActivation();
			}
			this.StartThrottling();
		}

		public void PrepareToMountAsPassive(JET_INSTANCE jetInstance)
		{
			BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "PrepareToMountAsPassive({0})", this.DatabaseName);
			using (LockManager.Lock(this.senderLock, LockManager.LockType.BlockModeSender))
			{
				this.role = BlockModeCollector.Role.Passive;
				this.HookupCallback(jetInstance);
			}
		}

		public void PrepareToTransitionToActive()
		{
			BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "PrepareToTransitionToActive({0})", this.DatabaseName);
			using (LockManager.Lock(this.senderLock, LockManager.LockType.BlockModeSender))
			{
				this.PrepareForActivation();
			}
			this.StartThrottling();
		}

		public void DismountComplete()
		{
			BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "DismountComplete({0})", this.DatabaseName);
			this.DisposeCallback();
			this.StopThrottling();
			int num = 5;
			int num2 = 0;
			BlockModeCollector.Role role;
			using (LockManager.Lock(this.senderLock, LockManager.LockType.BlockModeSender))
			{
				role = this.role;
				this.role = BlockModeCollector.Role.Unknown;
				if (role == BlockModeCollector.Role.Active)
				{
					this.sendersCompleteEvent.Signal();
					num2 = this.passiveSenders.Count;
					if (num2 > 0)
					{
						this.StartWritesInternal();
					}
				}
			}
			if (role == BlockModeCollector.Role.Active)
			{
				if (num2 > 0 && !this.sendersCompleteEvent.Wait(num * 1000))
				{
					BlockModeCollector.Tracer.TraceError((long)this.GetHashCode(), "Dismount proceeds without confirming all data was sent");
				}
				using (LockManager.Lock(this.senderLock, LockManager.LockType.BlockModeSender))
				{
					BlockModeSender[] array = new BlockModeSender[this.passiveSenders.Count];
					this.passiveSenders.Values.CopyTo(array, 0);
					this.passiveSenders.Clear();
					foreach (BlockModeSender blockModeSender in array)
					{
						blockModeSender.Close();
						if (ActiveDatabaseSenderPerformanceCounters.InstanceExists(blockModeSender.CopyName))
						{
							ActiveDatabaseSenderPerformanceCounters.RemoveInstance(blockModeSender.CopyName);
						}
					}
					this.sendersCompleteEvent.Dispose();
					this.sendersCompleteEvent = null;
					this.MsgStream = null;
				}
				if (ActiveDatabasePerformanceCounters.InstanceExists(this.DatabaseName))
				{
					ActiveDatabasePerformanceCounters.RemoveInstance(this.DatabaseName);
				}
				this.perfInstance = null;
			}
			IDisposable disposable = this.SocketStreamAsyncArgPool as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public void HandleSenderFailed(BlockModeSender sender, Exception ex)
		{
			bool flag = false;
			if (!LockManager.TestLock(this.senderLock, LockManager.LockType.BlockModeSender))
			{
				LockManager.GetLock(this.senderLock, LockManager.LockType.BlockModeSender);
			}
			else
			{
				flag = true;
			}
			try
			{
				BlockModeSender blockModeSender = null;
				if (this.passiveSenders.TryGetValue(sender.PassiveName, out blockModeSender))
				{
					if (blockModeSender == sender)
					{
						this.RemoveSender(sender);
						this.UpdateOldestSenderPosition();
					}
					else
					{
						BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "failing sender to passive {0} is old and not removed", sender.PassiveName);
					}
				}
			}
			finally
			{
				sender.Close();
				if (!flag)
				{
					LockManager.ReleaseLock(this.senderLock, LockManager.LockType.BlockModeSender);
				}
			}
		}

		public void StartReplicationToPassive(string passiveName, uint firstGenToSend)
		{
			bool flag = false;
			bool flag2 = false;
			BlockModeSender blockModeSender = null;
			BlockModeSender blockModeSender2 = null;
			NetworkChannel networkChannel = null;
			string text = string.Format("{0}\\{1}", this.DatabaseName, passiveName);
			BlockModeCollector.Tracer.TraceDebug<string, uint>((long)this.GetHashCode(), "StartReplicationToPassive ({0}) firstGen 0x{1:X}", text, firstGenToSend);
			try
			{
				using (LockManager.Lock(this.senderLock, LockManager.LockType.BlockModeSender))
				{
					if (this.passiveSenders.TryGetValue(passiveName, out blockModeSender))
					{
						BlockModeCollector.Tracer.TraceError<string>((long)this.GetHashCode(), "EnterBlockMode({0}) replacing old sender", text);
						this.RemoveSender(blockModeSender);
					}
					if (this.role != BlockModeCollector.Role.Active)
					{
						string message = string.Format("The local copy is not yet active. This is a race that will resolve itself. Passive requester is {0}", passiveName);
						BlockModeCollector.Tracer.TraceError((long)this.GetHashCode(), message);
						throw new GranularReplicationInitFailedException(message);
					}
					BlockModeMessageStream.SenderPosition senderPosition = this.MsgStream.Join(firstGenToSend);
					if (senderPosition == null)
					{
						string message2 = string.Format("Sender failed to join: passiveName({0}) genToSend(0x{1:X})", passiveName, firstGenToSend);
						BlockModeCollector.Tracer.TraceError((long)this.GetHashCode(), message2);
						throw new GranularReplicationInitFailedException(message2);
					}
					blockModeSender2 = new BlockModeSender(passiveName, this, senderPosition);
					this.passiveSenders.Add(passiveName, blockModeSender2);
					this.BuildPassiveSendersList();
					this.sendersCompleteEvent.AddCount();
				}
				BlockModeCollector.SocketStreamPerfCtrs perfCtrs = new BlockModeCollector.SocketStreamPerfCtrs(text);
				networkChannel = NetworkChannel.OpenChannel(passiveName, this.SocketStreamBufferPool, this.SocketStreamAsyncArgPool, perfCtrs, true);
				EnterBlockModeMsg enterBlockModeMsg = new EnterBlockModeMsg(networkChannel, EnterBlockModeMsg.Flags.None, this.DatabaseGuid, (long)((ulong)firstGenToSend));
				enterBlockModeMsg.Send();
				NetworkChannelMessage message3 = networkChannel.GetMessage();
				if (!(message3 is EnterBlockModeMsg))
				{
					string text2 = string.Format("Passive({0}) failed to ack", passiveName);
					BlockModeCollector.Tracer.TraceError((long)this.GetHashCode(), text2);
					throw new NetworkUnexpectedMessageException(this.DatabaseName, text2);
				}
				using (LockManager.Lock(this.senderLock, LockManager.LockType.BlockModeSender))
				{
					blockModeSender2.PassiveIsReady(networkChannel);
					flag2 = true;
					this.CheckCompression();
					blockModeSender2.StartRead();
					flag = true;
					this.StartWritesInternal();
				}
			}
			finally
			{
				if (!flag)
				{
					BlockModeCollector.Tracer.TraceError<string>((long)this.GetHashCode(), "Failed to setup repl with passive {0}", passiveName);
					if (blockModeSender2 != null)
					{
						this.HandleSenderFailed(blockModeSender2, null);
						if (!flag2 && networkChannel != null)
						{
							networkChannel.Close();
						}
					}
				}
				if (blockModeSender != null)
				{
					blockModeSender.Close();
				}
			}
		}

		public void MarkWritesShouldBeAttempted()
		{
			this.writesShouldBeAttemptedFlag = true;
		}

		public void TryStartWrites()
		{
			if (LockManager.TryGetLock(this.senderLock, LockManager.LockType.BlockModeSender))
			{
				try
				{
					this.StartWritesInternal();
					return;
				}
				finally
				{
					LockManager.ReleaseLock(this.senderLock, LockManager.LockType.BlockModeSender);
				}
			}
			BlockModeCollector.Tracer.TraceDebug((long)this.GetHashCode(), "TryStartWrites: lock conflict, exitting");
			this.MarkWritesShouldBeAttempted();
		}

		public void StartWrites()
		{
			bool flag = false;
			if (!LockManager.TestLock(this.senderLock, LockManager.LockType.BlockModeSender))
			{
				LockManager.GetLock(this.senderLock, LockManager.LockType.BlockModeSender);
			}
			else
			{
				flag = true;
			}
			try
			{
				this.StartWritesInternal();
			}
			finally
			{
				if (!flag)
				{
					LockManager.ReleaseLock(this.senderLock, LockManager.LockType.BlockModeSender);
				}
			}
		}

		internal void TriggerThrottlingUpdate()
		{
			if (this.lastLogGenerated > 0UL)
			{
				LogCopyStatus activeCopyLogGenerationInfo = new LogCopyStatus(CopyType.Active, LocalHost.Fqdn, false, this.lastLogGenerated, 0UL, 0UL);
				List<BlockModeSender> list = this.passiveSendersForThrottling;
				List<LogCopyStatus> list2 = new List<LogCopyStatus>(list.Count);
				foreach (BlockModeSender blockModeSender in list)
				{
					list2.Add(blockModeSender.LogCopyStatus);
				}
				ThrottlingUpdater throttlingUpdater = this.throttling;
				if (throttlingUpdater != null)
				{
					throttlingUpdater.TriggerUpdate(activeCopyLogGenerationInfo, list2, this.lastLogGeneratedTimeUtc);
				}
			}
		}

		private void PrepareForActivation()
		{
			this.AllocateMessageStream();
			this.sendersCompleteEvent = new CountdownEvent(1);
			this.perfInstance = ActiveDatabasePerformanceCounters.GetInstance(this.DatabaseName);
			this.role = BlockModeCollector.Role.Active;
		}

		private void RemoveSender(BlockModeSender sender)
		{
			this.passiveSenders.Remove(sender.PassiveName);
			this.sendersCompleteEvent.Signal();
			this.BuildPassiveSendersList();
			this.CheckCompression();
		}

		private void HookupCallback(JET_INSTANCE jetInstance)
		{
			IntPtr emitContext = new IntPtr(42);
			this.emitCallback = new EmitLogDataCallback(jetInstance, new JET_PFNEMITLOGDATA(this.EmitCallback), emitContext);
		}

		private void DisposeCallback()
		{
			if (this.emitCallback != null)
			{
				this.emitCallback.Dispose();
				this.emitCallback = null;
			}
		}

		private void StartThrottling()
		{
			BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "StartThrottling({0})", this.DatabaseName);
			this.throttling = new ThrottlingUpdater(this.DatabaseGuid);
			this.throttling.Start();
		}

		private void StopThrottling()
		{
			BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "StopThrottling({0})", this.DatabaseName);
			if (this.throttling != null)
			{
				this.throttling.Stop();
				this.throttling = null;
			}
		}

		private void BuildPassiveSendersList()
		{
			List<BlockModeSender> list = new List<BlockModeSender>(this.passiveSenders.Values.Count);
			foreach (BlockModeSender item in this.passiveSenders.Values)
			{
				list.Add(item);
			}
			this.passiveSendersForThrottling = list;
		}

		private JET_err EmitCallback(JET_INSTANCE instance, JET_EMITDATACTX emitContext, byte[] logdata, int logdataLength, object callbackctx)
		{
			BlockModeCollector.<>c__DisplayClass1 CS$<>8__locals1 = new BlockModeCollector.<>c__DisplayClass1();
			CS$<>8__locals1.emitContext = emitContext;
			CS$<>8__locals1.logdata = logdata;
			CS$<>8__locals1.logdataLength = logdataLength;
			CS$<>8__locals1.<>4__this = this;
			WatsonOnUnhandledException.Guard(NullExecutionDiagnostics.Instance, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<EmitCallback>b__0)));
			return JET_err.Success;
		}

		private void AllocateMessageStream()
		{
			this.MsgStream = new BlockModeMessageStream(this.DatabaseName, 2, this.bigWriteBuffers);
		}

		private void PreallocateIoResources()
		{
			IRegistryReader instance = RegistryReader.Instance;
			int value = instance.GetValue<int>(Registry.LocalMachine, "Software\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters", "MaxBlockModeSendDepthInMB", 0);
			long num;
			if (value != 0)
			{
				num = (long)(value * 1048576);
			}
			else
			{
				using (Context context = Context.CreateForSystem())
				{
					ServerInfo serverInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(context);
					num = serverInfo.ContinuousReplicationMaxMemoryPerDatabase;
				}
			}
			num = Math.Min(104857600L, num);
			num = Math.Max(3145728L, num);
			this.maxBuffersToKeepForSenders = (int)(num / 1048576L);
			BlockModeCollector.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Max Sender Depth is {0} buffers", this.maxBuffersToKeepForSenders);
			int numberOfBuffersToPreAllocate = 6;
			this.bigWriteBuffers = new BlockModeMessageStream.FreeIOBuffers(1048576, numberOfBuffersToPreAllocate);
			int num2 = 4;
			int num3 = 1;
			int num4 = 2;
			int num5 = 1;
			int preAllocCount = num2 * num3 * num5 * num4;
			int bufSize = 65536;
			this.SocketStreamBufferPool = new SimpleBufferPool(bufSize, preAllocCount);
			this.SocketStreamAsyncArgPool = new SocketStreamAsyncArgsPool(preAllocCount);
		}

		private void StartWritesInternal()
		{
			do
			{
				this.writesShouldBeAttemptedFlag = false;
				BlockModeSender[] array = new BlockModeSender[this.passiveSenders.Count];
				this.passiveSenders.Values.CopyTo(array, 0);
				BlockModeSender[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					BlockModeSender sender = array2[i];
					if (this.CheckSenderForOverflow(sender))
					{
						Exception ex = NetworkChannel.RunNetworkFunction(delegate
						{
							sender.TryStartWrite();
						});
						if (ex != null)
						{
							this.HandleSenderFailed(sender, ex);
						}
					}
				}
				this.UpdateOldestSenderPosition();
			}
			while (this.writesShouldBeAttemptedFlag);
		}

		private bool CheckSenderForOverflow(BlockModeSender sender)
		{
			ulong num = this.MsgStream.LatestBufferLifetimeOrdinal - sender.Position.CurrentBuffer.LifetimeOrdinal;
			if (num <= (ulong)((long)this.maxBuffersToKeepForSenders))
			{
				return true;
			}
			BlockModeCollector.Tracer.TraceError<string, ulong>((long)this.GetHashCode(), "Sender({0}) overflowed with depth {1}", sender.CopyName, num);
			ReplayCrimsonEvents.BlockModeOverflowOnActive.Log<string, string>(this.DatabaseName, sender.PassiveName);
			this.perfInstance.BlockModeOverflows.Increment();
			this.RemoveSender(sender);
			sender.Close();
			return false;
		}

		private void UpdateOldestSenderPosition()
		{
			ulong num = ulong.MaxValue;
			foreach (BlockModeSender blockModeSender in this.passiveSenders.Values)
			{
				ulong lifetimeOrdinal = blockModeSender.Position.CurrentBuffer.LifetimeOrdinal;
				if (lifetimeOrdinal < num)
				{
					num = lifetimeOrdinal;
				}
			}
			this.MsgStream.OldestBufferLifetimeReferencedBySenders = num;
		}

		private void CheckCompression()
		{
			bool flag = false;
			DagNetConfig dagNetConfig = DagNetEnvironment.FetchLastKnownNetConfig();
			if (dagNetConfig != null)
			{
				switch (dagNetConfig.NetworkCompression)
				{
				case NetworkOption.Enabled:
				case NetworkOption.InterSubnetOnly:
					using (Dictionary<string, BlockModeSender>.ValueCollection.Enumerator enumerator = this.passiveSenders.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							BlockModeSender blockModeSender = enumerator.Current;
							if (blockModeSender.CompressionDesired)
							{
								BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Passive {0} wants compression.", blockModeSender.CopyName);
								flag = true;
								break;
							}
						}
						goto IL_B2;
					}
					break;
				}
				BlockModeCollector.Tracer.TraceDebug((long)this.GetHashCode(), "DagNet does not enable compression.");
			}
			else
			{
				BlockModeCollector.Tracer.TraceError((long)this.GetHashCode(), "Failed to get dagnet config. Compression disabled.");
			}
			IL_B2:
			if (flag)
			{
				if (!this.MsgStream.CompressLogData)
				{
					BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Compression is being enabled for {0}.", this.DatabaseName);
					this.MsgStream.CompressLogData = true;
					this.perfInstance.CompressionEnabled.RawValue = 1L;
					return;
				}
			}
			else if (this.MsgStream.CompressLogData)
			{
				BlockModeCollector.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Compression is being disabled for {0}.", this.DatabaseName);
				this.MsgStream.CompressLogData = false;
				this.perfInstance.CompressionEnabled.RawValue = 0L;
			}
		}

		private readonly object senderLock = new object();

		private readonly object appendLock = new object();

		private EmitLogDataCallback emitCallback;

		private Dictionary<string, BlockModeSender> passiveSenders = new Dictionary<string, BlockModeSender>(3);

		private BlockModeCollector.Role role;

		private volatile bool writesShouldBeAttemptedFlag;

		private CountdownEvent sendersCompleteEvent;

		private int maxBuffersToKeepForSenders;

		private ActiveDatabasePerformanceCountersInstance perfInstance;

		private ThrottlingUpdater throttling;

		private ulong lastLogGenerated;

		private DateTime? lastLogGeneratedTimeUtc = null;

		private List<BlockModeSender> passiveSendersForThrottling = new List<BlockModeSender>(3);

		private BlockModeMessageStream.FreeIOBuffers bigWriteBuffers;

		private enum Role
		{
			Unknown,
			Passive,
			Active
		}

		internal class SocketStreamPerfCtrs : SocketStream.ISocketStreamPerfCounters
		{
			public SocketStreamPerfCtrs(string passiveName)
			{
				this.perfInstance = ActiveDatabaseSenderPerformanceCounters.GetInstance(passiveName);
			}

			public void RecordWriteLatency(long tics)
			{
				long incrementValue = StopwatchStamp.TicksToMicroSeconds(tics);
				this.perfInstance.AverageSocketWriteLatency.IncrementBy(incrementValue);
				this.perfInstance.AverageSocketWriteLatencyBase.Increment();
			}

			private ActiveDatabaseSenderPerformanceCountersInstance perfInstance;
		}
	}
}
