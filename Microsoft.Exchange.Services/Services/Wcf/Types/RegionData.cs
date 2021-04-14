using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RegionData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string RegionId { get; set; }

		[DataMember]
		public string RegionName { get; set; }

		[DataMember]
		public string CountryCode { get; set; }

		[DataMember]
		public string[] CarrierIds { get; set; }
	}
}
