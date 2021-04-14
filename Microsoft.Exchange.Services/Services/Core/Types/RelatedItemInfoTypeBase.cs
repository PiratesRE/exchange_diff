using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "RelatedItemInfoTypeBase")]
	[Serializable]
	public abstract class RelatedItemInfoTypeBase
	{
		protected RelatedItemInfoTypeBase(ItemId itemId, SingleRecipientType from, string preview)
		{
			this.From = from;
			this.Preview = preview;
			this.ItemId = itemId;
		}

		[DataMember(EmitDefaultValue = false, Order = 1)]
		public SingleRecipientType From { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string Preview { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public ItemId ItemId { get; set; }
	}
}
