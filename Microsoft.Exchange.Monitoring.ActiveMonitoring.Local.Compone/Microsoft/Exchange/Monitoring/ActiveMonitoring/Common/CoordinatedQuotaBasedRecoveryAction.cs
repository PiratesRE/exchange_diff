using System;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class CoordinatedQuotaBasedRecoveryAction : CoordinatedRecoveryAction
	{
		internal CoordinatedQuotaBasedRecoveryAction(RecoveryActionId actionId, ResponderDefinition responderDefinition, string resourceName, int minimumRequiredTobeInReadyState, TimeSpan durationToCheck, string[] servers) : base(actionId, responderDefinition.Name, minimumRequiredTobeInReadyState, 1, servers)
		{
			this.resourceName = resourceName;
			this.durationToCheck = durationToCheck;
			FixedThrottleEntry throttleDefinition = Dependencies.ThrottleHelper.Settings.GetThrottleDefinition(actionId, resourceName, responderDefinition);
			this.maximumAllowedLocalActionsInTheDuration = throttleDefinition.ThrottleParameters.LocalMaximumAllowedAttemptsInADay;
			this.minimumDurationBetweenActionsAcrossGroup = TimeSpan.FromMinutes((double)throttleDefinition.ThrottleParameters.GroupMinimumMinutesBetweenAttempts);
			this.maximumAllowedTotalActionsAcrossGroup = throttleDefinition.ThrottleParameters.GroupMaximumAllowedAttemptsInADay;
			this.TotalQuotaExhaustedAcrossGroup = 0;
			this.MaximumPossibleQuotaFromRemainingServers = servers.Length * this.maximumAllowedLocalActionsInTheDuration;
		}

		internal int TotalQuotaExhaustedAcrossGroup { get; private set; }

		internal int MaximumPossibleQuotaFromRemainingServers { get; private set; }

		internal bool IsQuotaExhausted { get; set; }

		internal bool IsQuotaComputationConcluded { get; set; }

		internal bool IsActionAttemptedBeforeMinimumTime { get; set; }

		public override void EnsureArbitrationSucceeeded(CoordinatedRecoveryAction.ResourceAvailabilityStatistics stats)
		{
			ManagedAvailabilityCrimsonEvents.ArbitrationQuotaInfo.Log<string, bool, bool, bool, int, int>(base.GetType().Name, this.IsQuotaComputationConcluded, this.IsQuotaExhausted, this.IsActionAttemptedBeforeMinimumTime, this.TotalQuotaExhaustedAcrossGroup, this.maximumAllowedTotalActionsAcrossGroup);
			base.EnsureArbitrationSucceeeded(stats);
			if (!this.IsQuotaComputationConcluded || this.IsQuotaExhausted || this.IsActionAttemptedBeforeMinimumTime)
			{
				throw new ArbitrationQuotaCalculationFailedException(this.TotalQuotaExhaustedAcrossGroup, this.maximumAllowedTotalActionsAcrossGroup, this.IsQuotaComputationConcluded, this.IsActionAttemptedBeforeMinimumTime);
			}
		}

		internal string GetQuotaStatusAsString()
		{
			return string.Format("QuotaStatus: (TotalQuotaExhaustedAcrossGroup = {0}, MaximumAllowedTotalActionsAcrossGroup = {1})", this.TotalQuotaExhaustedAcrossGroup, this.maximumAllowedTotalActionsAcrossGroup);
		}

		protected void ProcessQuotaInfo(RecoveryActionHelper.RecoveryActionQuotaInfo quotaInfo, out bool isArbitrating)
		{
			isArbitrating = false;
			lock (this.locker)
			{
				if (!this.IsArbitrationCompleted())
				{
					DateTime dateTime = DateTime.UtcNow.ToLocalTime();
					int num;
					if (quotaInfo != null)
					{
						num = quotaInfo.MaxAllowedQuota - quotaInfo.RemainingQuota;
					}
					else
					{
						num = this.maximumAllowedLocalActionsInTheDuration;
					}
					this.TotalQuotaExhaustedAcrossGroup += num;
					this.MaximumPossibleQuotaFromRemainingServers -= this.maximumAllowedLocalActionsInTheDuration;
					if (this.TotalQuotaExhaustedAcrossGroup >= this.maximumAllowedTotalActionsAcrossGroup)
					{
						this.IsQuotaExhausted = true;
						this.IsQuotaComputationConcluded = true;
					}
					else
					{
						int num2 = this.maximumAllowedTotalActionsAcrossGroup - this.TotalQuotaExhaustedAcrossGroup;
						if (this.MaximumPossibleQuotaFromRemainingServers < num2)
						{
							this.IsQuotaExhausted = false;
							this.IsQuotaComputationConcluded = true;
						}
					}
					if (quotaInfo != null)
					{
						if (this.minimumDurationBetweenActionsAcrossGroup > TimeSpan.Zero && quotaInfo.LastSucceededEntry != null && quotaInfo.LastSucceededEntry.EndTime > dateTime - this.minimumDurationBetweenActionsAcrossGroup)
						{
							this.IsActionAttemptedBeforeMinimumTime = true;
							this.IsQuotaComputationConcluded = true;
						}
						if (quotaInfo.LastStartedEntry != null && quotaInfo.LastStartedEntry.StartTime > quotaInfo.LastSystemBootTime && dateTime < quotaInfo.LastStartedEntry.EndTime && (quotaInfo.LastSucceededEntry == null || quotaInfo.LastStartedEntry.StartTime > quotaInfo.LastSucceededEntry.StartTime))
						{
							isArbitrating = true;
						}
					}
				}
			}
		}

		protected override bool IsArbitrationCompleted()
		{
			return base.IsArbitrationCompleted() && this.IsQuotaComputationConcluded;
		}

		protected override ResourceAvailabilityStatus RunCheck(string serverName, out string debugInfo)
		{
			debugInfo = string.Empty;
			DateTime dateTime = DateTime.UtcNow.ToLocalTime();
			DateTime queryStartTime = dateTime - this.durationToCheck;
			RecoveryActionHelper.RecoveryActionQuotaInfo recoveryActionQuotaInfo = null;
			bool flag = false;
			try
			{
				recoveryActionQuotaInfo = RpcGetRecoveryActionQuotaInfoImpl.SendRequest(serverName, base.ActionId, this.resourceName, this.maximumAllowedLocalActionsInTheDuration, queryStartTime, dateTime, 30000);
				this.ProcessQuotaInfo(recoveryActionQuotaInfo, out flag);
			}
			catch (Exception)
			{
				this.ProcessQuotaInfo(null, out flag);
				throw;
			}
			ResourceAvailabilityStatus result;
			if (flag)
			{
				result = ResourceAvailabilityStatus.Arbitrating;
			}
			else
			{
				result = ResourceAvailabilityStatus.Ready;
			}
			debugInfo = string.Format("[ActionId={0}, MaxAllowedQuota={1}, RemainingQuota={2}, IsTimedout={3}, LastOperationStartTime={4}, LastOperationEndTime={5}]", new object[]
			{
				recoveryActionQuotaInfo.ActionId,
				recoveryActionQuotaInfo.MaxAllowedQuota,
				recoveryActionQuotaInfo.RemainingQuota,
				recoveryActionQuotaInfo.IsTimedout,
				(recoveryActionQuotaInfo.LastStartedEntry != null) ? recoveryActionQuotaInfo.LastStartedEntry.StartTime : DateTime.MinValue,
				(recoveryActionQuotaInfo.LastSucceededEntry != null) ? recoveryActionQuotaInfo.LastSucceededEntry.EndTime : DateTime.MinValue
			});
			return result;
		}

		private readonly string resourceName;

		private readonly TimeSpan durationToCheck;

		private readonly int maximumAllowedLocalActionsInTheDuration;

		private readonly TimeSpan minimumDurationBetweenActionsAcrossGroup;

		private readonly int maximumAllowedTotalActionsAcrossGroup;

		private object locker = new object();
	}
}
