using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class FacebookWorkHistoryEntry : IExtensibleDataObject
	{
		[DataMember(Name = "employer")]
		public FacebookEmployer Employer { get; set; }

		[DataMember(Name = "position")]
		public FacebookWorkPosition Position { get; set; }

		[DataMember(Name = "start_date")]
		public string StartDate { get; set; }

		[DataMember(Name = "end_date")]
		public string EndDate { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
