using System;
using System.Threading;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CurrentCallsCounterHelper
	{
		private CurrentCallsCounterHelper()
		{
		}

		internal static CurrentCallsCounterHelper Instance
		{
			get
			{
				return CurrentCallsCounterHelper.instance;
			}
		}

		internal void Init()
		{
			this.currentCallsCounterTimer = new Timer(new TimerCallback(this.UpdateCurrentCallsCounters), null, Constants.UpdateCurrentCallsTimerInterval, Constants.UpdateCurrentCallsTimerInterval);
		}

		internal void Shutdown()
		{
			if (this.currentCallsCounterTimer != null)
			{
				this.currentCallsCounterTimer.Dispose();
				this.currentCallsCounterTimer = null;
			}
			this.UpdateCurrentCallsCounters(null);
		}

		internal void AnalyzeCall(BaseUMCallSession callSession)
		{
			CallContext currentCallContext = callSession.CurrentCallContext;
			if (currentCallContext == null)
			{
				return;
			}
			CallType callType = currentCallContext.CallType;
			if (callType != null && callType != 7)
			{
				this.currentCalls += 1L;
			}
			switch (callType)
			{
			case 1:
				this.currentUnauthenticatedPilotNumberCalls += 1L;
				break;
			case 2:
				this.currentAutoAttendantCalls += 1L;
				return;
			case 3:
				this.currentSubscriberAccessCalls += 1L;
				return;
			case 4:
				this.currentVoiceCalls += 1L;
				return;
			case 5:
				this.currentPlayOnPhoneCalls += 1L;
				return;
			case 6:
				this.currentFaxCalls += 1L;
				return;
			case 7:
			case 9:
				break;
			case 8:
				this.currentPromptEditingCalls += 1L;
				return;
			case 10:
				this.currentPlayOnPhoneCalls += 1L;
				return;
			default:
				return;
			}
		}

		internal void UpdatePerformanceCounters()
		{
			Util.SetCounter(GeneralCounters.CurrentCalls, this.currentCalls);
			Util.SetCounter(GeneralCounters.CurrentFaxCalls, this.currentFaxCalls);
			Util.SetCounter(GeneralCounters.CurrentVoicemailCalls, this.currentVoiceCalls);
			Util.SetCounter(GeneralCounters.CurrentPlayOnPhoneCalls, this.currentPlayOnPhoneCalls);
			Util.SetCounter(GeneralCounters.CurrentAutoAttendantCalls, this.currentAutoAttendantCalls);
			Util.SetCounter(GeneralCounters.CurrentPromptEditingCalls, this.currentPromptEditingCalls);
			Util.SetCounter(GeneralCounters.CurrentSubscriberAccessCalls, this.currentSubscriberAccessCalls);
			Util.SetCounter(GeneralCounters.CurrentUnauthenticatedPilotNumberCalls, this.currentUnauthenticatedPilotNumberCalls);
		}

		internal void Reset()
		{
			this.currentCalls = 0L;
			this.currentFaxCalls = 0L;
			this.currentVoiceCalls = 0L;
			this.currentPlayOnPhoneCalls = 0L;
			this.currentAutoAttendantCalls = 0L;
			this.currentPromptEditingCalls = 0L;
			this.currentSubscriberAccessCalls = 0L;
			this.currentUnauthenticatedPilotNumberCalls = 0L;
		}

		private void UpdateCurrentCallsCounters(object state)
		{
			if (UmServiceGlobals.ArePerfCountersEnabled)
			{
				lock (UmServiceGlobals.VoipPlatform.CallSessionHashTable.SyncRoot)
				{
					this.Reset();
					foreach (object obj in UmServiceGlobals.VoipPlatform.CallSessionHashTable.Values)
					{
						BaseUMCallSession callSession = (BaseUMCallSession)obj;
						this.AnalyzeCall(callSession);
					}
					this.UpdatePerformanceCounters();
				}
			}
		}

		private static CurrentCallsCounterHelper instance = new CurrentCallsCounterHelper();

		private Timer currentCallsCounterTimer;

		private long currentCalls;

		private long currentFaxCalls;

		private long currentVoiceCalls;

		private long currentPlayOnPhoneCalls;

		private long currentAutoAttendantCalls;

		private long currentPromptEditingCalls;

		private long currentSubscriberAccessCalls;

		private long currentUnauthenticatedPilotNumberCalls;
	}
}
