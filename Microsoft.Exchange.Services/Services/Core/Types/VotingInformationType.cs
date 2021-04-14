using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "VotingInformationType")]
	[Serializable]
	public class VotingInformationType
	{
		[XmlArrayItem("VotingOptionData", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public VotingOptionDataType[] UserOptions { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string VotingResponse { get; set; }
	}
}
