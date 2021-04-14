using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	internal class PhonebookServicesWebResponse
	{
		[DataMember]
		public PhonebookServicesWebResponse.PhonebookResultSet Phonebook { get; set; }

		[DataMember]
		public PhonebookServicesWebResponse.ErrorResult[] Errors { get; set; }

		[DataContract]
		internal class ErrorResult : IBingError
		{
			[DataMember]
			public string Code { get; set; }

			[DataMember]
			public string Message { get; set; }
		}

		[DataContract]
		internal class PhonebookResultSet
		{
			[DataMember]
			public PhonebookServicesWebResponse.PhonebookResultSet.PhonebookResult[] Results { get; set; }

			[DataContract(Name = "Results")]
			internal class PhonebookResult : IBingResult
			{
				[DataMember(Name = "Title")]
				public string Name { get; set; }

				[DataMember]
				public string PhoneNumber { get; set; }

				[DataMember(Name = "Address")]
				public string StreetAddress { get; set; }

				[DataMember]
				public string City { get; set; }

				[DataMember(Name = "StateOrProvince")]
				public string State { get; set; }

				[DataMember(Name = "CountryOrRegion")]
				public string Country { get; set; }

				[DataMember(Name = "PostalCode")]
				private string OriginalPostalCode { get; set; }

				public string PostalCode
				{
					get
					{
						if (this.Country == "US")
						{
							return null;
						}
						return this.OriginalPostalCode;
					}
				}

				[DataMember]
				public double Latitude { get; set; }

				[DataMember]
				public double Longitude { get; set; }

				[DataMember(Name = "DisplayUrl")]
				public string LocalHomePage { get; set; }

				[DataMember(Name = "BusinessUrl")]
				public string BusinessHomePage { get; set; }

				public LocationSource LocationSource
				{
					get
					{
						return LocationSource.PhonebookServices;
					}
				}

				[DataMember(Name = "UniqueId")]
				public string LocationUri { get; set; }

				private const string CountryWithPostalCodeRestriction = "US";
			}
		}
	}
}
