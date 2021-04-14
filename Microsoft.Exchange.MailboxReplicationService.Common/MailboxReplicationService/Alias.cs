using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class Alias
	{
		[DataMember]
		public string MservKeyName { get; set; }

		[DataMember]
		public DateTime? Created { get; set; }

		[DataMember]
		public DateTime LastWrite { get; set; }

		[DataMember]
		public bool ManagedShouldExistInMserv { get; set; }
	}
}
