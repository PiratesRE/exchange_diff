using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplaySystemRunConfigurationUpdaterSingleConfig : ReplaySystemDatabaseQueuedItem
	{
		public ReplaySystemRunConfigurationUpdaterSingleConfig(ReplicaInstanceManager riManager, Guid dbGuid, ReplayConfigChangeHints changeHint) : base(riManager, dbGuid)
		{
			this.WaitForCompletion = false;
			this.IsHighPriority = false;
			this.ForceRestart = false;
			this.ChangeHint = changeHint;
		}

		public bool WaitForCompletion { get; set; }

		public bool IsHighPriority { get; set; }

		public bool ForceRestart { get; set; }

		public ReplayConfigChangeHints ChangeHint { get; private set; }

		public override bool IsEquivalentOrSuperset(IQueuedCallback otherCallback)
		{
			if (base.IsEquivalentOrSuperset(otherCallback))
			{
				return true;
			}
			if (otherCallback is ReplaySystemRunConfigurationUpdaterSingleConfig)
			{
				bool flag = ((ReplaySystemRunConfigurationUpdaterSingleConfig)otherCallback).DbGuid.Equals(base.DbGuid);
				bool flag2 = !((ReplaySystemRunConfigurationUpdaterSingleConfig)otherCallback).WaitForCompletion || this.WaitForCompletion;
				bool flag3 = !((ReplaySystemRunConfigurationUpdaterSingleConfig)otherCallback).ForceRestart || this.ForceRestart;
				return flag && flag2 && flag3;
			}
			return false;
		}

		protected override void ExecuteInternal()
		{
			Dependencies.ADConfig.Refresh(string.Format("ReplaySystemRunConfigurationUpdaterSingleConfig: db={0}, hint={1}", base.DbGuid, this.ChangeHint));
			base.ReplicaInstanceManager.ConfigurationUpdater(new Guid?(base.DbGuid), this.WaitForCompletion, this.IsHighPriority, this.ForceRestart, this.ChangeHint);
		}
	}
}
