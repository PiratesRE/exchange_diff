using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public enum SubscribeToNotificationMetadata
	{
		[DisplayName("SN", "UCL")]
		UserContextLatency,
		[DisplayName("SN", "C")]
		SubscriptionCount,
		[DisplayName("SN", "TZ")]
		RequestTimeZone,
		[DisplayName("SN", "HieL")]
		HierarchyNotificationLatency,
		[DisplayName("SN", "RemL")]
		ReminderNotificationLatency,
		[DisplayName("SN", "PoPL")]
		PlayOnPhoneNotificationLatency,
		[DisplayName("SN", "RowL")]
		RowNotificationLatency,
		[DisplayName("SN", "CalL")]
		CalendarItemNotificationLatency,
		[DisplayName("SN", "PIKL")]
		PeopleIKnowNotificationLatency,
		[DisplayName("SN", "IML")]
		InstantMessageNotificationLatency,
		[DisplayName("SN", "NML")]
		NewMailNotificationLatency,
		[DisplayName("SN", "UINL")]
		UnseenItemNotificationLatency,
		[DisplayName("SN", "GAL")]
		GroupAssociationNotificationLatency,
		[DisplayName("SN", "OL")]
		Other
	}
}
