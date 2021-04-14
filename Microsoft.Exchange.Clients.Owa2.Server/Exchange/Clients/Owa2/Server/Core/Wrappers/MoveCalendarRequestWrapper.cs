using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MoveCalendarRequestWrapper
	{
		[DataMember(Name = "calendarToMove")]
		public FolderId CalendarToMove { get; set; }

		[DataMember(Name = "parentGroupId")]
		public string ParentGroupId { get; set; }

		[DataMember(Name = "calendarBefore")]
		public FolderId CalendarBefore { get; set; }
	}
}
