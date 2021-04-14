using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.E4E
{
	[Serializable]
	public class EncryptionConfiguration : ConfigurableObject
	{
		public EncryptionConfiguration() : base(new SimpleProviderPropertyBag())
		{
		}

		public EncryptionConfiguration(string imageBase64, string emailText, string portalText, string disclaimerText, bool otpEnabled) : base(new SimpleProviderPropertyBag())
		{
			this.ImageBase64 = imageBase64;
			this.EmailText = emailText;
			this.PortalText = portalText;
			this.DisclaimerText = disclaimerText;
			this.OTPEnabled = otpEnabled;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return EncryptionConfiguration.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal string ImageBase64
		{
			get
			{
				if (this.Image != null)
				{
					return Convert.ToBase64String(this.Image);
				}
				return string.Empty;
			}
			set
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					this.Image = Convert.FromBase64String(value);
					return;
				}
				this.Image = null;
			}
		}

		public byte[] Image
		{
			get
			{
				return (byte[])this[EncryptionConfigurationSchema.Image];
			}
			set
			{
				this[EncryptionConfigurationSchema.Image] = value;
			}
		}

		public string EmailText
		{
			get
			{
				return (string)this[EncryptionConfigurationSchema.EmailText];
			}
			set
			{
				this[EncryptionConfigurationSchema.EmailText] = value;
			}
		}

		public string PortalText
		{
			get
			{
				return (string)this[EncryptionConfigurationSchema.PortalText];
			}
			set
			{
				this[EncryptionConfigurationSchema.PortalText] = value;
			}
		}

		public string DisclaimerText
		{
			get
			{
				return (string)this[EncryptionConfigurationSchema.DisclaimerText];
			}
			set
			{
				this[EncryptionConfigurationSchema.DisclaimerText] = value;
			}
		}

		public bool OTPEnabled
		{
			get
			{
				return (bool)this[EncryptionConfigurationSchema.OTPEnabled];
			}
			set
			{
				this[EncryptionConfigurationSchema.OTPEnabled] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (ObjectId)this[EncryptionConfigurationSchema.Identity];
			}
		}

		internal static object IdentityGetter(IPropertyBag propertyBag)
		{
			byte[] image = (byte[])propertyBag[EncryptionConfigurationSchema.Image];
			string emailText = (string)propertyBag[EncryptionConfigurationSchema.EmailText];
			string portalText = (string)propertyBag[EncryptionConfigurationSchema.PortalText];
			string disclaimerText = (string)propertyBag[EncryptionConfigurationSchema.DisclaimerText];
			bool otpEnabled = (bool)propertyBag[EncryptionConfigurationSchema.OTPEnabled];
			return new OMEConfigurationId(image, emailText, portalText, disclaimerText, otpEnabled);
		}

		private static EncryptionConfigurationSchema schema = ObjectSchema.GetInstance<EncryptionConfigurationSchema>();
	}
}
