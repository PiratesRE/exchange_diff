using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "AggregateOnType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class AggregateOnType
	{
		public AggregateOnType()
		{
		}

		internal AggregateOnType(PropertyPath aggregationProperty, AggregateType aggregateType)
		{
			this.AggregationProperty = aggregationProperty;
			this.Aggregate = aggregateType;
		}

		[XmlElement("ExtendedFieldURI", typeof(ExtendedPropertyUri), IsNullable = false)]
		[XmlElement("FieldURI", typeof(PropertyUri), IsNullable = false)]
		[DataMember(Name = "AggregationProperty", Order = 1)]
		[XmlElement("IndexedFieldURI", typeof(DictionaryPropertyUri), IsNullable = false)]
		public PropertyPath AggregationProperty { get; set; }

		[IgnoreDataMember]
		[XmlAttribute("Aggregate")]
		public AggregateType Aggregate { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool AggregateSpecified { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Aggregate", EmitDefaultValue = false, Order = 2)]
		public string AggregateString
		{
			get
			{
				return EnumUtilities.ToString<AggregateType>(this.Aggregate);
			}
			set
			{
				this.Aggregate = EnumUtilities.Parse<AggregateType>(value);
				this.AggregateSpecified = true;
			}
		}
	}
}
