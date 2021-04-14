using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class MultiReplicationCheck : DisposeTrackableBase
	{
		public bool BreakOnCriticalFailures
		{
			get
			{
				return this.m_breakOnCriticalFailures;
			}
			set
			{
				this.m_breakOnCriticalFailures = value;
			}
		}

		public MultiReplicationCheck(string serverName, IEventManager eventManager, string momEventSource) : this(serverName, eventManager, momEventSource, null, null, null, 0U)
		{
		}

		public MultiReplicationCheck(string serverName, IEventManager eventManager, string momEventSource, IADDatabaseAvailabilityGroup dag, uint ignoreTransientErrorsThreshold) : this(serverName, eventManager, momEventSource, null, null, null, dag, ignoreTransientErrorsThreshold)
		{
		}

		public MultiReplicationCheck(string serverName, IEventManager eventManager, string momEventSource, List<ReplayConfiguration> replayConfigs, uint ignoreTransientErrorsThreshold) : this(serverName, eventManager, momEventSource, null, replayConfigs, null, ignoreTransientErrorsThreshold)
		{
		}

		public MultiReplicationCheck(string serverName, IEventManager eventManager, string momEventSource, DatabaseHealthValidationRunner validationRunner, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses, uint ignoreTransientErrorsThreshold) : this(serverName, eventManager, momEventSource, validationRunner, replayConfigs, copyStatuses, null, ignoreTransientErrorsThreshold)
		{
		}

		public MultiReplicationCheck(string serverName, IEventManager eventManager, string momEventSource, DatabaseHealthValidationRunner validationRunner, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses, IADDatabaseAvailabilityGroup dag, uint ignoreTransientErrorsThreshold)
		{
			this.m_ServerName = serverName;
			this.m_EventManager = eventManager;
			this.m_MomEventSource = momEventSource;
			this.m_ValidationRunner = validationRunner;
			this.m_ReplayConfigs = replayConfigs;
			this.m_CopyStatuses = copyStatuses;
			this.m_Dag = dag;
			this.m_IgnoreTransientErrorsThreshold = ignoreTransientErrorsThreshold;
			this.Initialize();
			this.BuildJaggedArray();
		}

		protected abstract void Initialize();

		private void BuildJaggedArray()
		{
			int length = Enum.GetValues(typeof(CheckCategory)).Length;
			this.m_JaggedArrayChecks = new List<IReplicationCheck>[length];
			for (int i = 0; i < length; i++)
			{
				this.m_JaggedArrayChecks[i] = new List<IReplicationCheck>();
			}
			foreach (IReplicationCheck replicationCheck in this.m_Checks)
			{
				this.m_JaggedArrayChecks[(int)replicationCheck.Category].Add(replicationCheck);
			}
		}

		public virtual void Run()
		{
			for (int i = 0; i < this.m_JaggedArrayChecks.Length; i++)
			{
				foreach (IReplicationCheck replicationCheck in this.m_JaggedArrayChecks[i])
				{
					if (this.m_breakOnCriticalFailures)
					{
						if (!this.m_breakOnCriticalFailures)
						{
							continue;
						}
						if (i != 0)
						{
							if (this.m_CriticalCheckHasFailed)
							{
								continue;
							}
						}
					}
					try
					{
						replicationCheck.Run();
					}
					catch (ReplicationCheckHighPriorityFailedException)
					{
						this.m_CriticalCheckHasFailed = true;
					}
					catch (ReplicationCheckFailedException)
					{
					}
					catch (ReplicationCheckWarningException)
					{
					}
				}
			}
		}

		public void LogEvents()
		{
			for (int i = 0; i < this.m_JaggedArrayChecks.Length; i++)
			{
				foreach (IReplicationCheck replicationCheck in this.m_JaggedArrayChecks[i])
				{
					ReplicationCheck replicationCheck2 = (ReplicationCheck)replicationCheck;
					if (replicationCheck2.HasRun)
					{
						replicationCheck2.LogEvents();
					}
				}
			}
		}

		public List<ReplicationCheckOutcome> GetAllOutcomes()
		{
			List<ReplicationCheckOutcome> list = new List<ReplicationCheckOutcome>();
			for (int i = 0; i < this.m_JaggedArrayChecks.Length; i++)
			{
				foreach (IReplicationCheck replicationCheck in this.m_JaggedArrayChecks[i])
				{
					ReplicationCheck replicationCheck2 = (ReplicationCheck)replicationCheck;
					if (replicationCheck2.HasRun)
					{
						list.Add(replicationCheck2.GetCheckOutcome());
					}
				}
			}
			return list;
		}

		public List<ReplicationCheckOutputObject> GetAllOutputObjects()
		{
			List<ReplicationCheckOutputObject> list = new List<ReplicationCheckOutputObject>();
			for (int i = 0; i < this.m_JaggedArrayChecks.Length; i++)
			{
				foreach (IReplicationCheck replicationCheck in this.m_JaggedArrayChecks[i])
				{
					ReplicationCheck replicationCheck2 = (ReplicationCheck)replicationCheck;
					if (replicationCheck2.HasRun)
					{
						List<ReplicationCheckOutputObject> checkOutputObjects = replicationCheck2.GetCheckOutputObjects();
						list.AddRange(checkOutputObjects);
					}
				}
			}
			return list;
		}

		internal IReplicationCheck[] Checks
		{
			get
			{
				return this.m_Checks;
			}
		}

		internal List<IReplicationCheck>[] JaggedArrayChecks
		{
			get
			{
				return this.m_JaggedArrayChecks;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MultiReplicationCheck>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			foreach (ReplicationCheck replicationCheck in this.m_Checks)
			{
				if (replicationCheck != null)
				{
					replicationCheck.Dispose();
				}
			}
		}

		protected IReplicationCheck[] m_Checks;

		protected bool m_breakOnCriticalFailures = true;

		protected IEventManager m_EventManager;

		protected List<ReplayConfiguration> m_ReplayConfigs;

		protected Dictionary<Guid, RpcDatabaseCopyStatus2> m_CopyStatuses;

		protected IADDatabaseAvailabilityGroup m_Dag;

		protected DatabaseHealthValidationRunner m_ValidationRunner;

		protected string m_MomEventSource;

		protected uint m_IgnoreTransientErrorsThreshold;

		protected string m_ServerName;

		private List<IReplicationCheck>[] m_JaggedArrayChecks;

		private bool m_CriticalCheckHasFailed;
	}
}
