using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SubscriptionCollection
	{
		[DataMember(IsRequired = true)]
		public Subscription[] Subscriptions { get; set; }

		public override string ToString()
		{
			IEnumerable<string> values = from e in this.Subscriptions
			select e.ToString();
			return string.Join(";", values);
		}
	}
}
