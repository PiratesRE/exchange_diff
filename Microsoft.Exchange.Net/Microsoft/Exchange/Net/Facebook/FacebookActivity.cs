using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class FacebookActivity : IExtensibleDataObject
	{
		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "category")]
		public string Category { get; set; }

		[DataMember(Name = "created_time")]
		public string CreatedTime { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
