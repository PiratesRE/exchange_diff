using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class MicrosoftExchangeRecipient : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MicrosoftExchangeRecipient.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MicrosoftExchangeRecipient.MostDerivedClass;
			}
		}

		internal MicrosoftExchangeRecipient(IConfigurationSession session, string commonName, ADObjectId containerId)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId("CN", commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public MicrosoftExchangeRecipient()
		{
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public bool EmailAddressPolicyEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.EmailAddressPolicyEnabled];
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[ADRecipientSchema.EmailAddresses];
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ADRecipientSchema.DisplayName];
			}
		}

		public string Alias
		{
			get
			{
				return (string)this[ADRecipientSchema.Alias];
			}
		}

		public bool HiddenFromAddressListsEnabled
		{
			get
			{
				return (bool)this[ADRecipientSchema.HiddenFromAddressListsEnabled];
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[ADRecipientSchema.LegacyExchangeDN];
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[ADRecipientSchema.PrimarySmtpAddress];
			}
		}

		public ADObjectId ForwardingAddress
		{
			get
			{
				return (ADObjectId)this[ADRecipientSchema.ForwardingAddress];
			}
		}

		private static MicrosoftExchangeRecipientSchema schema = ObjectSchema.GetInstance<MicrosoftExchangeRecipientSchema>();

		internal static string MostDerivedClass = "msExchExchangeServerRecipient";
	}
}
