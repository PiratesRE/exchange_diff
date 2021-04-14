using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[KnownType(typeof(UserExtended))]
	[DataContract(Name = "User", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class User : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public string[] AlternateEmailAddresses
		{
			get
			{
				return this.AlternateEmailAddressesField;
			}
			set
			{
				this.AlternateEmailAddressesField = value;
			}
		}

		[DataMember]
		public bool? BlockCredential
		{
			get
			{
				return this.BlockCredentialField;
			}
			set
			{
				this.BlockCredentialField = value;
			}
		}

		[DataMember]
		public string City
		{
			get
			{
				return this.CityField;
			}
			set
			{
				this.CityField = value;
			}
		}

		[DataMember]
		public string Country
		{
			get
			{
				return this.CountryField;
			}
			set
			{
				this.CountryField = value;
			}
		}

		[DataMember]
		public string Department
		{
			get
			{
				return this.DepartmentField;
			}
			set
			{
				this.DepartmentField = value;
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.DisplayNameField;
			}
			set
			{
				this.DisplayNameField = value;
			}
		}

		[DataMember]
		public ValidationError[] Errors
		{
			get
			{
				return this.ErrorsField;
			}
			set
			{
				this.ErrorsField = value;
			}
		}

		[DataMember]
		public string Fax
		{
			get
			{
				return this.FaxField;
			}
			set
			{
				this.FaxField = value;
			}
		}

		[DataMember]
		public string FirstName
		{
			get
			{
				return this.FirstNameField;
			}
			set
			{
				this.FirstNameField = value;
			}
		}

		[DataMember]
		public string ImmutableId
		{
			get
			{
				return this.ImmutableIdField;
			}
			set
			{
				this.ImmutableIdField = value;
			}
		}

		[DataMember]
		public bool? IsBlackberryUser
		{
			get
			{
				return this.IsBlackberryUserField;
			}
			set
			{
				this.IsBlackberryUserField = value;
			}
		}

		[DataMember]
		public bool? IsLicensed
		{
			get
			{
				return this.IsLicensedField;
			}
			set
			{
				this.IsLicensedField = value;
			}
		}

		[DataMember]
		public DateTime? LastDirSyncTime
		{
			get
			{
				return this.LastDirSyncTimeField;
			}
			set
			{
				this.LastDirSyncTimeField = value;
			}
		}

		[DataMember]
		public string LastName
		{
			get
			{
				return this.LastNameField;
			}
			set
			{
				this.LastNameField = value;
			}
		}

		[DataMember]
		public bool? LicenseReconciliationNeeded
		{
			get
			{
				return this.LicenseReconciliationNeededField;
			}
			set
			{
				this.LicenseReconciliationNeededField = value;
			}
		}

		[DataMember]
		public UserLicense[] Licenses
		{
			get
			{
				return this.LicensesField;
			}
			set
			{
				this.LicensesField = value;
			}
		}

		[DataMember]
		public string LiveId
		{
			get
			{
				return this.LiveIdField;
			}
			set
			{
				this.LiveIdField = value;
			}
		}

		[DataMember]
		public string MobilePhone
		{
			get
			{
				return this.MobilePhoneField;
			}
			set
			{
				this.MobilePhoneField = value;
			}
		}

		[DataMember]
		public Guid? ObjectId
		{
			get
			{
				return this.ObjectIdField;
			}
			set
			{
				this.ObjectIdField = value;
			}
		}

		[DataMember]
		public string Office
		{
			get
			{
				return this.OfficeField;
			}
			set
			{
				this.OfficeField = value;
			}
		}

		[DataMember]
		public ProvisioningStatus OverallProvisioningStatus
		{
			get
			{
				return this.OverallProvisioningStatusField;
			}
			set
			{
				this.OverallProvisioningStatusField = value;
			}
		}

		[DataMember]
		public bool? PasswordNeverExpires
		{
			get
			{
				return this.PasswordNeverExpiresField;
			}
			set
			{
				this.PasswordNeverExpiresField = value;
			}
		}

		[DataMember]
		public string PhoneNumber
		{
			get
			{
				return this.PhoneNumberField;
			}
			set
			{
				this.PhoneNumberField = value;
			}
		}

		[DataMember]
		public string PostalCode
		{
			get
			{
				return this.PostalCodeField;
			}
			set
			{
				this.PostalCodeField = value;
			}
		}

		[DataMember]
		public string PreferredLanguage
		{
			get
			{
				return this.PreferredLanguageField;
			}
			set
			{
				this.PreferredLanguageField = value;
			}
		}

		[DataMember]
		public DateTime? SoftDeletionTimestamp
		{
			get
			{
				return this.SoftDeletionTimestampField;
			}
			set
			{
				this.SoftDeletionTimestampField = value;
			}
		}

		[DataMember]
		public string State
		{
			get
			{
				return this.StateField;
			}
			set
			{
				this.StateField = value;
			}
		}

		[DataMember]
		public string StreetAddress
		{
			get
			{
				return this.StreetAddressField;
			}
			set
			{
				this.StreetAddressField = value;
			}
		}

		[DataMember]
		public bool? StrongPasswordRequired
		{
			get
			{
				return this.StrongPasswordRequiredField;
			}
			set
			{
				this.StrongPasswordRequiredField = value;
			}
		}

		[DataMember]
		public string Title
		{
			get
			{
				return this.TitleField;
			}
			set
			{
				this.TitleField = value;
			}
		}

		[DataMember]
		public string UsageLocation
		{
			get
			{
				return this.UsageLocationField;
			}
			set
			{
				this.UsageLocationField = value;
			}
		}

		[DataMember]
		public string UserPrincipalName
		{
			get
			{
				return this.UserPrincipalNameField;
			}
			set
			{
				this.UserPrincipalNameField = value;
			}
		}

		[DataMember]
		public ValidationStatus? ValidationStatus
		{
			get
			{
				return this.ValidationStatusField;
			}
			set
			{
				this.ValidationStatusField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] AlternateEmailAddressesField;

		private bool? BlockCredentialField;

		private string CityField;

		private string CountryField;

		private string DepartmentField;

		private string DisplayNameField;

		private ValidationError[] ErrorsField;

		private string FaxField;

		private string FirstNameField;

		private string ImmutableIdField;

		private bool? IsBlackberryUserField;

		private bool? IsLicensedField;

		private DateTime? LastDirSyncTimeField;

		private string LastNameField;

		private bool? LicenseReconciliationNeededField;

		private UserLicense[] LicensesField;

		private string LiveIdField;

		private string MobilePhoneField;

		private Guid? ObjectIdField;

		private string OfficeField;

		private ProvisioningStatus OverallProvisioningStatusField;

		private bool? PasswordNeverExpiresField;

		private string PhoneNumberField;

		private string PostalCodeField;

		private string PreferredLanguageField;

		private DateTime? SoftDeletionTimestampField;

		private string StateField;

		private string StreetAddressField;

		private bool? StrongPasswordRequiredField;

		private string TitleField;

		private string UsageLocationField;

		private string UserPrincipalNameField;

		private ValidationStatus? ValidationStatusField;
	}
}
