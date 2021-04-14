using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ADContact : ADRecipient, IADOrgPerson, IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADContact.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADContact.MostDerivedClass;
			}
		}

		internal override string ObjectCategoryName
		{
			get
			{
				return ADContact.ObjectCategoryNameInternal;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, this.MostDerivedObjectClass);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (RecipientType.MailContact == base.RecipientType && base.ExternalEmailAddress == null)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorNullExternalEmailAddress, ADRecipientSchema.ExternalEmailAddress, null));
			}
		}

		internal ADContact(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal ADContact(IRecipientSession session, string commonName, ADObjectId containerId)
		{
			this.m_Session = session;
			base.SetId(containerId.GetChildId("CN", commonName));
			base.SetObjectClass(this.MostDerivedObjectClass);
		}

		public ADContact()
		{
		}

		public bool DeliverToForwardingAddress
		{
			get
			{
				return (bool)this[ADContactSchema.DeliverToMailboxAndForward];
			}
			set
			{
				this[ADContactSchema.DeliverToMailboxAndForward] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> CatchAllRecipientBL
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADUserSchema.CatchAllRecipientBL];
			}
		}

		public MultiValuedProperty<byte[]> UserCertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ADRecipientSchema.Certificate];
			}
			set
			{
				this[ADRecipientSchema.Certificate] = value;
			}
		}

		public MultiValuedProperty<byte[]> UserSMIMECertificate
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[ADRecipientSchema.SMimeCertificate];
			}
			set
			{
				this[ADRecipientSchema.SMimeCertificate] = value;
			}
		}

		public string C
		{
			get
			{
				return (string)this[ADContactSchema.C];
			}
			set
			{
				this[ADContactSchema.C] = value;
			}
		}

		public string City
		{
			get
			{
				return (string)this[ADContactSchema.City];
			}
			set
			{
				this[ADContactSchema.City] = value;
			}
		}

		public string Co
		{
			get
			{
				return (string)this[ADContactSchema.Co];
			}
			set
			{
				this[ADContactSchema.Co] = value;
			}
		}

		public string Company
		{
			get
			{
				return (string)this[ADContactSchema.Company];
			}
			set
			{
				this[ADContactSchema.Company] = value;
			}
		}

		public int CountryCode
		{
			get
			{
				return (int)this[ADContactSchema.CountryCode];
			}
			set
			{
				this[ADContactSchema.CountryCode] = value;
			}
		}

		public string CountryOrRegionDisplayName
		{
			get
			{
				return (string)this[ADContactSchema.Co];
			}
		}

		public CountryInfo CountryOrRegion
		{
			get
			{
				return (CountryInfo)this[ADContactSchema.CountryOrRegion];
			}
			set
			{
				this[ADContactSchema.CountryOrRegion] = value;
			}
		}

		public string Department
		{
			get
			{
				return (string)this[ADContactSchema.Department];
			}
			set
			{
				this[ADContactSchema.Department] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> DirectReports
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADContactSchema.DirectReports];
			}
		}

		public string Fax
		{
			get
			{
				return (string)this[ADContactSchema.Fax];
			}
			set
			{
				this[ADContactSchema.Fax] = value;
			}
		}

		public string FirstName
		{
			get
			{
				return (string)this[ADContactSchema.FirstName];
			}
			set
			{
				this[ADContactSchema.FirstName] = value;
			}
		}

		public string HomePhone
		{
			get
			{
				return (string)this[ADContactSchema.HomePhone];
			}
			set
			{
				this[ADContactSchema.HomePhone] = value;
			}
		}

		public MultiValuedProperty<string> IndexedPhoneNumbers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADRecipientSchema.IndexedPhoneNumbers];
			}
		}

		public string Initials
		{
			get
			{
				return (string)this[ADContactSchema.Initials];
			}
			set
			{
				this[ADContactSchema.Initials] = value;
			}
		}

		public MultiValuedProperty<CultureInfo> Languages
		{
			get
			{
				return (MultiValuedProperty<CultureInfo>)this[ADContactSchema.Languages];
			}
			set
			{
				this[ADContactSchema.Languages] = value;
			}
		}

		public string LastName
		{
			get
			{
				return (string)this[ADContactSchema.LastName];
			}
			set
			{
				this[ADContactSchema.LastName] = value;
			}
		}

		public ADObjectId Manager
		{
			get
			{
				return (ADObjectId)this[ADContactSchema.Manager];
			}
			set
			{
				this[ADContactSchema.Manager] = value;
			}
		}

		public string MobilePhone
		{
			get
			{
				return (string)this[ADContactSchema.MobilePhone];
			}
			set
			{
				this[ADContactSchema.MobilePhone] = value;
			}
		}

		public string Office
		{
			get
			{
				return (string)this[ADContactSchema.Office];
			}
			set
			{
				this[ADContactSchema.Office] = value;
			}
		}

		public MultiValuedProperty<string> OtherFax
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADContactSchema.OtherFax];
			}
			set
			{
				this[ADContactSchema.OtherFax] = value;
			}
		}

		public MultiValuedProperty<string> OtherHomePhone
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADContactSchema.OtherHomePhone];
			}
			set
			{
				this[ADContactSchema.OtherHomePhone] = value;
			}
		}

		public MultiValuedProperty<string> OtherTelephone
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADContactSchema.OtherTelephone];
			}
			set
			{
				this[ADContactSchema.OtherTelephone] = value;
			}
		}

		public string Pager
		{
			get
			{
				return (string)this[ADContactSchema.Pager];
			}
			set
			{
				this[ADContactSchema.Pager] = value;
			}
		}

		public string Phone
		{
			get
			{
				return (string)this[ADContactSchema.Phone];
			}
			set
			{
				this[ADContactSchema.Phone] = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return (string)this[ADContactSchema.PostalCode];
			}
			set
			{
				this[ADContactSchema.PostalCode] = value;
			}
		}

		public MultiValuedProperty<string> PostOfficeBox
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADContactSchema.PostOfficeBox];
			}
			set
			{
				this[ADContactSchema.PostOfficeBox] = value;
			}
		}

		public string RtcSipLine
		{
			get
			{
				return (string)this[ADContactSchema.RtcSipLine];
			}
		}

		public MultiValuedProperty<string> SanitizedPhoneNumbers
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADContactSchema.SanitizedPhoneNumbers];
			}
		}

		public string StateOrProvince
		{
			get
			{
				return (string)this[ADContactSchema.StateOrProvince];
			}
			set
			{
				this[ADContactSchema.StateOrProvince] = value;
			}
		}

		public string StreetAddress
		{
			get
			{
				return (string)this[ADContactSchema.StreetAddress];
			}
			set
			{
				this[ADContactSchema.StreetAddress] = value;
			}
		}

		public string TelephoneAssistant
		{
			get
			{
				return (string)this[ADContactSchema.TelephoneAssistant];
			}
			set
			{
				this[ADContactSchema.TelephoneAssistant] = value;
			}
		}

		public string Title
		{
			get
			{
				return (string)this[ADContactSchema.Title];
			}
			set
			{
				this[ADContactSchema.Title] = value;
			}
		}

		public MultiValuedProperty<string> UMCallingLineIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADContactSchema.UMCallingLineIds];
			}
			set
			{
				this[ADContactSchema.UMCallingLineIds] = value;
			}
		}

		public MultiValuedProperty<string> VoiceMailSettings
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADContactSchema.VoiceMailSettings];
			}
			set
			{
				this[ADContactSchema.VoiceMailSettings] = value;
			}
		}

		public object[][] GetManagementChainView(bool getPeers, params PropertyDefinition[] returnProperties)
		{
			return ADOrgPerson.GetManagementChainView(base.Session, this, getPeers, returnProperties);
		}

		public object[][] GetDirectReportsView(params PropertyDefinition[] returnProperties)
		{
			return ADOrgPerson.GetDirectReportsView(base.Session, this, returnProperties);
		}

		public override void PopulateDtmfMap(bool create)
		{
			string text = (this.FirstName + this.LastName).Trim();
			string lastFirst = (this.LastName + this.FirstName).Trim();
			if (string.IsNullOrEmpty(text))
			{
				text = base.DisplayName;
				lastFirst = base.DisplayName;
			}
			base.PopulateDtmfMap(create, text, lastFirst, base.PrimarySmtpAddress, this.SanitizedPhoneNumbers);
		}

		private static readonly ADContactSchema schema = ObjectSchema.GetInstance<ADContactSchema>();

		internal static string MostDerivedClass = "contact";

		internal static string ObjectCategoryNameInternal = "person";
	}
}
