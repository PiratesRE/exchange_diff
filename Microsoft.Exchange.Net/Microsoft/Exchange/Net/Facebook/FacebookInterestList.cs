using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class FacebookInterestList
	{
		[DataMember(Name = "data")]
		public List<FacebookInterest> Interests { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
