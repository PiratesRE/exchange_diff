using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FacebookActivityList
	{
		[DataMember(Name = "data")]
		public List<FacebookActivity> Activities { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
