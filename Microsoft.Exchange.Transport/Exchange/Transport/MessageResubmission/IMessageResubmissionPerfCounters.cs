using System;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.MessageResubmission
{
	internal interface IMessageResubmissionPerfCounters
	{
		void ResetCounters();

		void UpdateResubmissionCount(int count, bool isShadowResubmit);

		ITimerCounter ResubmitMessagesLatencyCounter();

		void UpdateResubmitRequestCount(ResubmitRequestState state, int changeAmount);

		void ChangeResubmitRequestState(ResubmitRequestState oldState, ResubmitRequestState newState);

		void IncrementRecentRequestCount(bool isShadowResubmit);

		void RecordResubmitRequestTimeSpan(TimeSpan timeSpan);
	}
}
