using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[DesignerCategory("code")]
	[Serializable]
	public class User
	{
		public string Initials
		{
			get
			{
				return this.initialsField;
			}
			set
			{
				this.initialsField = value;
			}
		}

		public string JobTitle
		{
			get
			{
				return this.jobTitleField;
			}
			set
			{
				this.jobTitleField = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayNameField;
			}
			set
			{
				this.displayNameField = value;
			}
		}

		public string EmailAddress
		{
			get
			{
				return this.emailAddressField;
			}
			set
			{
				this.emailAddressField = value;
			}
		}

		public string GivenName
		{
			get
			{
				return this.givenNameField;
			}
			set
			{
				this.givenNameField = value;
			}
		}

		public string MiddleName
		{
			get
			{
				return this.middleNameField;
			}
			set
			{
				this.middleNameField = value;
			}
		}

		public string Surname
		{
			get
			{
				return this.surnameField;
			}
			set
			{
				this.surnameField = value;
			}
		}

		public string PreferredLanguage
		{
			get
			{
				return this.preferredLanguageField;
			}
			set
			{
				this.preferredLanguageField = value;
			}
		}

		public string UsageLocation
		{
			get
			{
				return this.usageLocationField;
			}
			set
			{
				this.usageLocationField = value;
			}
		}

		public string UserName
		{
			get
			{
				return this.userNameField;
			}
			set
			{
				this.userNameField = value;
			}
		}

		public string NewUserPassword
		{
			get
			{
				return this.newUserPasswordField;
			}
			set
			{
				this.newUserPasswordField = value;
			}
		}

		public string StreetAddress
		{
			get
			{
				return this.streetAddressField;
			}
			set
			{
				this.streetAddressField = value;
			}
		}

		public string City
		{
			get
			{
				return this.cityField;
			}
			set
			{
				this.cityField = value;
			}
		}

		public string State
		{
			get
			{
				return this.stateField;
			}
			set
			{
				this.stateField = value;
			}
		}

		public string PostalCode
		{
			get
			{
				return this.postalCodeField;
			}
			set
			{
				this.postalCodeField = value;
			}
		}

		public string CountryLetterCode
		{
			get
			{
				return this.countryLetterCodeField;
			}
			set
			{
				this.countryLetterCodeField = value;
			}
		}

		public string Mobile
		{
			get
			{
				return this.mobileField;
			}
			set
			{
				this.mobileField = value;
			}
		}

		public string TelephoneNumber
		{
			get
			{
				return this.telephoneNumberField;
			}
			set
			{
				this.telephoneNumberField = value;
			}
		}

		private string initialsField;

		private string jobTitleField;

		private string displayNameField;

		private string emailAddressField;

		private string givenNameField;

		private string middleNameField;

		private string surnameField;

		private string preferredLanguageField;

		private string usageLocationField;

		private string userNameField;

		private string newUserPasswordField;

		private string streetAddressField;

		private string cityField;

		private string stateField;

		private string postalCodeField;

		private string countryLetterCodeField;

		private string mobileField;

		private string telephoneNumberField;
	}
}
