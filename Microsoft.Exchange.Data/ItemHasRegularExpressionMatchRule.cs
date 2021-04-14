using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ItemHasRegularExpressionMatchRule : ActivationRule
	{
		public ItemHasRegularExpressionMatchRule(string regExName, string regExValue, RegExPropertyName propertyName, bool ignoreCase) : base("ItemHasRegularExpressionMatch")
		{
			this.RegExName = regExName;
			this.RegExValue = regExValue;
			this.PropertyName = propertyName;
			this.IgnoreCase = ignoreCase;
		}

		[DataMember]
		public string RegExName { get; set; }

		[DataMember]
		public string RegExValue { get; set; }

		[DataMember]
		public RegExPropertyName PropertyName { get; set; }

		[DataMember]
		public bool IgnoreCase { get; set; }
	}
}
