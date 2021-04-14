using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "And")]
	[Serializable]
	public class AndType : MultipleOperandBooleanExpressionType
	{
		internal override string FilterType
		{
			get
			{
				return "And";
			}
		}
	}
}
