using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInPhoneNumber : IExtensibleDataObject
	{
		[DataMember(Name = "phoneNumber")]
		public string Number { get; set; }

		[DataMember(Name = "phoneType")]
		public string Type { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
