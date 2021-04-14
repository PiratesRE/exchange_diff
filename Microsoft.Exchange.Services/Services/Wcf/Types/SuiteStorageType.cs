using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SuiteStorageType
	{
		[DataMember]
		public SuiteStorageKeyType Key { get; set; }

		[DataMember]
		public string Value { get; set; }

		public const string ConfigurationName = "Suite.Storage";

		public const string UserMailboxStorage = "User";

		public const string OrgMailboxStorage = "Org";
	}
}
