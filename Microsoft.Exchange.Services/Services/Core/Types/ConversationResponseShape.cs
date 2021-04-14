using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class ConversationResponseShape : ResponseShape
	{
		public ConversationResponseShape()
		{
		}

		internal ConversationResponseShape(ShapeEnum baseShape, PropertyPath[] additionalProperties) : base(baseShape, additionalProperties)
		{
		}

		[XmlIgnore]
		[DataMember(IsRequired = false)]
		public bool InferenceEnabled { get; set; }
	}
}
