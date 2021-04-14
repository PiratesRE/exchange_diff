using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MailboxInformation
	{
		[DataMember(IsRequired = true)]
		public Guid MailboxGuid { get; set; }

		[DataMember(IsRequired = true)]
		public ulong MailboxSize { get; set; }

		[DataMember(IsRequired = true)]
		public ulong MailboxItemCount { get; set; }

		[DataMember(IsRequired = true)]
		public Guid MdbGuid { get; set; }

		[DataMember(IsRequired = true)]
		public string MdbName { get; set; }

		[DataMember]
		public string MdbLegDN { get; set; }

		[DataMember(IsRequired = true)]
		public int ServerVersion { get; set; }

		[DataMember]
		public string ServerMailboxRelease { get; set; }

		[DataMember]
		public Guid MailboxHomeMdbGuid { get; set; }

		[DataMember]
		public int RulesSize { get; set; }

		[DataMember]
		public int RecipientType { get; set; }

		[DataMember]
		public int RecipientTypeDetails { get; set; }

		[DataMember]
		public long RecipientTypeDetailsLong { get; set; }

		[DataMember]
		public int RecipientDisplayType { get; set; }

		[DataMember]
		public Guid ArchiveGuid { get; set; }

		[DataMember]
		public Guid[] AlternateMailboxes { get; set; }

		[DataMember]
		public string MailboxIdentity { get; set; }

		[DataMember]
		public bool? UseMdbQuotaDefaults { get; set; }

		[DataMember]
		public ulong? MailboxQuota { get; set; }

		[DataMember]
		public ulong? MailboxDumpsterQuota { get; set; }

		[DataMember]
		public ulong? MailboxArchiveQuota { get; set; }

		[DataMember]
		public ulong? MdbQuota { get; set; }

		[DataMember]
		public ulong? MdbDumpsterQuota { get; set; }

		[DataMember]
		public string UserDataXML { get; set; }

		[DataMember]
		public ulong RegularItemCount { get; set; }

		[DataMember]
		public ulong RegularDeletedItemCount { get; set; }

		[DataMember]
		public ulong AssociatedItemCount { get; set; }

		[DataMember]
		public ulong AssociatedDeletedItemCount { get; set; }

		[DataMember]
		public ulong RegularItemsSize { get; set; }

		[DataMember]
		public ulong RegularDeletedItemsSize { get; set; }

		[DataMember]
		public ulong AssociatedItemsSize { get; set; }

		[DataMember]
		public ulong AssociatedDeletedItemsSize { get; set; }

		[DataMember]
		public MailboxServerInformation ServerInformation { get; set; }

		[DataMember(IsRequired = false)]
		public string ProviderName { get; set; }

		[DataMember(IsRequired = false)]
		public int MailboxTableFlags { get; set; }

		[DataMember(IsRequired = false)]
		public int ContentAggregationFlags { get; set; }

		[DataMember(IsRequired = false)]
		public Guid[] ContainerMailboxGuids { get; set; }

		[DataMember(IsRequired = false)]
		public float MrsVersion { get; set; }

		public override string ToString()
		{
			string format = "mbxGuid={0}\nmbxName='{1}\nmbxHomeMdbGuid={19}\nmbxSize={2} (deleted={3})\nmbxItemCount={4}\nrecipientType={5} {6} {7}\narchiveGuid={8}\naltMailboxes={9}\nmbxQuota={10}\nmbxDumpsterQuota={11}\nmbxArchiveQuota={12}\nmdbGuid={13}\nmdbName='{14}'\nserverVersion={15}\nmdbQuota={16}\nmdbDumpsterQuota={17}\nrulesSize={18}\nserverInfo={20}\n";
			string text = (this.UseMdbQuotaDefaults == true) ? "(use MDB default)" : ((this.MailboxQuota == null) ? "(unlimited)" : new ByteQuantifiedSize(this.MailboxQuota.Value).ToString());
			string text2 = (this.UseMdbQuotaDefaults == true) ? "(use MDB default)" : ((this.MailboxDumpsterQuota == null) ? "(unlimited)" : new ByteQuantifiedSize(this.MailboxDumpsterQuota.Value).ToString());
			string text3 = (this.UseMdbQuotaDefaults == true) ? "(use MDB default)" : ((this.MailboxArchiveQuota == null) ? "(unlimited)" : new ByteQuantifiedSize(this.MailboxArchiveQuota.Value).ToString());
			return string.Format(format, new object[]
			{
				this.MailboxGuid,
				this.MailboxIdentity,
				new ByteQuantifiedSize(this.MailboxSize).ToString(),
				new ByteQuantifiedSize(this.RegularDeletedItemsSize).ToString(),
				this.MailboxItemCount,
				(RecipientType)this.RecipientType,
				(RecipientTypeDetails)this.RecipientTypeDetails,
				(RecipientDisplayType)this.RecipientDisplayType,
				this.ArchiveGuid.ToString(),
				(this.AlternateMailboxes != null && this.AlternateMailboxes.Length > 0) ? "(present)" : "(not present)",
				text,
				text2,
				text3,
				this.MdbGuid,
				this.MdbName,
				new ServerVersion(this.ServerVersion).ToString(),
				(this.MdbQuota == null) ? "(unlimited)" : new ByteQuantifiedSize(this.MdbQuota.Value).ToString(),
				(this.MdbDumpsterQuota == null) ? "(unlimited)" : new ByteQuantifiedSize(this.MdbDumpsterQuota.Value).ToString(),
				this.RulesSize,
				this.MailboxHomeMdbGuid,
				this.ServerInformation
			});
		}

		public LocalizedString GetItemCountsAndSizesString()
		{
			return MrsStrings.ItemCountsAndSizes(this.RegularItemCount, new ByteQuantifiedSize(this.RegularItemsSize).ToString(), this.RegularDeletedItemCount, new ByteQuantifiedSize(this.RegularDeletedItemsSize).ToString(), this.AssociatedItemCount, new ByteQuantifiedSize(this.AssociatedItemsSize).ToString(), this.AssociatedDeletedItemCount, new ByteQuantifiedSize(this.AssociatedDeletedItemsSize).ToString());
		}
	}
}
