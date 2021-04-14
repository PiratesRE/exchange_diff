using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal sealed class EhfSyncErrorTracker
	{
		public int AllTransientFailuresCount
		{
			get
			{
				return this.transientFailureCount + this.criticalTransientFailureCount;
			}
		}

		public int CriticalTransientFailureCount
		{
			get
			{
				return this.criticalTransientFailureCount;
			}
		}

		public int PermanentFailureCount
		{
			get
			{
				return this.permanentFailureCount;
			}
		}

		public bool HasTooManyFailures
		{
			get
			{
				int ehfAdminSyncMaxFailureCount = this.ehfSyncAppConfig.EhfAdminSyncMaxFailureCount;
				return this.criticalTransientFailureCount != 0 && (this.criticalTransientFailureCount > ehfAdminSyncMaxFailureCount || this.permanentFailureCount > ehfAdminSyncMaxFailureCount || this.transientFailuresInCurrentCycle.Count > ehfAdminSyncMaxFailureCount);
			}
		}

		public bool HasTransientFailure
		{
			get
			{
				return this.criticalTransientFailureCount > 0 || this.transientFailuresInCurrentCycle.Count > 0;
			}
		}

		public void PrepareForNewSyncCycle(EnhancedTimeSpan syncInterval)
		{
			this.transientFailureDetails = new Dictionary<EhfCompanyIdentity, List<EhfTransientFailureInfo>>();
			this.criticalTransientFailureCount = 0;
			this.permanentFailureCount = 0;
			this.transientFailureCount = 0;
			ExDateTime utcNow = ExDateTime.UtcNow;
			Dictionary<Guid, int> dictionary = new Dictionary<Guid, int>();
			if (utcNow < this.lastSyncTime + 2L * syncInterval)
			{
				foreach (EhfCompanyIdentity ehfCompanyIdentity in this.transientFailuresInCurrentCycle)
				{
					int num;
					if (this.transientFailuresHistory.TryGetValue(ehfCompanyIdentity.EhfCompanyGuid, out num))
					{
						dictionary.Add(ehfCompanyIdentity.EhfCompanyGuid, num + 1);
					}
					else
					{
						dictionary.Add(ehfCompanyIdentity.EhfCompanyGuid, 1);
					}
				}
			}
			this.transientFailuresHistory = dictionary;
			this.transientFailuresInCurrentCycle = new HashSet<EhfCompanyIdentity>();
			this.lastSyncTime = utcNow;
		}

		public void AddTransientFailure(EhfCompanyIdentity companyIdentity, Exception failureException, string operationName)
		{
			if (companyIdentity == null)
			{
				throw new ArgumentNullException("companyIdentity");
			}
			this.transientFailuresInCurrentCycle.Add(companyIdentity);
			EhfTransientFailureInfo item = new EhfTransientFailureInfo(failureException, operationName);
			List<EhfTransientFailureInfo> list;
			if (!this.transientFailureDetails.TryGetValue(companyIdentity, out list))
			{
				list = new List<EhfTransientFailureInfo>();
				this.transientFailureDetails[companyIdentity] = list;
			}
			list.Add(item);
			this.transientFailureCount++;
		}

		public void AddPermanentFailure()
		{
			this.permanentFailureCount++;
		}

		public void AddCriticalFailure()
		{
			this.criticalTransientFailureCount++;
		}

		public void AbortSyncCycleIfRequired(EhfAdminAccountSynchronizer ehfAdminAccountSynchronizer, EdgeSyncDiag diagSession)
		{
			if (this.criticalTransientFailureCount != 0)
			{
				ehfAdminAccountSynchronizer.LogAndAbortSyncCycle();
			}
			foreach (EhfCompanyIdentity ehfCompanyIdentity in this.transientFailuresInCurrentCycle)
			{
				int num;
				if (!this.transientFailuresHistory.TryGetValue(ehfCompanyIdentity.EhfCompanyGuid, out num))
				{
					num = 0;
				}
				int num2 = num + 1;
				if (num2 < this.ehfSyncAppConfig.EhfAdminSyncTransientFailureRetryThreshold)
				{
					ehfAdminAccountSynchronizer.LogAndAbortSyncCycle();
				}
			}
			foreach (KeyValuePair<EhfCompanyIdentity, List<EhfTransientFailureInfo>> keyValuePair in this.transientFailureDetails)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (EhfTransientFailureInfo ehfTransientFailureInfo in keyValuePair.Value)
				{
					stringBuilder.AppendLine(ehfTransientFailureInfo.OperationName);
					stringBuilder.AppendLine(ehfTransientFailureInfo.FailureException.ToString());
					stringBuilder.AppendLine();
				}
				diagSession.EventLog.LogEvent(EdgeSyncEventLogConstants.Tuple_EhfAdminSyncTransientFailureRetryThresholdReached, null, new object[]
				{
					keyValuePair.Key.EhfCompanyId,
					keyValuePair.Key.EhfCompanyGuid,
					stringBuilder.ToString()
				});
			}
		}

		public void Initialize(EhfSyncAppConfig ehfSyncAppConfig)
		{
			this.ehfSyncAppConfig = ehfSyncAppConfig;
		}

		private int criticalTransientFailureCount;

		private int permanentFailureCount;

		private int transientFailureCount;

		private ExDateTime lastSyncTime = ExDateTime.MinValue;

		private EhfSyncAppConfig ehfSyncAppConfig;

		private Dictionary<Guid, int> transientFailuresHistory = new Dictionary<Guid, int>();

		private Dictionary<EhfCompanyIdentity, List<EhfTransientFailureInfo>> transientFailureDetails = new Dictionary<EhfCompanyIdentity, List<EhfTransientFailureInfo>>();

		private HashSet<EhfCompanyIdentity> transientFailuresInCurrentCycle = new HashSet<EhfCompanyIdentity>();
	}
}
