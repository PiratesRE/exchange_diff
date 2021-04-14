using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(ItemHasAttachmentRule))]
	[KnownType(typeof(ItemHasKnownEntityRule))]
	[KnownType(typeof(ItemIsRule))]
	[KnownType(typeof(ItemHasRegularExpressionMatchRule))]
	[KnownType(typeof(CollectionRule))]
	public abstract class ActivationRule
	{
		public ActivationRule(string type)
		{
			this.Type = type;
		}

		[DataMember]
		public string Type { get; set; }

		internal const string ExchangeJsonNameSpace = "http://schemas.datacontract.org/2004/07/Exchange";
	}
}
