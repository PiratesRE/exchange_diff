using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarActionFolderIdResponse : CalendarActionItemIdResponse
	{
		public CalendarActionFolderIdResponse(CalendarActionError errorCode) : base(errorCode)
		{
		}

		public CalendarActionFolderIdResponse(FolderId folderId, ItemId calendarEntryId) : base(calendarEntryId)
		{
			this.NewFolderId = folderId;
		}

		[DataMember]
		public FolderId NewFolderId { get; set; }
	}
}
