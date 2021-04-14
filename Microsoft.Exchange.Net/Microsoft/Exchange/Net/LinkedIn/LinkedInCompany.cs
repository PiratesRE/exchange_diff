using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInCompany : IExtensibleDataObject
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
