using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTraceTransportRuleSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition DLPPolicyIdProp = new HygienePropertyDefinition("DLPPolicyId", typeof(Guid));
	}
}
