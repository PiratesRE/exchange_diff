using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[Serializable]
	public class CompanyProfile
	{
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

		public string Description
		{
			get
			{
				return this.descriptionField;
			}
			set
			{
				this.descriptionField = value;
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

		public string Street
		{
			get
			{
				return this.streetField;
			}
			set
			{
				this.streetField = value;
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

		public string CommunicationCulture
		{
			get
			{
				return this.communicationCultureField;
			}
			set
			{
				this.communicationCultureField = value;
			}
		}

		public CompanyProfileState LifecycleState
		{
			get
			{
				return this.lifecycleStateField;
			}
			set
			{
				this.lifecycleStateField = value;
			}
		}

		public CompanyProfileStateChangeReason LifecycleStateChangeReason
		{
			get
			{
				return this.lifecycleStateChangeReasonField;
			}
			set
			{
				this.lifecycleStateChangeReasonField = value;
			}
		}

		private string displayNameField;

		private string descriptionField;

		private string telephoneNumberField;

		private string streetField;

		private string cityField;

		private string stateField;

		private string postalCodeField;

		private string countryLetterCodeField;

		private string communicationCultureField;

		private CompanyProfileState lifecycleStateField;

		private CompanyProfileStateChangeReason lifecycleStateChangeReasonField;
	}
}
