using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInIMAccounts : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData { get; set; }

		[DataMember(Name = "values")]
		public List<LinkedInIMAccount> Accounts;
	}
}
