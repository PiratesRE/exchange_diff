using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class MessageDeliveryGlobalSettings : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MessageDeliveryGlobalSettings.schema;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxReceiveSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MessageDeliveryGlobalSettingsSchema.MaxReceiveSize];
			}
			internal set
			{
				this[MessageDeliveryGlobalSettingsSchema.MaxReceiveSize] = value;
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MessageDeliveryGlobalSettingsSchema.MaxSendSize];
			}
			internal set
			{
				this[MessageDeliveryGlobalSettingsSchema.MaxSendSize] = value;
			}
		}

		public Unlimited<int> MaxRecipientEnvelopeLimit
		{
			get
			{
				return (Unlimited<int>)this[MessageDeliveryGlobalSettingsSchema.MaxRecipientEnvelopeLimit];
			}
			internal set
			{
				this[MessageDeliveryGlobalSettingsSchema.MaxRecipientEnvelopeLimit] = value;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MessageDeliveryGlobalSettings.mostDerivedClass;
			}
		}

		private static MessageDeliveryGlobalSettingsSchema schema = ObjectSchema.GetInstance<MessageDeliveryGlobalSettingsSchema>();

		private static string mostDerivedClass = "msExchMessageDeliveryConfig";

		public static readonly string DefaultName = "Message Delivery";
	}
}
