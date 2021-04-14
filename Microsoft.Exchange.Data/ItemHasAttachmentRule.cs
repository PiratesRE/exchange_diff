using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ItemHasAttachmentRule : ActivationRule
	{
		public ItemHasAttachmentRule() : base("ItemHasAttachment")
		{
		}
	}
}
