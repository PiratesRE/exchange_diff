using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Security
{
	[CLSCompliant(false)]
	[DataContract]
	public class RPSProfile
	{
		[DataMember]
		public string HexPuid { get; set; }

		[DataMember]
		public string MemberName { get; set; }

		[DataMember]
		public int TokenFlags { get; set; }

		[DataMember]
		public uint AuthFlags { get; set; }

		[DataMember]
		public uint CredsExpireIn { get; set; }

		[DataMember]
		public string RecoveryUrl { get; set; }

		[DataMember]
		public string ConsumerPuid { get; set; }

		[DataMember]
		public bool AppPassword { get; set; }

		[DataMember]
		public bool HasSignedTOU { get; set; }

		[DataMember]
		public uint AuthInstant { get; set; }

		[DataMember]
		public uint IssueInstant { get; set; }

		[DataMember]
		public byte ReputationByte { get; set; }

		[DataMember]
		public string HexCID { get; set; }

		[DataMember]
		public uint LoginAttributes { get; set; }

		[DataMember]
		public string ResponseHeader { get; set; }

		[DataMember]
		public uint RPSAuthState { get; set; }

		[DataMember]
		public string ConsumerCID { get; set; }

		[DataMember]
		public uint TicketType { get; set; }

		[DataMember]
		public uint ConsumerChild { get; set; }

		[DataMember]
		public string ConsumerConsentLevel { get; set; }

		[DataMember]
		public RPSProfile.UserInfo Profile { get; set; }

		public string CurrentAlias { get; set; }

		[DataContract]
		public class UserInfo
		{
			[DataMember]
			public string Gender { get; set; }

			[DataMember]
			public string Occupation { get; set; }

			[DataMember]
			public int Region { get; set; }

			[DataMember]
			public short TimeZone { get; set; }

			[DataMember]
			public DateTime Birthday { get; set; }

			[DataMember]
			public uint AliasVersion { get; set; }

			[DataMember]
			public string PostalCode { get; set; }

			[DataMember]
			public string FirstName { get; set; }

			[DataMember]
			public string LastName { get; set; }

			[DataMember]
			public short Language { get; set; }

			[DataMember]
			public string Country { get; set; }
		}
	}
}
