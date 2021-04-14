using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class PublicFolderItemStatistics : XsoMailboxConfigurationObject
	{
		internal static XsoMailboxConfigurationObjectSchema StaticSchema
		{
			get
			{
				return PublicFolderItemStatistics.schema;
			}
		}

		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return PublicFolderItemStatistics.schema;
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[MailMessageSchema.Subject];
			}
		}

		public string PublicFolderName
		{
			get
			{
				return this.publicFolderName;
			}
			internal set
			{
				this.publicFolderName = value;
			}
		}

		public DateTime? LastModificationTime
		{
			get
			{
				return (DateTime?)((ExDateTime?)this[PublicFolderItemStatisticsSchema.LastModifiedTime]);
			}
		}

		public DateTime? CreationTime
		{
			get
			{
				return (DateTime?)((ExDateTime?)this[PublicFolderItemStatisticsSchema.CreationTime]);
			}
		}

		public bool HasAttachments
		{
			get
			{
				return (bool)this[PublicFolderItemStatisticsSchema.HasAttachment];
			}
		}

		public string ItemType
		{
			get
			{
				return (string)this[PublicFolderItemStatisticsSchema.ItemClass];
			}
		}

		public Unlimited<ByteQuantifiedSize> MessageSize
		{
			get
			{
				int num = (int)this[PublicFolderItemStatisticsSchema.Size];
				return ByteQuantifiedSize.FromBytes(checked((ulong)num));
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (MailboxStoreObjectId)this[MailMessageSchema.Identity];
			}
		}

		private static PublicFolderItemStatisticsSchema schema = new PublicFolderItemStatisticsSchema();

		private string publicFolderName;
	}
}
