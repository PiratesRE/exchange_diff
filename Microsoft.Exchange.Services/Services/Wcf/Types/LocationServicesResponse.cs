using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	internal class LocationServicesResponse : IBingResultSet
	{
		[DataMember]
		public LocationServicesResponse.ResourceSet[] resourceSets { get; set; }

		public IBingResult[] Results
		{
			get
			{
				if (this.resourceSets != null && this.resourceSets.Length > 0 && this.resourceSets[0] != null)
				{
					return this.resourceSets[0].resources;
				}
				return null;
			}
		}

		public IBingError[] Errors
		{
			get
			{
				return null;
			}
		}

		private const string AddressEntityType = "Address";

		[DataContract]
		public class ResourceSet
		{
			[DataMember]
			public LocationServicesResponse.ResourceSet.Resource[] resources { get; set; }

			[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1", Name = "Location")]
			public class Resource : IBingResult
			{
				[DataMember(Name = "name")]
				public string OriginalName { get; set; }

				[DataMember]
				public LocationServicesResponse.ResourceSet.Resource.Point point { get; set; }

				public double Latitude
				{
					get
					{
						return double.Parse(this.point.coordinates[0]);
					}
				}

				public double Longitude
				{
					get
					{
						return double.Parse(this.point.coordinates[1]);
					}
				}

				[DataMember]
				public LocationServicesResponse.ResourceSet.Resource.Address address { get; set; }

				public string Name
				{
					get
					{
						if ("Address".Equals(this.entityType, StringComparison.OrdinalIgnoreCase))
						{
							return this.address.addressLine;
						}
						return this.OriginalName;
					}
				}

				public string StreetAddress
				{
					get
					{
						return this.address.addressLine;
					}
				}

				public string City
				{
					get
					{
						return this.address.locality;
					}
				}

				public string State
				{
					get
					{
						return this.address.adminDistrict;
					}
				}

				public string Country
				{
					get
					{
						return this.address.countryRegion;
					}
				}

				public string PostalCode
				{
					get
					{
						return this.address.postalCode;
					}
				}

				public LocationSource LocationSource
				{
					get
					{
						return LocationSource.LocationServices;
					}
				}

				public string LocationUri
				{
					get
					{
						return this.address.formattedAddress;
					}
				}

				public string PhoneNumber
				{
					get
					{
						return null;
					}
				}

				public string LocalHomePage
				{
					get
					{
						return null;
					}
				}

				public string BusinessHomePage
				{
					get
					{
						return null;
					}
				}

				[DataMember]
				public string entityType { get; set; }

				[DataContract]
				public class Point
				{
					[DataMember]
					public string[] coordinates { get; set; }
				}

				[DataContract]
				public class Address
				{
					[DataMember]
					public string addressLine { get; set; }

					[DataMember]
					public string adminDistrict { get; set; }

					[DataMember]
					public string countryRegion { get; set; }

					[DataMember]
					public string formattedAddress { get; set; }

					[DataMember]
					public string locality { get; set; }

					[DataMember]
					public string postalCode { get; set; }
				}
			}
		}
	}
}
