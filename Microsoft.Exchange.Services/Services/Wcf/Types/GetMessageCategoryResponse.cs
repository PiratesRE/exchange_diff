using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMessageCategoryResponse : OptionsResponseBase
	{
		public GetMessageCategoryResponse()
		{
			this.MessageCategoryCollection = new MessageCategoryCollection();
		}

		[DataMember(IsRequired = true)]
		public MessageCategoryCollection MessageCategoryCollection { get; set; }

		public override string ToString()
		{
			return string.Format("GetMessageCategoryResponse: {0}", this.MessageCategoryCollection);
		}
	}
}
