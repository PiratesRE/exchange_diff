using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract(Name = "AccountToMigrate")]
	internal class MigrationAccount
	{
		[DataMember(Name = "clusterName")]
		public string ClusterName { get; set; }

		[DataMember(Name = "dgroupId")]
		public uint DgroupId { get; set; }

		[DataMember(Name = "puid")]
		public long Puid { get; set; }

		[DataMember(Name = "login")]
		public string Login { get; set; }

		[DataMember(Name = "firstName")]
		public string FirstName { get; set; }

		[DataMember(Name = "lastName")]
		public string LastName { get; set; }

		[DataMember(Name = "accountSize")]
		public long AccountSize { get; set; }

		[DataMember(Name = "timeZone")]
		public string TimeZone { get; set; }

		[DataMember(Name = "lcid")]
		public string Lcid { get; set; }

		[DataMember(Name = "leaseInitialExpiry")]
		public DateTime LeaseInitialExpiry { get; set; }

		[DataMember(Name = "aliases")]
		public string[] Aliases { get; set; }
	}
}
