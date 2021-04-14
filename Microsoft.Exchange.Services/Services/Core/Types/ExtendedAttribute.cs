using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "ExtendedAttribute", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ExtendedAttributeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ExtendedAttribute
	{
		public ExtendedAttribute()
		{
		}

		public ExtendedAttribute(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}

		[DataMember(Name = "Name", IsRequired = false, Order = 0)]
		[XmlElement("Name")]
		public string Name { get; set; }

		[DataMember(Name = "Value", IsRequired = false, Order = 1)]
		[XmlElement("Value")]
		public string Value { get; set; }
	}
}
