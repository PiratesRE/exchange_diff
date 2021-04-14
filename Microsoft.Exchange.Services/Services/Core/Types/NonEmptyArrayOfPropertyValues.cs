using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "NonEmptyArrayOfPropertyValuesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class NonEmptyArrayOfPropertyValues
	{
		public NonEmptyArrayOfPropertyValues()
		{
		}

		internal NonEmptyArrayOfPropertyValues(string[] values)
		{
			this.Values = values;
		}

		[XmlElement(ElementName = "Value")]
		[DataMember(Name = "Values")]
		public string[] Values { get; set; }
	}
}
