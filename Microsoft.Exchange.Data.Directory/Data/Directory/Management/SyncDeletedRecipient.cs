using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class SyncDeletedRecipient : ADPresentationObject
	{
		public SyncDeletedRecipient()
		{
		}

		public SyncDeletedRecipient(DeletedRecipient deleteRecipient) : base(deleteRecipient)
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncDeletedRecipient.schema;
			}
		}

		private new bool IsValid
		{
			get
			{
				return base.IsValid;
			}
		}

		private new string OriginatingServer
		{
			get
			{
				return base.OriginatingServer;
			}
		}

		private new ADObjectId ObjectCategory
		{
			get
			{
				return base.ObjectCategory;
			}
		}

		private new ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return base.ExchangeVersion;
			}
		}

		public bool EndOfList
		{
			get
			{
				return (bool)this[SyncDeletedObjectSchema.EndOfList];
			}
			internal set
			{
				this[SyncDeletedObjectSchema.EndOfList] = value;
			}
		}

		public byte[] Cookie
		{
			get
			{
				return (byte[])this[SyncDeletedObjectSchema.Cookie];
			}
			internal set
			{
				this[SyncDeletedObjectSchema.Cookie] = value;
			}
		}

		private static SyncDeletedObjectSchema schema = ObjectSchema.GetInstance<SyncDeletedObjectSchema>();
	}
}
