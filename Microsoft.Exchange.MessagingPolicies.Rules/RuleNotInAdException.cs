using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RuleNotInAdException : ExchangeConfigurationException
	{
		public RuleNotInAdException() : base(TransportRulesStrings.RuleNotInAd)
		{
		}

		public RuleNotInAdException(Exception innerException) : base(TransportRulesStrings.RuleNotInAd, innerException)
		{
		}

		protected RuleNotInAdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
