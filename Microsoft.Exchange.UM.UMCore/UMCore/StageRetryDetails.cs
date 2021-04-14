using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class StageRetryDetails
	{
		internal StageRetryDetails(StageRetryDetails.FinalAction finalAction, TimeSpan retryInterval, TimeSpan allowedTimeForRetries)
		{
			this.finalAction = finalAction;
			this.retryInterval = retryInterval;
			this.stageEnteredTime = ExDateTime.UtcNow;
			this.lastRunTime = ExDateTime.UtcNow;
			this.allowedTimeForRetries = allowedTimeForRetries;
		}

		internal StageRetryDetails(StageRetryDetails.FinalAction finalAction) : this(finalAction, TimeSpan.Zero, TimeSpan.Zero)
		{
		}

		internal bool IsStageOptional
		{
			get
			{
				return this.finalAction == StageRetryDetails.FinalAction.SkipStage;
			}
		}

		internal bool TimeToTry
		{
			get
			{
				if (this.stageNeverRan)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "StageRetryDetails : stageneverRan = true", new object[0]);
					return true;
				}
				ExDateTime exDateTime = this.lastRunTime + this.retryInterval;
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "StageRetryDetails : this.lastRunTime = {0}, this.retryInterval = {1}, temp ={2}, now = {3}", new object[]
				{
					this.lastRunTime,
					this.retryInterval,
					exDateTime,
					ExDateTime.UtcNow
				});
				return ExDateTime.UtcNow >= exDateTime;
			}
		}

		internal bool IsTimeToGiveUp
		{
			get
			{
				if (TimeSpan.Zero.Equals(this.allowedTimeForRetries))
				{
					return true;
				}
				bool result = ExDateTime.UtcNow >= this.stageEnteredTime + this.allowedTimeForRetries;
				FaultInjectionUtils.FaultInjectChangeValue<bool>(2162568509U, ref result);
				return result;
			}
		}

		internal TimeSpan TotalDelayDueToThisStage
		{
			get
			{
				return ExDateTime.UtcNow - this.stageEnteredTime;
			}
		}

		internal void UpgdateStageRunTimestamp()
		{
			this.stageNeverRan = false;
			this.lastRunTime = ExDateTime.UtcNow;
		}

		private StageRetryDetails.FinalAction finalAction;

		private TimeSpan retryInterval;

		private TimeSpan allowedTimeForRetries;

		private ExDateTime stageEnteredTime;

		private ExDateTime lastRunTime;

		private bool stageNeverRan = true;

		internal enum FinalAction
		{
			DropMessage,
			SkipStage
		}
	}
}
