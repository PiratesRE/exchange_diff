using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "IsEqualTo")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class IsEqualToType : TwoOperandExpressionType
	{
		internal override string FilterType
		{
			get
			{
				return "IsEqualTo";
			}
		}
	}
}
