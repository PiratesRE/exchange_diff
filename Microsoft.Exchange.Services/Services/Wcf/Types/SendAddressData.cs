using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SendAddressData : OptionsPropertyChangeTracker
	{
		[DataMember(IsRequired = true)]
		public string DisplayName { get; set; }

		[DataMember]
		public string AddressId { get; set; }
	}
}
