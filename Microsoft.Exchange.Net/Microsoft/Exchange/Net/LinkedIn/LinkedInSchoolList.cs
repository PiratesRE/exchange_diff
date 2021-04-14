using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInSchoolList : IExtensibleDataObject
	{
		[DataMember(Name = "values")]
		public List<LinkedInSchool> Schools { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
