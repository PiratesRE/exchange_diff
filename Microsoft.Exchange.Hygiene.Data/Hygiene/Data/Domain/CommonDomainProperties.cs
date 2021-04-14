using System;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal class CommonDomainProperties
	{
		public static readonly HygienePropertyDefinition CallerId = new HygienePropertyDefinition("CallerId", typeof(string));

		public static readonly HygienePropertyDefinition TransactionId = new HygienePropertyDefinition("TransactionId", typeof(string));
	}
}
