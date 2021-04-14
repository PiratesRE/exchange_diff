using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToUpdateRuleInAdException : ExchangeConfigurationException
	{
		public UnableToUpdateRuleInAdException() : base(TransportRulesStrings.UnableToUpdateRuleInAd)
		{
		}

		public UnableToUpdateRuleInAdException(Exception innerException) : base(TransportRulesStrings.UnableToUpdateRuleInAd, innerException)
		{
		}

		protected UnableToUpdateRuleInAdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
