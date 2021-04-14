using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using FUSE.Paxos;
using FUSE.Paxos.Network;
using FUSE.Weld.Base;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DxStore;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.Server
{
	public sealed class DxStoreStateMachine : StateMachine<ServiceEndpoint, DxStoreCommand>
	{
		public DxStoreStateMachine(Policy policy, DxStoreInstance instance, INodeEndPoints<ServiceEndpoint> nodeEndPoints, IStorage<string, DxStoreCommand> storage, GroupMembersMesh mesh, Counters perfCounter, Round<string>? roundInitial) : base(nodeEndPoints, mesh, storage, policy, null, perfCounter, roundInitial)
		{
			this.instance = instance;
			this.self = nodeEndPoints.Self;
			this.localDataStore = instance.LocalDataStore;
			this.Mesh = mesh;
			this.truncator = new PeriodicPaxosTrancator(instance);
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.StateMachineTracer;
			}
		}

		public GroupMembersMesh Mesh { get; set; }

		public PaxosBasicInfo GetPaxosInfo()
		{
			PaxosBasicInfo paxosBasicInfo = new PaxosBasicInfo();
			paxosBasicInfo.CountExecuted = base.CountExecuted;
			if (base.Paxos == null)
			{
				return paxosBasicInfo;
			}
			Paxos<string, DxStoreCommand> paxos = base.Paxos;
			paxosBasicInfo.Self = paxos.Self;
			paxosBasicInfo.LeaderHint = paxos.LeaderRoundHint;
			paxosBasicInfo.CountTruncated = paxos.Storage.CountTruncated;
			if (paxos.ConfigurationHint != null && paxos.ConfigurationHint.Acceptors != null)
			{
				paxosBasicInfo.Members = paxos.ConfigurationHint.Acceptors.ToArray<string>();
			}
			if (paxos.GossipByNode != null)
			{
				paxosBasicInfo.Gossip = new PaxosBasicInfo.GossipDictionary();
				foreach (KeyValuePair<string, int> keyValuePair in paxos.CountDecidedByNode)
				{
					paxosBasicInfo.Gossip[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			return paxosBasicInfo;
		}

		public override Task StartAsync(int countExecuted)
		{
			DxStoreStateMachine.Tracer.TraceDebug<string, int>((long)this.instance.IdentityHash, "{0}: Starting state machine (CountExecuted: {1})", this.instance.Identity, countExecuted);
			return base.StartAsync(countExecuted);
		}

		public override Task ExecuteAsync(int instanceId, Proposal<string, DxStoreCommand> proposal)
		{
			return Concurrency.RunSynchronously(delegate()
			{
				lock (this.locker)
				{
					this.ExecuteLocally(instanceId, proposal);
				}
			});
		}

		public void Reconfigure(InstanceGroupMemberConfig[] members)
		{
			string[] array = (from m in base.Paxos.ConfigurationHint.Acceptors
			orderby m
			select m).ToArray<string>();
			string[] array2 = (from m in members
			select m.Name into m
			orderby m
			select m).ToArray<string>();
			if (array2.SequenceEqual(array))
			{
				this.instance.EventLogger.Log(DxEventSeverity.Info, 0, "{0}: Reconfigure skipped since there is not difference in the membership (members={1})", new object[]
				{
					this.instance.GroupConfig.Identity,
					string.Join(",", array)
				});
				return;
			}
			this.instance.EventLogger.Log(DxEventSeverity.Info, 0, "{0}: Attempting reconfigure membership (Existing: {1} New: {2})", new object[]
			{
				this.instance.GroupConfig.Identity,
				string.Join(",", array),
				string.Join(",", array2)
			});
			Configuration<string> configuration = new Configuration<string>(array2, array2, array2, null);
			Utils.RunOperation(this.instance.GroupConfig.Identity, "Statemachine submitting reconfigure", delegate
			{
				Utils.RunWithTimeoutToken(this.instance.GroupConfig.Settings.MemberReconfigureTimeout, (CancellationToken token) => this.ReplicateAsync(configuration, token));
			}, this.instance.EventLogger, LogOptions.LogException | this.instance.GroupConfig.Settings.AdditionalLogOptions, false, null, null, null, null, null);
		}

		public void ReplicateCommand(DxStoreCommand command, TimeSpan? timeout = null)
		{
			this.ReplicateCommand(command, false, timeout);
		}

		public void ReplicateCommand(DxStoreCommand command, bool isForceExecuteLocally, TimeSpan? timeout = null)
		{
			Proposal<string> proposal = StateMachine<string, DxStoreCommand>.MakeProposal(command);
			timeout = new TimeSpan?(timeout ?? this.instance.GroupConfig.Settings.PaxosCommandExecutionTimeout);
			Utils.RunOperation(this.instance.GroupConfig.Identity, "Send Replicate Request :" + command.GetTypeId(), delegate
			{
				if (DxStoreStateMachine.Tracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					DxStoreStateMachine.Tracer.TraceDebug<string, WellKnownCommandName, string>((long)this.instance.IdentityHash, "{0}: Replicate requested {1}: {2}", this.instance.Identity, command.GetTypeId(), command.GetDebugString());
				}
				Utils.RunWithTimeoutToken(timeout.Value, (CancellationToken token) => this.ReplicateAsyncEx(proposal, isForceExecuteLocally, token));
			}, this.instance.EventLogger, LogOptions.LogException | this.instance.GroupConfig.Settings.AdditionalLogOptions, false, null, null, null, delegate(Exception exception)
			{
				if (exception != null)
				{
					if (DxStoreStateMachine.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						DxStoreStateMachine.Tracer.TraceError<string, WellKnownCommandName, Exception>((long)this.instance.IdentityHash, "{0}: Replicate failed for {1} with {2}", this.instance.Identity, command.GetTypeId(), exception);
						return;
					}
				}
				else if (DxStoreStateMachine.Tracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					DxStoreStateMachine.Tracer.TraceDebug<string, WellKnownCommandName>((long)this.instance.IdentityHash, "{0}: Replicate succeeded for {1}", this.instance.Identity, command.GetTypeId());
				}
			}, null);
		}

		public Task ReplicateAsyncEx(Proposal<string> proposal, bool isForceExecuteLocally, CancellationToken cancelToken)
		{
			bool flag = DxStoreStateMachine.Tracer.IsTraceEnabled(TraceType.DebugTrace);
			Task<Tuple<int, Proposal<string>>> result = base.WaitAsync(proposal, cancelToken);
			string leaderHint = this._paxos.LeaderHint;
			if (isForceExecuteLocally || string.IsNullOrEmpty(leaderHint))
			{
				if (flag)
				{
					DxStoreStateMachine.Tracer.TraceDebug<string, bool>((long)this.instance.IdentityHash, "{0}: Proposal submitted from the local paxos. Local paxos may be promoted to a leader (IsForce: {1})", this.instance.Identity, isForceExecuteLocally);
				}
				leaderHint = this._paxos.Self;
			}
			else if (flag)
			{
				DxStoreStateMachine.Tracer.TraceDebug<string, string>((long)this.instance.IdentityHash, "{0}: Proposal submitted through {1}.", this.instance.Identity, leaderHint);
			}
			Message item = new Message.Initiate<string, DxStoreCommand>(proposal);
			this.Outgoing.OnNext(Tuple.Create<string, Message>(leaderHint, item));
			return result;
		}

		public void BecomeLeader(TimeSpan timeout)
		{
			DxStoreCommand.PromoteToLeader promoteToLeader = new DxStoreCommand.PromoteToLeader();
			promoteToLeader.Initialize(this.self, null);
			this.ReplicateCommand(promoteToLeader, true, new TimeSpan?(timeout));
		}

		public void Stop()
		{
			if (this.truncator != null)
			{
				this.truncator.Stop();
			}
		}

		private void ExecuteLocally(int instanceNumber, Proposal<string, DxStoreCommand> proposal)
		{
			if (!this.truncator.IsStarted)
			{
				this.truncator.Start();
			}
			DxStoreCommand command = proposal.value;
			bool isTracingEnabled = ExTraceGlobals.StoreWriteTracer.IsTraceEnabled(TraceType.DebugTrace);
			WellKnownCommandName cmdTypeId = command.GetTypeId();
			this.instance.RunOperation("ExecuteCommandLocally :" + cmdTypeId, delegate
			{
				if (isTracingEnabled)
				{
					ExTraceGlobals.StoreWriteTracer.TraceDebug((long)this.instance.IdentityHash, "{0}: Execute start: [Instance#{1}] {2}: {3}", new object[]
					{
						this.instance.Identity,
						instanceNumber,
						cmdTypeId,
						command.GetDebugString()
					});
				}
				this.ExecuteCommand(instanceNumber, command);
			}, LogOptions.LogException, null, null, null, delegate(Exception exception)
			{
				if (exception != null)
				{
					if (ExTraceGlobals.StoreWriteTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.StoreWriteTracer.TraceError((long)this.instance.IdentityHash, "{0}: Execute failed: [Instance#{1}] {2}: Error: {3}", new object[]
						{
							this.instance.Identity,
							instanceNumber,
							cmdTypeId,
							exception.ToString()
						});
					}
				}
				else if (isTracingEnabled)
				{
					ExTraceGlobals.StoreWriteTracer.TraceDebug<string, int, WellKnownCommandName>((long)this.instance.IdentityHash, "{0}: Execute success: [Instance#{1}] {2}", this.instance.Identity, instanceNumber, cmdTypeId);
				}
				this.NotifyInitiatorAsync(instanceNumber, command, exception);
			});
		}

		private void ExecuteCommand(int instanceNumber, DxStoreCommand command)
		{
			WellKnownCommandName typeId = command.GetTypeId();
			bool flag = false;
			switch (typeId)
			{
			case WellKnownCommandName.CreateKey:
			{
				DxStoreCommand.CreateKey createKey = command as DxStoreCommand.CreateKey;
				if (createKey != null)
				{
					flag = true;
					bool arg = this.localDataStore.CreateKey(new int?(instanceNumber), createKey.FullKeyName);
					ExTraceGlobals.StoreWriteTracer.TraceDebug<string, bool>((long)this.instance.IdentityHash, "{0}: CreateKey - IsKeyCreated: {1}", this.instance.Identity, arg);
				}
				break;
			}
			case WellKnownCommandName.DeleteKey:
			{
				DxStoreCommand.DeleteKey deleteKey = command as DxStoreCommand.DeleteKey;
				if (deleteKey != null)
				{
					flag = true;
					bool arg2 = this.localDataStore.DeleteKey(new int?(instanceNumber), deleteKey.FullKeyName);
					ExTraceGlobals.StoreWriteTracer.TraceDebug<string, bool>((long)this.instance.IdentityHash, "{0}: DeleteKey - IsFound: {1}", this.instance.Identity, arg2);
				}
				break;
			}
			case WellKnownCommandName.SetProperty:
			{
				DxStoreCommand.SetProperty setProperty = command as DxStoreCommand.SetProperty;
				if (setProperty != null)
				{
					flag = true;
					this.localDataStore.SetProperty(new int?(instanceNumber), setProperty.KeyName, setProperty.Name, setProperty.Value);
				}
				break;
			}
			case WellKnownCommandName.DeleteProperty:
			{
				DxStoreCommand.DeleteProperty deleteProperty = command as DxStoreCommand.DeleteProperty;
				if (deleteProperty != null)
				{
					flag = true;
					bool arg3 = this.localDataStore.DeleteProperty(new int?(instanceNumber), deleteProperty.KeyName, deleteProperty.Name);
					ExTraceGlobals.StoreWriteTracer.TraceDebug<string, bool>((long)this.instance.IdentityHash, "{0}: DeleteProperty - IsFound: {1}", this.instance.Identity, arg3);
				}
				break;
			}
			case WellKnownCommandName.ExecuteBatch:
			{
				DxStoreCommand.ExecuteBatch executeBatch = command as DxStoreCommand.ExecuteBatch;
				if (executeBatch != null)
				{
					flag = true;
					this.localDataStore.ExecuteBatch(new int?(instanceNumber), executeBatch.KeyName, executeBatch.Commands);
				}
				break;
			}
			case WellKnownCommandName.ApplySnapshot:
			{
				DxStoreCommand.ApplySnapshot applySnapshot = command as DxStoreCommand.ApplySnapshot;
				if (applySnapshot != null)
				{
					flag = true;
					this.localDataStore.ApplySnapshot(applySnapshot.SnapshotInfo, new int?(instanceNumber));
				}
				break;
			}
			case WellKnownCommandName.PromoteToLeader:
			{
				DxStoreCommand.PromoteToLeader promoteToLeader = command as DxStoreCommand.PromoteToLeader;
				if (promoteToLeader != null)
				{
					flag = true;
					string value = string.Format("Promote to leader requested at {0}", promoteToLeader.TimeInitiated.ToShortString());
					PropertyValue propertyValue = new PropertyValue(value);
					this.localDataStore.SetProperty(new int?(instanceNumber), "Private\\NonStoreUpdates\\Promote", "Initiator-" + promoteToLeader.Initiator, propertyValue);
				}
				break;
			}
			case WellKnownCommandName.DummyCmd:
			{
				DxStoreCommand.DummyCommand dummyCommand = command as DxStoreCommand.DummyCommand;
				if (dummyCommand != null)
				{
					flag = true;
					string value2 = string.Format("Constrained write test for original command {0}", dummyCommand.OriginalDbCommandId);
					PropertyValue propertyValue2 = new PropertyValue(value2);
					this.localDataStore.SetProperty(new int?(instanceNumber), "Private\\NonStoreUpdates\\Dummy", "Initiator-" + dummyCommand.Initiator, propertyValue2);
				}
				break;
			}
			}
			if (!flag)
			{
				ExTraceGlobals.StoreWriteTracer.TraceError<string, WellKnownCommandName, string>((long)this.instance.IdentityHash, "{0}: Unknown command {1} (Type: {2})", this.instance.Identity, typeId, command.GetType().Name);
				this.localDataStore.SetProperty(new int?(instanceNumber), "Private\\NonStoreUpdates\\UnknownCommands", "Initiator-" + command.Initiator, new PropertyValue(command.TimeInitiated.ToShortString()));
			}
		}

		private void NotifyInitiatorAsync(int instanceNumber, DxStoreCommand command, Exception exception)
		{
			if (command.IsNotifyInitiator)
			{
				Task.Factory.StartNew(delegate()
				{
					this.NotifyInitiator(instanceNumber, command, exception);
				});
			}
		}

		private void NotifyInitiator(int instanceNumber, DxStoreCommand command, Exception exception)
		{
			DxStoreInstanceClient client = this.instance.InstanceClientFactory.GetClient(command.Initiator);
			client.NotifyInitiator(command.CommandId, this.self, instanceNumber, exception == null, (exception != null) ? exception.Message : "<none>", null);
		}

		private readonly string self;

		private readonly ILocalDataStore localDataStore;

		private readonly PeriodicPaxosTrancator truncator;

		private readonly object locker = new object();

		private readonly DxStoreInstance instance;
	}
}
