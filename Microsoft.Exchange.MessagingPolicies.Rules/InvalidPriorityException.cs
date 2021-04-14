using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidPriorityException : ExchangeConfigurationException
	{
		public InvalidPriorityException() : base(TransportRulesStrings.InvalidPriority)
		{
		}

		public InvalidPriorityException(Exception innerException) : base(TransportRulesStrings.InvalidPriority, innerException)
		{
		}

		protected InvalidPriorityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
