using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarSharingRecipientInfo
	{
		[DataMember]
		public EmailAddressWrapper EmailAddress { get; set; }

		[DataMember]
		public bool IsInsideOrganization { get; set; }

		[DataMember]
		public string CurrentDetailLevel { get; set; }

		[DataMember]
		public string HandlerType { get; set; }

		[DataMember]
		public string[] AllowedDetailLevels { get; set; }
	}
}
