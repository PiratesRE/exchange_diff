using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CollectionRule : ActivationRule
	{
		public CollectionRule(string mode, ActivationRule[] rules) : base("Collection")
		{
			this.Mode = mode;
			this.Rules = rules;
		}

		[DataMember]
		public string Mode { get; set; }

		[DataMember]
		public ActivationRule[] Rules { get; set; }
	}
}
