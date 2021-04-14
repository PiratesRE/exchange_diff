using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal sealed class FacebookPicture : IExtensibleDataObject
	{
		[DataMember(Name = "data")]
		public FacebookPictureData Data { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
