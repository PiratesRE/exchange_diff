using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInIMAccount : IExtensibleDataObject
	{
		[DataMember(Name = "im-account-type")]
		public string IMAccountType { get; set; }

		[DataMember(Name = "im-account-name")]
		public string IMAccountName { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
