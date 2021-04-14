using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	[XmlType(TypeName = "MeetingAttendeeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum MeetingAttendeeType
	{
		Organizer,
		Required,
		Optional,
		Room,
		Resource
	}
}
