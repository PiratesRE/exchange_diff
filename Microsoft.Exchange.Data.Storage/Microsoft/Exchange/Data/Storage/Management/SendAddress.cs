using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class SendAddress : ConfigurableObject, IComparable<SendAddress>
	{
		public SendAddress() : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
		}

		public SendAddress(string addressId, string displayName, string mailboxIdParameterString) : this(addressId, displayName, new SendAddressIdentity(mailboxIdParameterString, addressId))
		{
		}

		public SendAddress(string addressId, string displayName, SendAddressIdentity sendAddressIdentity) : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			if (addressId == null)
			{
				throw new ArgumentNullException("addressId");
			}
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			if (displayName.Length == 0)
			{
				throw new ArgumentException("display name was set to empty", "displayName");
			}
			if (sendAddressIdentity == null)
			{
				throw new ArgumentNullException("sendAddressIdentity");
			}
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = sendAddressIdentity;
			this.propertyBag.ResetChangeTracking();
			this.AddressId = addressId;
			this.DisplayName = displayName;
		}

		public string AddressId
		{
			get
			{
				return (string)this[SendAddressSchema.AddressId];
			}
			private set
			{
				this[SendAddressSchema.AddressId] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[SendAddressSchema.DisplayName];
			}
			private set
			{
				this[SendAddressSchema.DisplayName] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SendAddress.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public static SendAddress CreateAutomaticSendAddressFor(string mailboxIdParameterString)
		{
			string empty = string.Empty;
			SendAddressIdentity sendAddressIdentity = new SendAddressIdentity(mailboxIdParameterString, empty);
			return new SendAddress(empty, ClientStrings.AutomaticDisplayName, sendAddressIdentity);
		}

		public int CompareTo(SendAddress other)
		{
			if (other == null)
			{
				return -1;
			}
			if (this.IsAutomatic() && other.IsAutomatic())
			{
				return 0;
			}
			if (this.IsAutomatic())
			{
				return -1;
			}
			if (other.IsAutomatic())
			{
				return 1;
			}
			return string.Compare(this.DisplayName, other.DisplayName, StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsAutomatic()
		{
			return this.AddressId.Equals(string.Empty);
		}

		private static readonly SendAddressSchema schema = ObjectSchema.GetInstance<SendAddressSchema>();
	}
}
