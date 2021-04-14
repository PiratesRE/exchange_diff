using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CountryData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string LocalizedDisplayName { get; set; }
	}
}
