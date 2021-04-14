using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class SetOrgPerson : OrgPersonBasicProperties
	{
		[DataMember]
		public string StreetAddress
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.StreetAddress];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.StreetAddress] = value;
			}
		}

		[DataMember]
		public string City
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.City];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.City] = value;
			}
		}

		[DataMember]
		public string StateOrProvince
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.StateOrProvince];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.StateOrProvince] = value;
			}
		}

		[DataMember]
		public string PostalCode
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.PostalCode];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.PostalCode] = value;
			}
		}

		[DataMember]
		public string CountryOrRegion
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.CountryOrRegion];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.CountryOrRegion] = (string.IsNullOrEmpty(value) ? null : value);
			}
		}

		[DataMember]
		public string Office
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.Office];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Office] = value;
			}
		}

		[DataMember]
		public string Phone
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.Phone];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Phone] = value;
			}
		}

		[DataMember]
		public string Fax
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.Fax];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Fax] = value;
			}
		}

		[DataMember]
		public string HomePhone
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.HomePhone];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.HomePhone] = value;
			}
		}

		[DataMember]
		public string MobilePhone
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.MobilePhone];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.MobilePhone] = value;
			}
		}

		[DataMember]
		public string Notes
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.Notes];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Notes] = value;
			}
		}

		[DataMember]
		public string Title
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.Title];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Title] = value;
			}
		}

		[DataMember]
		public string Department
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.Department];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Department] = value;
			}
		}

		[DataMember]
		public string Company
		{
			get
			{
				return (string)base[OrgPersonPresentationObjectSchema.Company];
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Company] = value;
			}
		}

		[DataMember]
		public Identity Manager
		{
			get
			{
				return Identity.FromIdParameter(base[OrgPersonPresentationObjectSchema.Manager]);
			}
			set
			{
				base[OrgPersonPresentationObjectSchema.Manager] = value.ToIdParameter();
			}
		}
	}
}
