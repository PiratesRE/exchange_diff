using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class LocalCalendarEntry : CalendarEntry
	{
		[DataMember]
		public bool IsInternetCalendar { get; set; }

		[DataMember]
		public bool IsDefaultCalendar { get; set; }

		[DataMember]
		public FolderId CalendarFolderId { get; set; }
	}
}
