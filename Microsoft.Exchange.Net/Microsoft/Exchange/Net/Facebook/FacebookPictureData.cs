using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal sealed class FacebookPictureData : IExtensibleDataObject
	{
		[DataMember(Name = "url")]
		public string Url { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
