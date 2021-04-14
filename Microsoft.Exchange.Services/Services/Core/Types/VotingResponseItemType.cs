using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "VotingResponseItem", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class VotingResponseItemType : ResponseObjectType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		[XmlIgnore]
		public string Response { get; set; }
	}
}
