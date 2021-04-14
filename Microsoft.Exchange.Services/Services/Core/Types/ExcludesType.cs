using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Excludes")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ExcludesType : SearchExpressionType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlElement("FieldURI", typeof(PropertyUri))]
		[XmlElement("IndexedFieldURI", typeof(DictionaryPropertyUri))]
		[XmlElement("ExtendedFieldURI", typeof(ExtendedPropertyUri))]
		public PropertyPath Item { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public ExcludesValueType Bitmask { get; set; }

		internal override string FilterType
		{
			get
			{
				return "Excludes";
			}
		}
	}
}
