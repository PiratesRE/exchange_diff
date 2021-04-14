using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolFilter
{
	internal static class Util
	{
		public static class PerformanceCounters
		{
			public static class RecipientFilter
			{
				public static void RecipientRejectedByRecipientValidation()
				{
					RecipientFilterPerfCounters.TotalRecipientsRejectedByRecipientValidation.Increment();
				}

				public static void RecipientRejectedByBlockList()
				{
					RecipientFilterPerfCounters.TotalRecipientsRejectedByBlockList.Increment();
				}

				public static void RemoveCounters()
				{
					RecipientFilterPerfCounters.TotalRecipientsRejectedByRecipientValidation.RawValue = 0L;
					RecipientFilterPerfCounters.TotalRecipientsRejectedByBlockList.RawValue = 0L;
				}
			}

			public static class SenderFilter
			{
				public static void MessageEvaluatedBySenderFilter()
				{
					SenderFilterPerfCounters.TotalMessagesEvaluatedBySenderFilter.Increment();
				}

				public static void MessageFilteredBySenderFilter()
				{
					SenderFilterPerfCounters.TotalMessagesFilteredBySenderFilter.Increment();
				}

				public static void SenderBlockedDueToPerRecipientBlockedSender()
				{
					SenderFilterPerfCounters.TotalPerRecipientSenderBlocks.Increment();
				}

				public static void RemoveCounters()
				{
					SenderFilterPerfCounters.TotalMessagesEvaluatedBySenderFilter.RawValue = 0L;
					SenderFilterPerfCounters.TotalMessagesFilteredBySenderFilter.RawValue = 0L;
					SenderFilterPerfCounters.TotalPerRecipientSenderBlocks.RawValue = 0L;
				}
			}
		}
	}
}
