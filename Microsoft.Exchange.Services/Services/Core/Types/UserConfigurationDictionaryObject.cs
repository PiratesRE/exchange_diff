using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UserConfigurationDictionaryObjectType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class UserConfigurationDictionaryObject
	{
		[IgnoreDataMember]
		[XmlElement("Type")]
		public UserConfigurationDictionaryObjectType Type { get; set; }

		[DataMember(Name = "Type", IsRequired = true)]
		[XmlIgnore]
		public string TypeString
		{
			get
			{
				return EnumUtilities.ToString<UserConfigurationDictionaryObjectType>(this.Type);
			}
			set
			{
				this.Type = EnumUtilities.Parse<UserConfigurationDictionaryObjectType>(value);
			}
		}

		[DataMember(IsRequired = true)]
		[XmlElement("Value")]
		public string[] Value { get; set; }
	}
}
