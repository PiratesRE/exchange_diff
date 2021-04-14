using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(CombinedScenarioRecoResult))]
	[KnownType(typeof(RowNotificationPayload))]
	[KnownType(typeof(HierarchyNotificationPayload))]
	[KnownType(typeof(JsonFaultResponse))]
	[KnownType(typeof(CreateAttachmentNotificationPayload))]
	[KnownType(typeof(DayTimeDurationRecoResult))]
	[KnownType(typeof(CalendarItemNotificationPayload))]
	[KnownType(typeof(InstantMessagePayload))]
	[KnownType(typeof(NewMailNotificationPayload))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(FilteredViewHierarchyNotificationPayload))]
	[KnownType(typeof(SearchNotificationPayload))]
	[KnownType(typeof(InstantSearchNotificationPayload))]
	[KnownType(typeof(PeopleIKnowRowNotificationPayload))]
	[KnownType(typeof(AttachmentOperationCorrelationIdNotificationPayload))]
	public class SubscriptionData
	{
		[DataMember]
		public string SubscriptionId { get; set; }

		[DataMember]
		public SubscriptionParameters Parameters { get; set; }
	}
}
