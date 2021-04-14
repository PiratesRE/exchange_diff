using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidFilteringServiceResultException : LocalizedException
	{
		public InvalidFilteringServiceResultException() : base(TransportRulesStrings.InvalidFilteringServiceResult)
		{
		}

		public InvalidFilteringServiceResultException(Exception innerException) : base(TransportRulesStrings.InvalidFilteringServiceResult, innerException)
		{
		}

		protected InvalidFilteringServiceResultException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
