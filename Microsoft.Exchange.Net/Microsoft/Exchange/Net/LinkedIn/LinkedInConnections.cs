using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInConnections : IExtensibleDataObject
	{
		[DataMember(Name = "values")]
		public List<LinkedInPerson> People { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
