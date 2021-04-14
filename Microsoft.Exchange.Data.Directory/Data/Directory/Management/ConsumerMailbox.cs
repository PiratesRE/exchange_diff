using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ConsumerMailbox : ADPresentationObject
	{
		public ConsumerMailbox()
		{
		}

		public ConsumerMailbox(ADUser dataObject) : base(dataObject)
		{
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "user";
			}
		}

		public ADObjectId Database
		{
			get
			{
				return (ADObjectId)this[ConsumerMailboxSchema.Database];
			}
		}

		public string Description
		{
			get
			{
				MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)this[ConsumerMailboxSchema.Description];
				if (multiValuedProperty != null && multiValuedProperty.Count > 0)
				{
					return multiValuedProperty[0];
				}
				return string.Empty;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ConsumerMailboxSchema.DisplayName];
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)this[ConsumerMailboxSchema.EmailAddresses];
			}
		}

		public Guid? ExchangeGuid
		{
			get
			{
				if (this.Database == null)
				{
					return null;
				}
				return (Guid?)this[ConsumerMailboxSchema.ExchangeGuid];
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				if (this.Database == null)
				{
					return null;
				}
				return (string)this[ConsumerMailboxSchema.LegacyExchangeDN];
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		public NetID NetID
		{
			get
			{
				ulong netID;
				if (this.ExchangeGuid != null && ConsumerIdentityHelper.TryGetPuidFromGuid(this.ExchangeGuid.Value, out netID))
				{
					return new NetID((long)netID);
				}
				return null;
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this[ConsumerMailboxSchema.PrimarySmtpAddress];
			}
		}

		public string ServerName
		{
			get
			{
				if (this.Database == null)
				{
					return null;
				}
				return (string)this[ConsumerMailboxSchema.ServerName];
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				return (SmtpAddress)this[MailboxSchema.WindowsLiveID];
			}
		}

		public PrimaryMailboxSourceType PrimaryMailboxSource
		{
			get
			{
				return (PrimaryMailboxSourceType)this[ADUserSchema.PrimaryMailboxSource];
			}
		}

		public IPAddress SatchmoClusterIp
		{
			get
			{
				return (IPAddress)this[ADUserSchema.SatchmoClusterIp];
			}
		}

		public string SatchmoDGroup
		{
			get
			{
				return (string)this[ADUserSchema.SatchmoDGroup];
			}
		}

		public bool FblEnabled
		{
			get
			{
				return (bool)this[ADUserSchema.FblEnabled];
			}
		}

		public string Gender
		{
			get
			{
				return (string)this[ADUserSchema.Gender];
			}
		}

		public string Occupation
		{
			get
			{
				return (string)this[ADUserSchema.Occupation];
			}
		}

		public string Region
		{
			get
			{
				return (string)this[ADUserSchema.Region];
			}
		}

		public string Timezone
		{
			get
			{
				return (string)this[ADUserSchema.Timezone];
			}
		}

		public DateTime? Birthdate
		{
			get
			{
				return (DateTime?)this[ADUserSchema.Birthdate];
			}
		}

		public string BirthdayPrecision
		{
			get
			{
				return (string)this[ADUserSchema.BirthdayPrecision];
			}
		}

		public string NameVersion
		{
			get
			{
				return (string)this[ADUserSchema.NameVersion];
			}
		}

		public string AlternateSupportEmailAddresses
		{
			get
			{
				return (string)this[ADUserSchema.AlternateSupportEmailAddresses];
			}
		}

		public string PostalCode
		{
			get
			{
				return (string)this[ADUserSchema.PostalCode];
			}
		}

		public bool OptInUser
		{
			get
			{
				return (bool)this[ADUserSchema.OptInUser];
			}
		}

		public bool MigrationDryRun
		{
			get
			{
				return (bool)this[ADUserSchema.MigrationDryRun];
			}
		}

		public bool IsMigratedConsumerMailbox
		{
			get
			{
				return (bool)this[ADUserSchema.IsMigratedConsumerMailbox];
			}
		}

		public string FirstName
		{
			get
			{
				return (string)this[ADUserSchema.FirstName];
			}
		}

		public string LastName
		{
			get
			{
				return (string)this[ADUserSchema.LastName];
			}
		}

		public CountryInfo UsageLocation
		{
			get
			{
				return (CountryInfo)this[ADRecipientSchema.UsageLocation];
			}
		}

		public MultiValuedProperty<int> LocaleID
		{
			get
			{
				return (MultiValuedProperty<int>)this[ADUserSchema.LocaleID];
			}
		}

		public bool IsPremiumConsumerMailbox
		{
			get
			{
				return (bool)this[ADUserSchema.IsPremiumConsumerMailbox];
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ConsumerMailbox.schema;
			}
		}

		internal static ConsumerMailbox FromDataObject(ADUser dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new ConsumerMailbox(dataObject);
		}

		private static ConsumerMailboxSchema schema = ObjectSchema.GetInstance<ConsumerMailboxSchema>();
	}
}
