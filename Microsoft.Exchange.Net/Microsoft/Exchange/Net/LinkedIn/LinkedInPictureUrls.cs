using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInPictureUrls : IExtensibleDataObject
	{
		[DataMember(Name = "values")]
		public List<string> Urls { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
