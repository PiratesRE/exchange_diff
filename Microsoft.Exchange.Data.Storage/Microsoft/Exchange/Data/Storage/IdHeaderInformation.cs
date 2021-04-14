using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IdHeaderInformation
	{
		public IdHeaderInformation()
		{
			this.mailboxId = new MailboxId(null);
		}

		public byte[] StoreIdBytes
		{
			get
			{
				return this.storeIdBytes;
			}
			set
			{
				this.storeIdBytes = value;
			}
		}

		public byte[] FolderIdBytes
		{
			get
			{
				return this.folderIdBytes;
			}
			set
			{
				this.folderIdBytes = value;
			}
		}

		public MailboxId MailboxId
		{
			get
			{
				return this.mailboxId;
			}
			set
			{
				this.mailboxId = value;
			}
		}

		public IdProcessingInstruction IdProcessingInstruction
		{
			get
			{
				return this.idProcessingInstruction;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<IdProcessingInstruction>(value, "value");
				this.idProcessingInstruction = value;
			}
		}

		public int OccurrenceInstanceIndex
		{
			get
			{
				return this.occurrenceInstanceIndex;
			}
			set
			{
				this.occurrenceInstanceIndex = value;
			}
		}

		public IdStorageType IdStorageType
		{
			get
			{
				return this.idStorageType;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<IdStorageType>(value, "value");
				this.idStorageType = value;
			}
		}

		public StoreObjectId ToStoreObjectId()
		{
			switch (this.IdProcessingInstruction)
			{
			case IdProcessingInstruction.Normal:
				return StoreObjectId.FromProviderSpecificId(this.StoreIdBytes);
			case IdProcessingInstruction.Series:
				return StoreObjectId.FromProviderSpecificId(this.StoreIdBytes, StoreObjectType.CalendarItemSeries);
			}
			return StoreObjectId.Deserialize(this.StoreIdBytes);
		}

		private IdStorageType idStorageType;

		private IdProcessingInstruction idProcessingInstruction;

		private byte[] storeIdBytes;

		private byte[] folderIdBytes;

		private MailboxId mailboxId;

		private int occurrenceInstanceIndex;
	}
}
