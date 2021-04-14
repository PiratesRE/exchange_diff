using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetUserData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string City
		{
			get
			{
				return this.city;
			}
			set
			{
				this.city = value;
				base.TrackPropertyChanged("City");
			}
		}

		[DataMember]
		public string CountryOrRegion
		{
			get
			{
				return this.countryOrRegion;
			}
			set
			{
				this.countryOrRegion = value;
				base.TrackPropertyChanged("CountryOrRegion");
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
				base.TrackPropertyChanged("DisplayName");
			}
		}

		[DataMember]
		public string Fax
		{
			get
			{
				return this.fax;
			}
			set
			{
				this.fax = value;
				base.TrackPropertyChanged("Fax");
			}
		}

		[DataMember]
		public string FirstName
		{
			get
			{
				return this.firstName;
			}
			set
			{
				this.firstName = value;
				base.TrackPropertyChanged("FirstName");
			}
		}

		[DataMember]
		public string HomePhone
		{
			get
			{
				return this.homePhone;
			}
			set
			{
				this.homePhone = value;
				base.TrackPropertyChanged("HomePhone");
			}
		}

		[DataMember]
		public string Initials
		{
			get
			{
				return this.initials;
			}
			set
			{
				this.initials = value;
				base.TrackPropertyChanged("Initials");
			}
		}

		[DataMember]
		public string LastName
		{
			get
			{
				return this.lastName;
			}
			set
			{
				this.lastName = value;
				base.TrackPropertyChanged("LastName");
			}
		}

		[DataMember]
		public string MobilePhone
		{
			get
			{
				return this.mobilePhone;
			}
			set
			{
				this.mobilePhone = value;
				base.TrackPropertyChanged("MobilePhone");
			}
		}

		[DataMember]
		public string Office
		{
			get
			{
				return this.office;
			}
			set
			{
				this.office = value;
				base.TrackPropertyChanged("Office");
			}
		}

		[DataMember]
		public string Phone
		{
			get
			{
				return this.phone;
			}
			set
			{
				this.phone = value;
				base.TrackPropertyChanged("Phone");
			}
		}

		[DataMember]
		public string PostalCode
		{
			get
			{
				return this.postalCode;
			}
			set
			{
				this.postalCode = value;
				base.TrackPropertyChanged("PostalCode");
			}
		}

		[DataMember]
		public string StateOrProvince
		{
			get
			{
				return this.stateOrProvince;
			}
			set
			{
				this.stateOrProvince = value;
				base.TrackPropertyChanged("StateOrProvince");
			}
		}

		[DataMember]
		public string StreetAddress
		{
			get
			{
				return this.streetAddress;
			}
			set
			{
				this.streetAddress = value;
				base.TrackPropertyChanged("StreetAddress");
			}
		}

		private string city;

		private string countryOrRegion;

		private string displayName;

		private string fax;

		private string firstName;

		private string homePhone;

		private string initials;

		private string lastName;

		private string mobilePhone;

		private string office;

		private string phone;

		private string postalCode;

		private string stateOrProvince;

		private string streetAddress;
	}
}
