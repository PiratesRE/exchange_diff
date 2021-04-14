using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "PersonaPostalAddress")]
	[XmlType(TypeName = "PersonaPostalAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PostalAddress
	{
		[XmlElement]
		[DataMember(IsRequired = false, Order = 1)]
		public string Street { get; set; }

		[XmlElement]
		[DataMember(IsRequired = false, Order = 2)]
		public string City { get; set; }

		[DataMember(IsRequired = false, Order = 3)]
		[XmlElement]
		public string State { get; set; }

		[DataMember(IsRequired = false, Order = 4)]
		[XmlElement]
		public string Country { get; set; }

		[DataMember(IsRequired = false, Order = 5)]
		[XmlElement]
		public string PostalCode { get; set; }

		[DataMember(IsRequired = false, Order = 6)]
		[XmlElement]
		public string PostOfficeBox { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public PostalAddressType Type { get; set; }

		[XmlElement(ElementName = "Type")]
		[DataMember(Name = "Type", IsRequired = true, Order = 7)]
		public string TypeString
		{
			get
			{
				return this.Type.ToString();
			}
			set
			{
				this.Type = (PostalAddressType)Enum.Parse(typeof(PostalAddressType), value);
			}
		}

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 8)]
		public double? Latitude { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 9)]
		public double? Longitude { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 10)]
		public double? Accuracy { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 11)]
		public double? Altitude { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 12)]
		public double? AltitudeAccuracy { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 13)]
		public string FormattedAddress { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 14)]
		public string LocationUri { get; set; }

		[XmlElement]
		[IgnoreDataMember]
		public LocationSourceType LocationSource { get; set; }

		[DataMember(Name = "LocationSource", IsRequired = true, Order = 15)]
		[XmlIgnore]
		public string LocationSourceString
		{
			get
			{
				return this.LocationSource.ToString();
			}
			set
			{
				this.LocationSource = (LocationSourceType)Enum.Parse(typeof(LocationSourceType), value);
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool LatitudeSpecified
		{
			get
			{
				return this.Latitude != null;
			}
			set
			{
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool LongitudeSpecified
		{
			get
			{
				return this.Longitude != null;
			}
			set
			{
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool AccuracySpecified
		{
			get
			{
				return this.Accuracy != null;
			}
			set
			{
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool AltitudeSpecified
		{
			get
			{
				return this.Altitude != null;
			}
			set
			{
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool AltitudeAccuracySpecified
		{
			get
			{
				return this.AltitudeAccuracy != null;
			}
			set
			{
			}
		}

		public PostalAddress()
		{
		}

		public PostalAddress(string street, string city, string state, string country, string postalCode, string postOfficeBox, string formattedAddress, string locationUri, LocationSourceType locationSource, double? latitude, double? longitude, double? accuracy, double? altitude, double? altitudeAccuracy, PostalAddressType type)
		{
			this.Street = street;
			this.City = city;
			this.State = state;
			this.Country = country;
			this.PostalCode = postalCode;
			this.PostOfficeBox = postOfficeBox;
			this.FormattedAddress = formattedAddress;
			this.LocationUri = locationUri;
			this.LocationSource = locationSource;
			this.Latitude = latitude;
			this.Longitude = longitude;
			this.Accuracy = accuracy;
			this.Altitude = altitude;
			this.AltitudeAccuracy = altitudeAccuracy;
			this.Type = type;
		}
	}
}
