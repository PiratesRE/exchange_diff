using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Contains")]
	[Serializable]
	public class ContainsExpressionType : SearchExpressionType
	{
		[XmlElement("FieldURI", typeof(PropertyUri))]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlElement("ExtendedFieldURI", typeof(ExtendedPropertyUri))]
		[XmlElement("IndexedFieldURI", typeof(DictionaryPropertyUri))]
		public PropertyPath Item { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public ConstantValueType Constant { get; set; }

		[IgnoreDataMember]
		[XmlAttribute]
		public ContainmentModeType ContainmentMode { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool ContainmentModeSpecified { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Name = "ContainmentMode", Order = 0)]
		public string ContainmentModeString
		{
			get
			{
				if (!this.ContainmentModeSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<ContainmentModeType>(this.ContainmentMode);
			}
			set
			{
				this.ContainmentMode = EnumUtilities.Parse<ContainmentModeType>(value);
				this.ContainmentModeSpecified = true;
			}
		}

		[XmlAttribute]
		[IgnoreDataMember]
		public ContainmentComparisonType ContainmentComparison { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool ContainmentComparisonSpecified { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, Name = "ContainmentComparison", Order = 0)]
		public string ContainmentComparisonString
		{
			get
			{
				if (!this.ContainmentComparisonSpecified)
				{
					return null;
				}
				return EnumUtilities.ToString<ContainmentComparisonType>(this.ContainmentComparison);
			}
			set
			{
				this.ContainmentComparison = EnumUtilities.Parse<ContainmentComparisonType>(value);
				this.ContainmentComparisonSpecified = true;
			}
		}

		internal override string FilterType
		{
			get
			{
				return "Contains";
			}
		}
	}
}
