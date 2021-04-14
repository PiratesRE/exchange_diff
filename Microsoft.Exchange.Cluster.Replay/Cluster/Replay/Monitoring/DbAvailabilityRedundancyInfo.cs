using System;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	internal class DbAvailabilityRedundancyInfo
	{
		public DbAvailabilityRedundancyInfo.NCopyStateTransitionInfo RedundancyInfo { get; private set; }

		public DbAvailabilityRedundancyInfo.NCopyStateTransitionInfo AvailabilityInfo { get; private set; }

		public int RedundancyCount { get; private set; }

		public int AvailabilityCount { get; private set; }

		private DbHealthInfo DbInfo { get; set; }

		public DbAvailabilityRedundancyInfo(DbHealthInfo dbInfo)
		{
			this.DbInfo = dbInfo;
			this.RedundancyInfo = new DbAvailabilityRedundancyInfo.NCopyStateTransitionInfo();
			this.AvailabilityInfo = new DbAvailabilityRedundancyInfo.NCopyStateTransitionInfo();
		}

		public void Update()
		{
			DbHealthInfo dbInfo = this.DbInfo;
			if (!dbInfo.DbFoundInAD.IsSuccess || dbInfo.DbServerInfos.Count == 0)
			{
				this.AvailabilityInfo.UpdateStates(0);
				this.RedundancyInfo.UpdateStates(0);
				return;
			}
			this.AvailabilityCount = this.GetHealthyCopyCount((DbCopyHealthInfo copyInfo) => copyInfo.CopyIsAvailable);
			this.RedundancyCount = this.GetHealthyCopyCount((DbCopyHealthInfo copyInfo) => copyInfo.CopyIsRedundant);
			this.AvailabilityInfo.UpdateStates(this.AvailabilityCount);
			this.RedundancyInfo.UpdateStates(this.RedundancyCount);
		}

		public int GetHealthyCopyCount(Func<DbCopyHealthInfo, StateTransitionInfo> healthCheckStateGetter)
		{
			int num = 0;
			foreach (DbCopyHealthInfo copyInfo in this.DbInfo.DbServerInfos.Values)
			{
				if (this.IsCopyHealthy(copyInfo, healthCheckStateGetter))
				{
					num++;
				}
			}
			return num;
		}

		public bool IsCopyHealthy(DbCopyHealthInfo copyInfo, Func<DbCopyHealthInfo, StateTransitionInfo> healthCheckStateGetter)
		{
			if (!copyInfo.CopyFoundInAD.IsSuccess)
			{
				return false;
			}
			if (!copyInfo.CopyStatusRetrieved.IsSuccess)
			{
				return false;
			}
			StateTransitionInfo stateTransitionInfo = healthCheckStateGetter(copyInfo);
			return stateTransitionInfo.IsSuccess;
		}

		public class NCopyStateTransitionInfo
		{
			public NCopyStateTransitionInfo()
			{
				for (int i = 0; i < this.m_infos.Length; i++)
				{
					this.m_infos[i] = new StateTransitionInfo();
				}
			}

			public StateTransitionInfo this[int i]
			{
				get
				{
					int num = this.SanitizeIndex(i);
					return this.m_infos[num - 1];
				}
				set
				{
					int num = this.SanitizeIndex(i);
					this.m_infos[num - 1] = value;
				}
			}

			public void UpdateStates(int healthyCopyCount)
			{
				healthyCopyCount = Math.Min(healthyCopyCount, 4);
				for (int i = 1; i <= 4; i++)
				{
					StateTransitionInfo stateTransitionInfo = this[i];
					if (i <= healthyCopyCount)
					{
						stateTransitionInfo.ReportSuccess();
					}
					else
					{
						stateTransitionInfo.ReportFailure();
					}
				}
			}

			public void ForEach(Action<StateTransitionInfo> doSomething)
			{
				for (int i = 0; i < this.m_infos.Length; i++)
				{
					StateTransitionInfo obj = this.m_infos[i];
					doSomething(obj);
				}
			}

			private int SanitizeIndex(int i)
			{
				int val = Math.Min(i, 4);
				return Math.Max(val, 1);
			}

			public const int MinNumCopies = 1;

			public const int MaxNumCopies = 4;

			private StateTransitionInfo[] m_infos = new StateTransitionInfo[4];
		}
	}
}
