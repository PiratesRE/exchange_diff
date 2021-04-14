using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarActionGroupIdResponse : CalendarActionResponse
	{
		public CalendarActionGroupIdResponse(CalendarActionError errorCode) : base(errorCode)
		{
		}

		public CalendarActionGroupIdResponse(Guid groupClassId, ItemId groupItemId)
		{
			this.NewGroupClassId = groupClassId.ToString();
			this.NewGroupItemId = groupItemId;
		}

		[DataMember]
		public string NewGroupClassId { get; set; }

		[DataMember]
		public ItemId NewGroupItemId { get; set; }
	}
}
