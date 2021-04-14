using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class PublicFolderStatistics : XsoMailboxConfigurationObject
	{
		internal static PublicFolderStatisticsSchema InternalSchema
		{
			get
			{
				return PublicFolderStatistics.schema;
			}
		}

		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return PublicFolderStatistics.schema;
			}
		}

		public int? AssociatedItemCount
		{
			get
			{
				return (int?)this[PublicFolderStatisticsSchema.AssociatedItemCount];
			}
		}

		public uint ContactCount
		{
			get
			{
				return this.contactCount;
			}
			internal set
			{
				this.contactCount = value;
			}
		}

		public DateTime? CreationTime
		{
			get
			{
				return new DateTime?((DateTime)((ExDateTime)this[PublicFolderStatisticsSchema.CreationTime]));
			}
		}

		public uint DeletedItemCount
		{
			get
			{
				return this.deletedItemCount;
			}
			internal set
			{
				this.deletedItemCount = value;
			}
		}

		public string EntryId
		{
			get
			{
				StoreObjectId objectId = ((VersionedId)this[PublicFolderStatisticsSchema.EntryId]).ObjectId;
				return objectId.ToHexEntryId();
			}
		}

		public MapiFolderPath FolderPath
		{
			get
			{
				string text = (string)this[PublicFolderStatisticsSchema.FolderPath];
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					return new MapiFolderPath('\\' + text);
				}
				return new MapiFolderPath(text);
			}
		}

		public int? ItemCount
		{
			get
			{
				return (int?)this[PublicFolderStatisticsSchema.ItemCount];
			}
		}

		public DateTime? LastModificationTime
		{
			get
			{
				return (DateTime?)((ExDateTime?)this[PublicFolderStatisticsSchema.LastModificationTime]);
			}
		}

		public string Name
		{
			get
			{
				return (string)this[PublicFolderStatisticsSchema.Name];
			}
		}

		public uint OwnerCount
		{
			get
			{
				return this.ownerCount;
			}
			internal set
			{
				this.ownerCount = value;
			}
		}

		public ByteQuantifiedSize TotalAssociatedItemSize
		{
			get
			{
				return ByteQuantifiedSize.FromBytes(checked((ulong)((long)this[PublicFolderStatisticsSchema.TotalAssociatedItemSize])));
			}
		}

		public ByteQuantifiedSize TotalDeletedItemSize
		{
			get
			{
				return this.totalDeletedItemSize;
			}
			internal set
			{
				this.totalDeletedItemSize = value;
			}
		}

		public ByteQuantifiedSize TotalItemSize
		{
			get
			{
				return ByteQuantifiedSize.FromBytes(checked((ulong)((long)this[PublicFolderStatisticsSchema.TotalItemSize])));
			}
		}

		private static PublicFolderStatisticsSchema schema = ObjectSchema.GetInstance<PublicFolderStatisticsSchema>();

		private uint ownerCount;

		private uint contactCount;

		private uint deletedItemCount;

		private ByteQuantifiedSize totalDeletedItemSize;
	}
}
