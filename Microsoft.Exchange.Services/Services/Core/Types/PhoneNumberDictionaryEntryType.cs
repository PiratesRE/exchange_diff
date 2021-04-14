using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PhoneNumberDictionaryEntryType
	{
		public PhoneNumberDictionaryEntryType()
		{
		}

		public PhoneNumberDictionaryEntryType(PhoneNumberKeyType key, string value)
		{
			this.Key = key;
			this.Value = value;
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public PhoneNumberKeyType Key { get; set; }

		[DataMember(Name = "Key", EmitDefaultValue = false, Order = 0)]
		[XmlIgnore]
		public string KeyString
		{
			get
			{
				return EnumUtilities.ToString<PhoneNumberKeyType>(this.Key);
			}
			set
			{
				this.Key = EnumUtilities.Parse<PhoneNumberKeyType>(value);
			}
		}

		[XmlText]
		[DataMember(Name = "PhoneNumber", EmitDefaultValue = false, Order = 1)]
		public string Value { get; set; }
	}
}
