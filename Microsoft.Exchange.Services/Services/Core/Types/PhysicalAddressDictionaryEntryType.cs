using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PhysicalAddressDictionaryEntryType
	{
		public PhysicalAddressDictionaryEntryType()
		{
		}

		public PhysicalAddressDictionaryEntryType(string street, string city, string state, string countryOrRegion, string postalCode)
		{
			this.Street = street;
			this.City = city;
			this.State = state;
			this.CountryOrRegion = countryOrRegion;
			this.PostalCode = postalCode;
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string Street { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string City { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string State { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string CountryOrRegion { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public string PostalCode { get; set; }

		[XmlAttribute]
		[IgnoreDataMember]
		public PhysicalAddressKeyType Key { get; set; }

		[DataMember(Name = "Key", EmitDefaultValue = false, Order = 0)]
		[XmlIgnore]
		public string KeyString
		{
			get
			{
				return EnumUtilities.ToString<PhysicalAddressKeyType>(this.Key);
			}
			set
			{
				this.Key = EnumUtilities.Parse<PhysicalAddressKeyType>(value);
			}
		}

		internal bool IsSet()
		{
			return !string.IsNullOrEmpty(this.Street) || !string.IsNullOrEmpty(this.City) || !string.IsNullOrEmpty(this.State) || !string.IsNullOrEmpty(this.PostalCode) || !string.IsNullOrEmpty(this.CountryOrRegion);
		}
	}
}
