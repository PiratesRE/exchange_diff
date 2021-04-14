using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.DxStore.HA.Events;

namespace Microsoft.Exchange.DxStore.HA
{
	internal class DataStorePeriodicChecker : TimerComponent
	{
		public DataStorePeriodicChecker(int startupDelayInSec, int intervalInSec) : base(TimeSpan.FromSeconds((double)startupDelayInSec), TimeSpan.FromSeconds((double)intervalInSec), "Database Consistency Checker")
		{
			this.lastRecordedDueTime = DateTimeOffset.MinValue;
		}

		public DataStorePeriodicChecker() : this(RegistryParameters.DistributedStoreConsistencyStartupDelayInSecs, RegistryParameters.DistributedStoreConsistencyCheckPeriodicIntervalInSecs)
		{
		}

		protected override void TimerCallbackInternal()
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config != null && config.IsPAM)
			{
				this.CheckDataStoresBestEffort();
			}
		}

		internal T GetDxStorePrivateProperty<T>(string propertyName, T defaultValue)
		{
			T value;
			using (IDistributedStoreKey baseKey = DistributedStore.Instance.DxStoreKeyFactoryInstance.GetBaseKey(DxStoreKeyAccessMode.Read, null, null, true))
			{
				value = baseKey.GetValue(propertyName, defaultValue, null);
			}
			return value;
		}

		internal void SetDxStorePrivateProperty<T>(string propertyName, T propertyValue)
		{
			using (IDistributedStoreKey baseKey = DistributedStore.Instance.DxStoreKeyFactoryInstance.GetBaseKey(DxStoreKeyAccessMode.Write, null, null, true))
			{
				baseKey.SetValue(propertyName, propertyValue, false, null);
			}
		}

		internal DateTimeOffset GetLastAnalyzeTime()
		{
			string dxStorePrivateProperty = this.GetDxStorePrivateProperty<string>("LastSuccessfulDataStoreAnalyzeTime", string.Empty);
			if (string.IsNullOrWhiteSpace(dxStorePrivateProperty))
			{
				return DateTimeOffset.MinValue;
			}
			DateTimeOffset result;
			if (!DateTimeOffset.TryParse(dxStorePrivateProperty, out result))
			{
				result = DateTimeOffset.MinValue;
			}
			result = result.ToLocalTime();
			return result;
		}

		internal void SetLastAnalyzeTimeToNow()
		{
			this.SetDxStorePrivateProperty<string>("LastSuccessfulDataStoreAnalyzeTime", DateTimeOffset.UtcNow.ToString("o"));
		}

		internal void CheckDataStoresBestEffort()
		{
			Exception ex = Utils.RunBestEffort(delegate
			{
				this.CheckDataStores();
			});
			if (ex != null)
			{
				DxStoreHACrimsonEvents.DataStoreValidationFailed.Log<string, Exception>("CheckDataStores() failed.", ex);
			}
		}

		internal void CheckDataStores()
		{
			IActiveManagerSettings settings = DxStoreSetting.Instance.GetSettings();
			if (!settings.DxStoreIsPeriodicFixupEnabled || settings.DxStoreRunMode != DxStoreMode.Shadow)
			{
				return;
			}
			DateTimeOffset lastUpdateTime = DateTimeOffset.MinValue;
			DataStoreSnapshotAnalyzer analyzer = new DataStoreSnapshotAnalyzer((DiffReportVerboseMode)RegistryParameters.DistributedStoreDiffReportVerboseFlags);
			string text = null;
			if (!analyzer.IsPaxosConfiguredAndLeaderExist(out text))
			{
				if (!this.isPaxosNotReadyLogged)
				{
					DxStoreHACrimsonEvents.DataStoreValidationSkipped.Log<string, string>("Paxos either not configured or paxos leader does not exist", text);
					this.isPaxosNotReadyLogged = true;
				}
				return;
			}
			this.isPaxosNotReadyLogged = false;
			Exception ex = Utils.RunBestEffort(delegate
			{
				lastUpdateTime = this.GetLastAnalyzeTime();
			});
			if (ex != null)
			{
				if (!this.isLastAnalyzeTimeFailureLogged)
				{
					DxStoreHACrimsonEvents.DataStoreValidationFailed.Log<string, Exception>("GetLastAnalyzeTime", ex);
					this.isLastAnalyzeTimeFailureLogged = true;
				}
				return;
			}
			this.isLastAnalyzeTimeFailureLogged = false;
			DateTimeOffset now = DateTimeOffset.Now;
			DateTimeOffset dateTimeOffset = now;
			if (lastUpdateTime != DateTimeOffset.MinValue)
			{
				dateTimeOffset = lastUpdateTime.Add(TimeSpan.FromSeconds((double)RegistryParameters.DistributedStoreConsistencyVerifyIntervalInSecs));
			}
			if (dateTimeOffset > now)
			{
				if (this.lastRecordedDueTime < dateTimeOffset)
				{
					DxStoreHACrimsonEvents.DataStoreValidationSkipped.Log<string, DateTimeOffset>("Time not elapsed", dateTimeOffset);
					this.lastRecordedDueTime = dateTimeOffset;
				}
				return;
			}
			ex = Utils.RunBestEffort(delegate
			{
				analyzer.AnalyzeDataStores();
			});
			string timingInfoAsString = analyzer.GetTimingInfoAsString();
			if (ex != null)
			{
				DxStoreHACrimsonEvents.DataStoreValidationFailed.Log<string, Exception>(string.Format("AnalyzeDataStores() failed. Phase: {0}, Timing: {1}", analyzer.AnalysisPhase, timingInfoAsString), ex);
				return;
			}
			DataStoreDiffReport report = analyzer.Container.Report;
			DxStoreHACrimsonEvents.DataStoreValidationCompleted.Log<bool, string>(report.IsEverythingMatches, timingInfoAsString);
			analyzer.LogDiffDetailsToEventLog();
			this.SetLastAnalyzeTimeToNow();
			bool flag = false;
			if (!report.IsEverythingMatches)
			{
				if (RegistryParameters.DistributedStoreDisableDxStoreFixUp)
				{
					DxStoreHACrimsonEvents.DataStoreValidationSkipped.Log<string, string>("Database fixup skipped since it is disabled in registry", string.Empty);
				}
				else if (report.TotalClusdbPropertiesCount > 0)
				{
					if (!this.IsLastLogPropertiesAreTheOnlyDifference(report))
					{
						flag = true;
					}
					else
					{
						DxStoreHACrimsonEvents.DataStoreValidationSkipped.Log<string, string>("Database fixup skipped since last log properties are the only entries that have changed", string.Empty);
					}
				}
				else
				{
					DxStoreHACrimsonEvents.DataStoreValidationSkipped.Log<string, string>("Database fixup skipped since clusdb does not have single property", string.Empty);
				}
			}
			if (flag)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ex = Utils.RunBestEffort(delegate
				{
					analyzer.CopyClusdbSnapshotToDxStore();
				});
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				if (ex != null)
				{
					DxStoreHACrimsonEvents.DataStoreFailedToUpdate.Log<long, Exception>(stopwatch.ElapsedMilliseconds, ex);
					return;
				}
				DxStoreHACrimsonEvents.DataStoreSuccessfulyUpdated.Log<long, string>(stopwatch.ElapsedMilliseconds, string.Empty);
			}
		}

		internal bool IsLastLogPropertiesAreTheOnlyDifference(DataStoreDiffReport report)
		{
			if (report.CountKeysOnlyInClusdb == 0 && report.CountKeysOnlyInDxStore == 0 && report.CountKeysInClusdbAndDxStoreButPropertiesDifferent == 1)
			{
				DataStoreMergedContainer.KeyEntry keyEntry = report.KeysInBothButPropertiesMismatch.FirstOrDefault<DataStoreMergedContainer.KeyEntry>();
				if (keyEntry != null && Utils.IsEqual(keyEntry.Name, "\\ExchangeActiveManager\\LastLog", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public const string LastSuccessfulAnalyzeTimePropertyName = "LastSuccessfulDataStoreAnalyzeTime";

		private DateTimeOffset lastRecordedDueTime;

		private bool isPaxosNotReadyLogged;

		private bool isLastAnalyzeTimeFailureLogged;
	}
}
