using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageTraceTransportRule : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return base.Id;
			}
		}

		public Guid DLPPolicyId
		{
			get
			{
				return (Guid)this[MessageTraceTransportRuleSchema.DLPPolicyIdProp];
			}
			set
			{
				this[MessageTraceTransportRuleSchema.DLPPolicyIdProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MessageTraceTransportRule.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MessageTraceTransportRule.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		private static readonly MessageTraceTransportRuleSchema schema = ObjectSchema.GetInstance<MessageTraceTransportRuleSchema>();

		private static string mostDerivedClass = "MessageTraceTransportRule";
	}
}
