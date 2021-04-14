using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel.DataContracts
{
	[DataContract]
	public class CallerIdItem
	{
		public CallerIdItem(CallerIdItem taskCallerIdItem)
		{
			this.CallerIdType = (int)taskCallerIdItem.CallerIdType;
			this.GALContactLegacyDN = taskCallerIdItem.GALContactLegacyDN;
			this.PersonalContactStoreId = taskCallerIdItem.PersonalContactStoreId;
			this.PhoneNumber = taskCallerIdItem.PhoneNumber;
			this.DisplayName = taskCallerIdItem.DisplayName;
			this.PersonaEmailAddress = taskCallerIdItem.PersonaEmailAddress;
		}

		[DataMember]
		public int CallerIdType { get; set; }

		[DataMember]
		public string GALContactLegacyDN { get; set; }

		[DataMember]
		public string PersonalContactStoreId { get; set; }

		[DataMember]
		public string PhoneNumber { get; set; }

		[DataMember]
		public string PersonaEmailAddress { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		public CallerIdItem ToTaskObject()
		{
			return new CallerIdItem((CallerIdItemType)this.CallerIdType, this.PhoneNumber, this.GALContactLegacyDN, this.PersonalContactStoreId, this.PersonaEmailAddress, this.DisplayName);
		}
	}
}
