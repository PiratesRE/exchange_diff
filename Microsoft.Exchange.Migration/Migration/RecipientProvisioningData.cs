using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Migration
{
	internal abstract class RecipientProvisioningData : ProvisioningData
	{
		public bool IsSmtpAddressCheckWithAcceptedDomain
		{
			get
			{
				object obj = base["SMTPAddressCheckWithAcceptedDomain"];
				return obj != null && (bool)obj;
			}
			set
			{
				base["SMTPAddressCheckWithAcceptedDomain"] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)base[ADRecipientSchema.DisplayName];
			}
			set
			{
				base[ADRecipientSchema.DisplayName] = value;
			}
		}

		public string[] EmailAddresses
		{
			get
			{
				return (string[])base[ADRecipientSchema.EmailAddresses];
			}
			set
			{
				base[ADRecipientSchema.EmailAddresses] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)base[ADObjectSchema.Name];
			}
			set
			{
				base[ADObjectSchema.Name] = value;
			}
		}

		public string CustomAttribute1
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute1];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute1] = value;
			}
		}

		public string CustomAttribute2
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute2];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute2] = value;
			}
		}

		public string CustomAttribute3
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute3];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute3] = value;
			}
		}

		public string CustomAttribute4
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute4];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute4] = value;
			}
		}

		public string CustomAttribute5
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute5];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute5] = value;
			}
		}

		public string CustomAttribute6
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute6];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute6] = value;
			}
		}

		public string CustomAttribute7
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute7];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute7] = value;
			}
		}

		public string CustomAttribute8
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute8];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute8] = value;
			}
		}

		public string CustomAttribute9
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute9];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute9] = value;
			}
		}

		public string CustomAttribute10
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute10];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute10] = value;
			}
		}

		public string CustomAttribute11
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute11];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute11] = value;
			}
		}

		public string CustomAttribute12
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute12];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute12] = value;
			}
		}

		public string CustomAttribute13
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute13];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute13] = value;
			}
		}

		public string CustomAttribute14
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute14];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute14] = value;
			}
		}

		public string CustomAttribute15
		{
			get
			{
				return (string)base[ADRecipientSchema.CustomAttribute15];
			}
			set
			{
				base[ADRecipientSchema.CustomAttribute15] = value;
			}
		}

		public const string SMTPAddressCheckWithAcceptedDomainFlagName = "SMTPAddressCheckWithAcceptedDomain";

		public const string MicrosoftOnlineServicesIDParameterName = "MicrosoftOnlineServicesID";
	}
}
