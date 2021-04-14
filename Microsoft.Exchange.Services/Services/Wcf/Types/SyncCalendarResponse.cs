using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class SyncCalendarResponse
	{
		[DataMember]
		public string SyncState { get; set; }

		[DataMember]
		public bool IncludesLastItemInRange { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ItemId[] DeletedItems { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EwsCalendarItemType[] UpdatedItems { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EwsCalendarItemType[] RecurrenceMastersWithInstances { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EwsCalendarItemType[] RecurrenceMastersWithoutInstances { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EwsCalendarItemType[] UnchangedRecurrenceMastersWithInstances { get; set; }
	}
}
