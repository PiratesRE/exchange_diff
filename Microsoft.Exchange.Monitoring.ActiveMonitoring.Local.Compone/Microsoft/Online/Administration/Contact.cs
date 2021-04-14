using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Contact", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class Contact : IExtensibleDataObject
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
		public string CompanyName
		{
			get
			{
				return this.CompanyNameField;
			}
			set
			{
				this.CompanyNameField = value;
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
		public string EmailAddress
		{
			get
			{
				return this.EmailAddressField;
			}
			set
			{
				this.EmailAddressField = value;
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

		private string CityField;

		private string CompanyNameField;

		private string CountryField;

		private string DepartmentField;

		private string DisplayNameField;

		private string EmailAddressField;

		private ValidationError[] ErrorsField;

		private string FaxField;

		private string FirstNameField;

		private DateTime? LastDirSyncTimeField;

		private string LastNameField;

		private string MobilePhoneField;

		private Guid? ObjectIdField;

		private string OfficeField;

		private string PhoneNumberField;

		private string PostalCodeField;

		private string StateField;

		private string StreetAddressField;

		private string TitleField;

		private ValidationStatus? ValidationStatusField;
	}
}
