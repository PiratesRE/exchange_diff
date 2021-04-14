using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FacebookUsersList
	{
		[DataMember(Name = "data")]
		public List<FacebookUser> Users { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
