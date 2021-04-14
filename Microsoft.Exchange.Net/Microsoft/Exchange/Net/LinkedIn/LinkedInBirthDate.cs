using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInBirthDate : IExtensibleDataObject
	{
		[DataMember(Name = "day")]
		public int Day { get; set; }

		[DataMember(Name = "month")]
		public int Month { get; set; }

		[DataMember(Name = "year")]
		public int Year { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
