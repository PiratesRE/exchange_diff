using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplaySystemRunConfigurationUpdaterFullScan : ReplaySystemQueuedItem
	{
		public ReplaySystemRunConfigurationUpdaterFullScan(ReplicaInstanceManager riManager, ReplayConfigChangeHints changeHint) : base(riManager)
		{
			this.WaitForCompletion = false;
			this.ChangeHint = changeHint;
		}

		public bool WaitForCompletion { get; set; }

		public ReplayConfigChangeHints ChangeHint { get; private set; }

		public override bool IsEquivalentOrSuperset(IQueuedCallback otherCallback)
		{
			if (base.IsEquivalentOrSuperset(otherCallback))
			{
				return true;
			}
			if (otherCallback is ReplaySystemRunConfigurationUpdaterFullScan)
			{
				return !((ReplaySystemRunConfigurationUpdaterFullScan)otherCallback).WaitForCompletion || this.WaitForCompletion;
			}
			if (otherCallback is ReplaySystemRunConfigurationUpdaterSingleConfig)
			{
				bool flag = !((ReplaySystemRunConfigurationUpdaterSingleConfig)otherCallback).WaitForCompletion || this.WaitForCompletion;
				bool flag2 = !((ReplaySystemRunConfigurationUpdaterSingleConfig)otherCallback).ForceRestart;
				return flag && flag2;
			}
			return false;
		}

		protected override void ExecuteInternal()
		{
			try
			{
				base.ReplicaInstanceManager.ConfigurationUpdater(null, this.WaitForCompletion, false, false, this.ChangeHint);
			}
			catch (TaskServerException ex)
			{
				ReplayEventLogConstants.Tuple_ConfigUpdaterScanFailed.LogEvent(base.ReplicaInstanceManager.GetHashCode().ToString(), new object[]
				{
					ex.Message
				});
				throw;
			}
			catch (TaskServerTransientException ex2)
			{
				ReplayEventLogConstants.Tuple_ConfigUpdaterScanFailed.LogEvent(base.ReplicaInstanceManager.GetHashCode().ToString(), new object[]
				{
					ex2.Message
				});
				throw;
			}
		}
	}
}
