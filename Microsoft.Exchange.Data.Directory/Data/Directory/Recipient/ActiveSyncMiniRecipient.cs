using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class ActiveSyncMiniRecipient : StorageMiniRecipient
	{
		public ActiveSyncMiniRecipient()
		{
		}

		internal ActiveSyncMiniRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		public bool ActiveSyncEnabled
		{
			get
			{
				return (bool)(this[ActiveSyncMiniRecipientSchema.ActiveSyncEnabled] ?? false);
			}
		}

		public ADObjectId ActiveSyncMailboxPolicy
		{
			get
			{
				return (ADObjectId)this[ActiveSyncMiniRecipientSchema.ActiveSyncMailboxPolicy];
			}
		}

		public MultiValuedProperty<string> ActiveSyncAllowedDeviceIDs
		{
			get
			{
				return (MultiValuedProperty<string>)this[ActiveSyncMiniRecipientSchema.ActiveSyncAllowedDeviceIDs];
			}
		}

		public MultiValuedProperty<string> ActiveSyncBlockedDeviceIDs
		{
			get
			{
				return (MultiValuedProperty<string>)this[ActiveSyncMiniRecipientSchema.ActiveSyncBlockedDeviceIDs];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ActiveSyncMiniRecipient.schema;
			}
		}

		private static readonly ActiveSyncMiniRecipientSchema schema = ObjectSchema.GetInstance<ActiveSyncMiniRecipientSchema>();
	}
}
