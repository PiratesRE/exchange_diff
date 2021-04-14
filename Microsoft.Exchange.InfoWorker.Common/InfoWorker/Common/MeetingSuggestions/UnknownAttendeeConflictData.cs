using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class UnknownAttendeeConflictData : AttendeeConflictData
	{
		internal static UnknownAttendeeConflictData Create()
		{
			return new UnknownAttendeeConflictData();
		}
	}
}
