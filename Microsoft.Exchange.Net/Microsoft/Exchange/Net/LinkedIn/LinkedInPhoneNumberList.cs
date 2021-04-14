using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInPhoneNumberList : IExtensibleDataObject
	{
		[DataMember(Name = "values")]
		public List<LinkedInPhoneNumber> Numbers { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
