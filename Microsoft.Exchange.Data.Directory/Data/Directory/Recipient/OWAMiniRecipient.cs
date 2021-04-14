using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[Serializable]
	public class OWAMiniRecipient : StorageMiniRecipient
	{
		internal OWAMiniRecipient(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		public OWAMiniRecipient()
		{
		}

		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[OWAMiniRecipientSchema.PhoneticDisplayName];
			}
		}

		public bool ActiveSyncEnabled
		{
			get
			{
				return (bool)(this[OWAMiniRecipientSchema.ActiveSyncEnabled] ?? false);
			}
		}

		public ExternalOofOptions ExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this[OWAMiniRecipientSchema.ExternalOofOptions];
			}
		}

		public string MobilePhoneNumber
		{
			get
			{
				return (string)this[OWAMiniRecipientSchema.MobilePhoneNumber];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return OWAMiniRecipient.schema;
			}
		}

		private static OWAMiniRecipientSchema schema = ObjectSchema.GetInstance<OWAMiniRecipientSchema>();
	}
}
