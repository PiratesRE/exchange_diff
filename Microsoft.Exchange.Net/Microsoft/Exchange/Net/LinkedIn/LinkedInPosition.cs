using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInPosition : IExtensibleDataObject
	{
		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "company")]
		public LinkedInCompany Company { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
