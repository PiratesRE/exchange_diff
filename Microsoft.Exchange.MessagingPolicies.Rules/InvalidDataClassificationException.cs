using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MessagingPolicies
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidDataClassificationException : ArgumentException
	{
		public InvalidDataClassificationException() : base(TransportRulesStrings.InvalidDataClassification)
		{
		}

		public InvalidDataClassificationException(Exception innerException) : base(TransportRulesStrings.InvalidDataClassification, innerException)
		{
		}

		protected InvalidDataClassificationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
