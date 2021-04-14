using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInSchool : IExtensibleDataObject
	{
		[DataMember(Name = "schoolName")]
		public string SchoolName { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
