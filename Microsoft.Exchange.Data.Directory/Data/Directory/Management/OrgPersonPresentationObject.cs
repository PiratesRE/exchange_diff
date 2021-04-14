using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public abstract class OrgPersonPresentationObject : ADPresentationObject
	{
		protected OrgPersonPresentationObject()
		{
		}

		protected OrgPersonPresentationObject(ADObject dataObject) : base(dataObject)
		{
		}

		[Parameter(Mandatory = false)]
		public string AssistantName
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.AssistantName];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.AssistantName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string City
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.City];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.City] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Company
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Company];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Company] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo CountryOrRegion
		{
			get
			{
				return (CountryInfo)this[OrgPersonPresentationObjectSchema.CountryOrRegion];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.CountryOrRegion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Department
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Department];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Department] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> DirectReports
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[OrgPersonPresentationObjectSchema.DirectReports];
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.DisplayName];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.DisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Fax
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Fax];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Fax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FirstName
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.FirstName];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.FirstName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public GeoCoordinates GeoCoordinates
		{
			get
			{
				return (GeoCoordinates)this[OrgPersonPresentationObjectSchema.GeoCoordinates];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.GeoCoordinates] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HomePhone
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.HomePhone];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.HomePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Initials
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Initials];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Initials] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LastName
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.LastName];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.LastName] = value;
			}
		}

		public ADObjectId Manager
		{
			get
			{
				return (ADObjectId)this[OrgPersonPresentationObjectSchema.Manager];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Manager] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MobilePhone
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.MobilePhone];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.MobilePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Notes];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Notes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Office
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Office];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Office] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherFax
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrgPersonPresentationObjectSchema.OtherFax];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.OtherFax] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherHomePhone
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrgPersonPresentationObjectSchema.OtherHomePhone];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.OtherHomePhone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> OtherTelephone
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrgPersonPresentationObjectSchema.OtherTelephone];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.OtherTelephone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Pager
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Pager];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Pager] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Phone
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Phone];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Phone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.PhoneticDisplayName];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.PhoneticDisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PostalCode
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.PostalCode];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.PostalCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> PostOfficeBox
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrgPersonPresentationObjectSchema.PostOfficeBox];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.PostOfficeBox] = value;
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return (RecipientType)this[OrgPersonPresentationObjectSchema.RecipientType];
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[OrgPersonPresentationObjectSchema.RecipientTypeDetails];
			}
		}

		[Parameter(Mandatory = false)]
		public string SimpleDisplayName
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.SimpleDisplayName];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.SimpleDisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StateOrProvince
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.StateOrProvince];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.StateOrProvince] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string StreetAddress
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.StreetAddress];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.StreetAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Title
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.Title];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.Title] = value;
			}
		}

		public ADObjectId UMDialPlan
		{
			get
			{
				return (ADObjectId)this[OrgPersonPresentationObjectSchema.UMRecipientDialPlanId];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.UMRecipientDialPlanId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UMDtmfMap
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrgPersonPresentationObjectSchema.UMDtmfMap];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.UMDtmfMap] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AllowUMCallsFromNonUsersFlags AllowUMCallsFromNonUsers
		{
			get
			{
				return (AllowUMCallsFromNonUsersFlags)this[OrgPersonPresentationObjectSchema.AllowUMCallsFromNonUsers];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.AllowUMCallsFromNonUsers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WebPage
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.WebPage];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.WebPage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TelephoneAssistant
		{
			get
			{
				return (string)this[OrgPersonPresentationObjectSchema.TelephoneAssistant];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.TelephoneAssistant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress WindowsEmailAddress
		{
			get
			{
				return (SmtpAddress)this[OrgPersonPresentationObjectSchema.WindowsEmailAddress];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.WindowsEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> UMCallingLineIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrgPersonPresentationObjectSchema.UMCallingLineIds];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.UMCallingLineIds] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? SeniorityIndex
		{
			get
			{
				return (int?)this[OrgPersonPresentationObjectSchema.SeniorityIndex];
			}
			set
			{
				this[OrgPersonPresentationObjectSchema.SeniorityIndex] = value;
			}
		}

		public MultiValuedProperty<string> VoiceMailSettings
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrgPersonPresentationObjectSchema.VoiceMailSettings];
			}
		}
	}
}
