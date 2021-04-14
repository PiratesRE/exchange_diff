using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AutoAttendantCountersUtil : DirectoryAccessCountersUtil
	{
		internal AutoAttendantCountersUtil(BaseUMCallSession vo) : base(vo)
		{
			if (CommonConstants.UseDataCenterLogging)
			{
				this.instanceName = "DataCenterAutoAttendantPerfCounterName";
				return;
			}
			this.instanceName = base.Session.CurrentCallContext.AutoAttendantInfo.Name;
		}

		internal void ComputeSuccessRate()
		{
			long rawValue = this.GetInstance().TotalCalls.RawValue;
			long value = this.GetInstance().TransfersToSendMessage.RawValue + this.GetInstance().DisconnectedWithoutInput.RawValue + this.GetInstance().OperatorTransfersInitiatedByUserFromOpeningMenu.RawValue + this.GetInstance().SentToAutoAttendant.RawValue + this.GetInstance().TransferredCount.RawValue + this.GetInstance().AmbiguousNameTransfers.RawValue + this.GetInstance().DisallowedTransfers.RawValue;
			if (rawValue > 0L)
			{
				base.Session.SetCounter(this.GetInstance().PercentageSuccessfulCalls, value);
				base.Session.SetCounter(this.GetInstance().PercentageSuccessfulCalls_Base, rawValue);
				return;
			}
			base.Session.SetCounter(this.GetInstance().PercentageSuccessfulCalls, 0L);
			base.Session.SetCounter(this.GetInstance().PercentageSuccessfulCalls_Base, 1L);
		}

		internal AACountersInstance GetInstance()
		{
			return AACounters.GetInstance(this.instanceName);
		}

		internal void IncrementCallTypeCounters(bool isBusinessHourCall)
		{
			base.Session.IncrementCounter(this.GetInstance().TotalCalls);
			if (isBusinessHourCall)
			{
				base.Session.IncrementCounter(this.GetInstance().BusinessHoursCalls);
				return;
			}
			base.Session.IncrementCounter(this.GetInstance().OutOfHoursCalls);
		}

		internal void IncrementTransferCounters(TransferExtension ext)
		{
			switch (ext)
			{
			case TransferExtension.Operator:
				base.Session.IncrementCounter(this.GetInstance().OperatorTransfers);
				return;
			case TransferExtension.CustomMenuExtension:
				base.Session.IncrementCounter(this.GetInstance().TransferredCount);
				return;
			case TransferExtension.UserExtension:
				base.Session.IncrementCounter(this.GetInstance().TransferredCount);
				return;
			default:
				return;
			}
		}

		internal void IncrementTransferCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().TransferredCount);
		}

		internal void IncrementTransfersToSendMessageCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().TransfersToSendMessage);
		}

		internal void IncrementNameSpokenCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().NameSpoken);
		}

		internal void IncrementTransfersToDtmfFallbackAutoAttendantCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().TransfersToDtmfFallbackAutoAttendant);
		}

		internal void IncrementUserInitiatedTransferToOperatorCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().OperatorTransfersInitiatedByUser);
		}

		internal void IncrementUserInitiatedTransferToOperatorFromMainMenuCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().OperatorTransfersInitiatedByUserFromOpeningMenu);
			base.Session.IncrementCounter(this.GetInstance().OperatorTransfersInitiatedByUser);
		}

		internal void IncrementTransferToKeyMappingAutoAttendantCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().SentToAutoAttendant);
		}

		internal void IncrementCustomMenuCounters(CustomMenuKey option)
		{
			switch (option)
			{
			case CustomMenuKey.One:
				base.Session.IncrementCounter(this.GetInstance().MenuOption1);
				break;
			case CustomMenuKey.Two:
				base.Session.IncrementCounter(this.GetInstance().MenuOption2);
				break;
			case CustomMenuKey.Three:
				base.Session.IncrementCounter(this.GetInstance().MenuOption3);
				break;
			case CustomMenuKey.Four:
				base.Session.IncrementCounter(this.GetInstance().MenuOption4);
				break;
			case CustomMenuKey.Five:
				base.Session.IncrementCounter(this.GetInstance().MenuOption5);
				break;
			case CustomMenuKey.Six:
				base.Session.IncrementCounter(this.GetInstance().MenuOption6);
				break;
			case CustomMenuKey.Seven:
				base.Session.IncrementCounter(this.GetInstance().MenuOption7);
				break;
			case CustomMenuKey.Eight:
				base.Session.IncrementCounter(this.GetInstance().MenuOption8);
				break;
			case CustomMenuKey.Nine:
				base.Session.IncrementCounter(this.GetInstance().MenuOption9);
				break;
			case CustomMenuKey.Timeout:
				base.Session.IncrementCounter(this.GetInstance().MenuOptionTimeout);
				break;
			}
			base.Session.IncrementCounter(this.GetInstance().CustomMenuOptions);
		}

		internal void IncrementSpeechCallsCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().SpeechCalls);
		}

		internal void IncrementANRTransfersToOperatorCounter()
		{
			base.Session.IncrementCounter(this.GetInstance().AmbiguousNameTransfers);
		}

		internal void IncrementDisallowedTransferCalls()
		{
			base.Session.IncrementCounter(this.GetInstance().DisallowedTransfers);
		}

		internal void UpdateAverageTimeCounters(ExDateTime startTime)
		{
			TimeSpan timeSpan = ExDateTime.UtcNow - startTime;
			base.Session.SetCounter(this.GetInstance().AverageRecentCallTime, AutoAttendantCountersUtil.recentCallTimeAverage.Update((long)timeSpan.Seconds));
			base.Session.SetCounter(this.GetInstance().AverageCallTime, AutoAttendantCountersUtil.callTimeAverage.Update((long)timeSpan.Seconds));
		}

		protected override ExPerformanceCounter GetSingleCounter(DirectoryAccessCountersUtil.DirectoryAccessCounter perfcounter)
		{
			ExPerformanceCounter result;
			switch (perfcounter)
			{
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccess:
				result = this.GetInstance().DirectoryAccessed;
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedByExtension:
				result = this.GetInstance().DirectoryAccessedByExtension;
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedByDialByName:
				result = this.GetInstance().DirectoryAccessedByDialByName;
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedSuccessfullyByDialByName:
				result = this.GetInstance().DirectoryAccessedSuccessfullyByDialByName;
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedBySpokenName:
				result = this.GetInstance().DirectoryAccessedBySpokenName;
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedSuccessfullyBySpokenName:
				result = this.GetInstance().DirectoryAccessedSuccessfullyBySpokenName;
				break;
			default:
				throw new InvalidPerfCounterException(perfcounter.ToString());
			}
			return result;
		}

		protected override List<ExPerformanceCounter> GetCounters(DirectoryAccessCountersUtil.DirectoryAccessCounter counter)
		{
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			switch (counter)
			{
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccess:
				list.Add(this.GetInstance().DirectoryAccessed);
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedByExtension:
				list.Add(this.GetInstance().DirectoryAccessedByExtension);
				list.Add(this.GetInstance().DirectoryAccessed);
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedByDialByName:
				list.Add(this.GetInstance().DirectoryAccessedByDialByName);
				list.Add(this.GetInstance().DirectoryAccessed);
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedSuccessfullyByDialByName:
				list.Add(this.GetInstance().DirectoryAccessedSuccessfullyByDialByName);
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedBySpokenName:
				list.Add(this.GetInstance().DirectoryAccessedBySpokenName);
				list.Add(this.GetInstance().DirectoryAccessed);
				break;
			case DirectoryAccessCountersUtil.DirectoryAccessCounter.DirectoryAccessedSuccessfullyBySpokenName:
				list.Add(this.GetInstance().DirectoryAccessedSuccessfullyBySpokenName);
				break;
			default:
				throw new InvalidPerfCounterException(counter.ToString());
			}
			return list;
		}

		private static MovingAverage recentCallTimeAverage = new MovingAverage(50);

		private static Average callTimeAverage = new Average();

		private string instanceName;
	}
}
