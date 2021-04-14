using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class FacebookEducationHistoryEntry : IExtensibleDataObject
	{
		[DataMember(Name = "school")]
		public FacebookSchool School { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
