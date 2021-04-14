using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Migration
{
	internal class ContactProvisioningData : RecipientProvisioningData
	{
		internal ContactProvisioningData()
		{
			base.Action = ProvisioningAction.CreateNew;
			base.ProvisioningType = ProvisioningType.Contact;
		}

		public string FirstName
		{
			get
			{
				return (string)base[ADOrgPersonSchema.FirstName];
			}
			set
			{
				base[ADOrgPersonSchema.FirstName] = value;
			}
		}

		public string Initials
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Initials];
			}
			set
			{
				base[ADOrgPersonSchema.Initials] = value;
			}
		}

		public string LastName
		{
			get
			{
				return (string)base[ADOrgPersonSchema.LastName];
			}
			set
			{
				base[ADOrgPersonSchema.LastName] = value;
			}
		}

		public string ExternalEmailAddress
		{
			get
			{
				return (string)base[ADRecipientSchema.ExternalEmailAddress];
			}
			set
			{
				base[ADRecipientSchema.ExternalEmailAddress] = value;
			}
		}

		public string Company
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Company];
			}
			set
			{
				base[ADOrgPersonSchema.Company] = value;
			}
		}

		public string Department
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Department];
			}
			set
			{
				base[ADOrgPersonSchema.Department] = value;
			}
		}

		public string Fax
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Fax];
			}
			set
			{
				base[ADOrgPersonSchema.Fax] = value;
			}
		}

		public string MobilePhone
		{
			get
			{
				return (string)base[ADOrgPersonSchema.MobilePhone];
			}
			set
			{
				base[ADOrgPersonSchema.MobilePhone] = value;
			}
		}

		public string Office
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Office];
			}
			set
			{
				base[ADOrgPersonSchema.Office] = value;
			}
		}

		public string Phone
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Phone];
			}
			set
			{
				base[ADOrgPersonSchema.Phone] = value;
			}
		}

		public string Title
		{
			get
			{
				return (string)base[ADOrgPersonSchema.Title];
			}
			set
			{
				base[ADOrgPersonSchema.Title] = value;
			}
		}

		public string HomePhone
		{
			get
			{
				return (string)base[ADOrgPersonSchema.HomePhone];
			}
			set
			{
				base[ADOrgPersonSchema.HomePhone] = value;
			}
		}

		public string StreetAddress
		{
			get
			{
				return (string)base[ADOrgPersonSchema.StreetAddress];
			}
			set
			{
				base[ADOrgPersonSchema.StreetAddress] = value;
			}
		}

		public string City
		{
			get
			{
				return (string)base[ADOrgPersonSchema.City];
			}
			set
			{
				base[ADOrgPersonSchema.City] = value;
			}
		}

		public string StateOrProvince
		{
			get
			{
				return (string)base[ADOrgPersonSchema.StateOrProvince];
			}
			set
			{
				base[ADOrgPersonSchema.StateOrProvince] = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return (string)base[ADOrgPersonSchema.PostalCode];
			}
			set
			{
				base[ADOrgPersonSchema.PostalCode] = value;
			}
		}

		public string CountryOrRegion
		{
			get
			{
				return (string)base[ADOrgPersonSchema.CountryOrRegion];
			}
			set
			{
				base[ADOrgPersonSchema.CountryOrRegion] = value;
			}
		}

		public string Notes
		{
			get
			{
				return (string)base[ADRecipientSchema.Notes];
			}
			set
			{
				base[ADRecipientSchema.Notes] = value;
			}
		}

		public static ContactProvisioningData Create(string name, string emailAddress)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(name, "name");
			MigrationUtil.ThrowOnNullOrEmptyArgument(emailAddress, "emailAddress");
			return new ContactProvisioningData
			{
				Name = name,
				ExternalEmailAddress = emailAddress
			};
		}
	}
}
