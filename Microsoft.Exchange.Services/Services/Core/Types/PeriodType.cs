using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PeriodType
	{
		public PeriodType()
		{
		}

		public PeriodType(string bias, string name, string id)
		{
			this.Bias = bias;
			this.Name = name;
			this.Id = id;
		}

		[XmlAttribute(DataType = "duration")]
		[DataMember(EmitDefaultValue = false, Order = 0)]
		public string Bias { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 0)]
		[XmlAttribute]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 0)]
		[XmlAttribute]
		public string Id { get; set; }
	}
}
