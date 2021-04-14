using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarGroup
	{
		[DataMember]
		public ItemId ItemId { get; set; }

		[DataMember]
		public string GroupId { get; set; }

		[DataMember]
		public string GroupName { get; set; }

		[DataMember]
		public CalendarGroupType GroupType { get; set; }

		[DataMember]
		public CalendarEntry[] Calendars { get; set; }
	}
}
