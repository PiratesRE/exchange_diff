using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMessageClassificationResponse : OptionsResponseBase
	{
		public GetMessageClassificationResponse()
		{
			this.MessageClassificationCollection = new MessageClassificationCollection();
		}

		[DataMember(IsRequired = true)]
		public MessageClassificationCollection MessageClassificationCollection { get; set; }

		public override string ToString()
		{
			return string.Format("GetMessageClassificationResponse: {0}", this.MessageClassificationCollection);
		}
	}
}
