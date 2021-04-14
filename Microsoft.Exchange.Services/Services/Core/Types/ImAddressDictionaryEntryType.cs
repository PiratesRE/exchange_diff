using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ImAddressDictionaryEntryType
	{
		public ImAddressDictionaryEntryType()
		{
		}

		public ImAddressDictionaryEntryType(ImAddressKeyType key, string value)
		{
			this.Key = key;
			this.Value = value;
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public ImAddressKeyType Key { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Key", EmitDefaultValue = false, Order = 0)]
		public string KeyString
		{
			get
			{
				return EnumUtilities.ToString<ImAddressKeyType>(this.Key);
			}
			set
			{
				this.Key = EnumUtilities.Parse<ImAddressKeyType>(value);
			}
		}

		[DataMember(Name = "ImAddress", EmitDefaultValue = false, Order = 1)]
		[XmlText]
		public string Value { get; set; }
	}
}
