using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AutoReseedWorkflowSuppression
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AutoReseedTracer;
			}
		}

		public AutoReseedWorkflowSuppression()
		{
			this.m_healthyWorkflowSuppression = new TransientDatabaseErrorSuppression();
			TransientPeriodicDatabaseErrorSuppression value = new TransientPeriodicDatabaseErrorSuppression(TimeSpan.Zero, InvokeWithTimeout.InfiniteTimeSpan, AutoReseedWorkflowSuppression.s_dbFailedSuppresionInterval, AutoReseedWorkflowSuppression.s_dbFailedSuppresionInterval, TransientErrorInfo.ErrorType.Success);
			TransientPeriodicDatabaseErrorSuppression value2 = new TransientPeriodicDatabaseErrorSuppression(TimeSpan.Zero, InvokeWithTimeout.InfiniteTimeSpan, AutoReseedWorkflowSuppression.s_dbReseedSuppresionInterval, AutoReseedWorkflowSuppression.s_dbReseedRetryInterval, TransientErrorInfo.ErrorType.Success);
			TransientPeriodicDatabaseErrorSuppression value3 = new TransientPeriodicDatabaseErrorSuppression(TimeSpan.Zero, InvokeWithTimeout.InfiniteTimeSpan, AutoReseedWorkflowSuppression.s_ciReseedSuppresionInterval, AutoReseedWorkflowSuppression.s_ciReseedRetryInterval, TransientErrorInfo.ErrorType.Success);
			TransientPeriodicDatabaseErrorSuppression value4 = new TransientPeriodicDatabaseErrorSuppression(TimeSpan.Zero, InvokeWithTimeout.InfiniteTimeSpan, AutoReseedWorkflowSuppression.s_ciRebuildSuppresionInterval, AutoReseedWorkflowSuppression.s_ciRebuildRetryInterval, TransientErrorInfo.ErrorType.Success);
			this.m_wfSuppressionTable = new Dictionary<AutoReseedWorkflowType, TransientPeriodicDatabaseErrorSuppression>
			{
				{
					AutoReseedWorkflowType.FailedCopy,
					value
				},
				{
					AutoReseedWorkflowType.FailedSuspendedCopyAutoReseed,
					value2
				},
				{
					AutoReseedWorkflowType.CatalogAutoReseed,
					value3
				},
				{
					AutoReseedWorkflowType.FailedSuspendedCatalogRebuild,
					value4
				}
			};
			this.m_catalogAutoReseedElapsedTime = new Dictionary<Guid, DateTime>();
		}

		public bool ReportWorkflowLaunchConditionMet(AutoReseedWorkflowType workflowType, Guid dbGuid, CatalogAutoReseedWorkflow.CatalogAutoReseedReason reason = CatalogAutoReseedWorkflow.CatalogAutoReseedReason.None, int activationPreference = 1)
		{
			bool flag = false;
			AutoReseedWorkflowSuppression.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedWorkflowSuppression.ReportWorkflowLaunchConditionMet: Database '{0}' running workflow {1} with ActivationPreference {2} and reason {3}", new object[]
			{
				dbGuid,
				workflowType,
				activationPreference,
				reason
			});
			foreach (KeyValuePair<AutoReseedWorkflowType, TransientPeriodicDatabaseErrorSuppression> keyValuePair in this.m_wfSuppressionTable)
			{
				if (keyValuePair.Key == workflowType)
				{
					TransientErrorInfo.ErrorType errorType;
					flag = (keyValuePair.Value.ReportFailurePeriodic(dbGuid, out errorType) && errorType == TransientErrorInfo.ErrorType.Failure);
					AutoReseedWorkflowSuppression.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedWorkflowSuppression.ReportWorkflowLaunchConditionMet: Database '{0}' is reporting failure on periodic suppression fail table with ErrorType {1} for workflow {2}, shouldLaunch {3}.", new object[]
					{
						dbGuid,
						errorType,
						keyValuePair.Key,
						flag
					});
				}
				else
				{
					TransientErrorInfo.ErrorType errorType;
					keyValuePair.Value.ReportSuccessPeriodic(dbGuid, out errorType);
					AutoReseedWorkflowSuppression.Tracer.TraceDebug<Guid, TransientErrorInfo.ErrorType, AutoReseedWorkflowType>((long)this.GetHashCode(), "AutoReseedWorkflowSuppression.ReportWorkflowLaunchConditionMet: Database '{0}' is resetting periodic suppression fail table with ErrorType {1} for workflow {2}.", dbGuid, errorType, keyValuePair.Key);
				}
			}
			if (flag && workflowType == AutoReseedWorkflowType.CatalogAutoReseed && reason == CatalogAutoReseedWorkflow.CatalogAutoReseedReason.Upgrade)
			{
				DateTime utcNow = DateTime.UtcNow;
				activationPreference = ((activationPreference <= 1) ? 1 : activationPreference);
				TimeSpan timeSpan = TimeSpan.FromSeconds((double)(activationPreference * (int)AutoReseedWorkflowSuppression.s_ciCatalogAutoReseedOnUpgradeInterval.TotalSeconds));
				DateTime value;
				if (this.m_catalogAutoReseedElapsedTime.TryGetValue(dbGuid, out value))
				{
					TimeSpan timeSpan2 = utcNow.Subtract(value);
					if (timeSpan2 < timeSpan)
					{
						flag = false;
						AutoReseedWorkflowSuppression.Tracer.TraceDebug((long)this.GetHashCode(), "AutoReseedWorkflowSuppression.ReportWorkflowLaunchConditionMet: Database '{0}' blocking reseed elapsed time {1} < delay time {2}. ActivationPref is {3}", new object[]
						{
							dbGuid,
							timeSpan2,
							timeSpan,
							activationPreference
						});
					}
					else
					{
						this.m_catalogAutoReseedElapsedTime.Remove(dbGuid);
						AutoReseedWorkflowSuppression.Tracer.TraceDebug<Guid, TimeSpan, int>((long)this.GetHashCode(), "AutoReseedWorkflowSuppression.ReportWorkflowLaunchConditionMet: Database '{0}' allowing reseed at current time as a previous reseed was called and delayed {1}. ActivationPref is {2}", dbGuid, timeSpan, activationPreference);
					}
				}
				else
				{
					this.m_catalogAutoReseedElapsedTime[dbGuid] = utcNow;
					flag = false;
					AutoReseedWorkflowSuppression.Tracer.TraceDebug<Guid, TimeSpan, int>((long)this.GetHashCode(), "AutoReseedWorkflowSuppression.ReportWorkflowLaunchConditionMet: Database '{0}' blocking reseed for {1} as this is the first call. ActivationPref is {2}", dbGuid, timeSpan, activationPreference);
				}
			}
			return flag;
		}

		public void ReportNoWorkflowsNeedToLaunch(Guid dbGuid)
		{
			foreach (TransientPeriodicDatabaseErrorSuppression transientPeriodicDatabaseErrorSuppression in this.m_wfSuppressionTable.Values)
			{
				TransientErrorInfo.ErrorType errorType;
				transientPeriodicDatabaseErrorSuppression.ReportSuccessPeriodic(dbGuid, out errorType);
			}
		}

		public bool ReportHealthyWorkflowLaunchConditionMet(Guid dbGuid)
		{
			return this.m_healthyWorkflowSuppression.ReportFailure(dbGuid, AutoReseedWorkflowSuppression.s_dbHealthySuppressionInterval);
		}

		public void ReportHealthyWorkflowNotNeeded(Guid dbGuid)
		{
			this.m_healthyWorkflowSuppression.ReportSuccess(dbGuid);
		}

		internal static readonly TimeSpan s_dbHealthySuppressionInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbHealthySuppressionInSecs);

		internal static readonly TimeSpan s_dbFailedSuppresionInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedPeriodicIntervalInSecs);

		internal static readonly TimeSpan s_dbReseedSuppresionInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedSuspendedSuppressionInSecs);

		internal static readonly TimeSpan s_dbReseedRetryInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedDbFailedSuspendedPeriodicIntervalInSecs);

		internal static readonly TimeSpan s_ciReseedSuppresionInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedCiSuppressionInSecs);

		internal static readonly TimeSpan s_ciReseedRetryInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedCiPeriodicIntervalInSecs);

		internal static readonly TimeSpan s_ciRebuildSuppresionInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedCiRebuildFailedSuspendedSuppressionInSecs);

		internal static readonly TimeSpan s_ciRebuildRetryInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedCiRebuildFailedSuspendedPeriodicIntervalInSecs);

		internal static readonly TimeSpan s_ciCatalogAutoReseedOnUpgradeInterval = TimeSpan.FromSeconds((double)RegistryParameters.AutoReseedCiCatalogOnUpgradeIntervalInSecs);

		private readonly TransientDatabaseErrorSuppression m_healthyWorkflowSuppression;

		private readonly Dictionary<AutoReseedWorkflowType, TransientPeriodicDatabaseErrorSuppression> m_wfSuppressionTable;

		private Dictionary<Guid, DateTime> m_catalogAutoReseedElapsedTime;
	}
}
