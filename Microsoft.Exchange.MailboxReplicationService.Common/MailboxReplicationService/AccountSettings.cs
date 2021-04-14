using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class AccountSettings : ItemPropertiesBase
	{
		[DataMember]
		public string PrimaryLogin { get; set; }

		[DataMember]
		public DateTime LastLoginTime { get; set; }

		[DataMember]
		public int AccountStatusInt { get; set; }

		public OlcAccountStatus AccountStatus
		{
			get
			{
				return (OlcAccountStatus)this.AccountStatusInt;
			}
			set
			{
				this.AccountStatusInt = (int)value;
			}
		}

		[DataMember]
		public ulong OimTrxSize { get; set; }

		[DataMember]
		public ulong OimNonTrxSize { get; set; }

		[DataMember]
		public DateTime? RegistrationDate { get; set; }

		[DataMember]
		public ushort UserClassCode { get; set; }

		[DataMember]
		public string DemoCode { get; set; }

		[DataMember]
		public int DatFlagsInt { get; set; }

		[DataMember]
		public int DatFlags2Int { get; set; }

		[DataMember]
		public string[] FeatureSetList { get; set; }

		[DataMember]
		public string[] HMFeatureSetList { get; set; }

		[DataMember]
		public ulong StorageSpaceUsageLimit { get; set; }

		[DataMember]
		public DateTime? StorageSpaceUsageLimitIncremented { get; set; }

		[DataMember]
		public Alias[] Aliases { get; set; }
	}
}
