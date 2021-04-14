using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Not")]
	[Serializable]
	public class NotType : SearchExpressionType, INonLeafSearchExpressionType
	{
		[XmlElement("IsEqualTo", typeof(IsEqualToType))]
		[DataMember(EmitDefaultValue = false)]
		[XmlElement("IsGreaterThanOrEqualTo", typeof(IsGreaterThanOrEqualToType))]
		[XmlElement("SearchExpression", typeof(SearchExpressionType))]
		[XmlElement("Contains", typeof(ContainsExpressionType))]
		[XmlElement("Excludes", typeof(ExcludesType))]
		[XmlElement("Exists", typeof(ExistsType))]
		[XmlElement("IsGreaterThan", typeof(IsGreaterThanType))]
		[XmlElement("And", typeof(AndType))]
		[XmlElement("IsLessThan", typeof(IsLessThanType))]
		[XmlElement("IsLessThanOrEqualTo", typeof(IsLessThanOrEqualToType))]
		[XmlElement("IsNotEqualTo", typeof(IsNotEqualToType))]
		[XmlElement("Not", typeof(NotType))]
		[XmlElement("Or", typeof(OrType))]
		public SearchExpressionType Item { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public SearchExpressionType[] Items
		{
			get
			{
				return new SearchExpressionType[]
				{
					this.Item
				};
			}
			set
			{
				this.Item = value[0];
			}
		}

		internal override string FilterType
		{
			get
			{
				return "Not";
			}
		}
	}
}
