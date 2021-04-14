using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FacebookWorkPosition : IExtensibleDataObject
	{
		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
