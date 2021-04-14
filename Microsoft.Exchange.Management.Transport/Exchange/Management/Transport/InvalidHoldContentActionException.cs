using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidHoldContentActionException : InvalidComplianceRuleActionException
	{
		public InvalidHoldContentActionException() : base(Strings.InvalidHoldContentAction)
		{
		}

		public InvalidHoldContentActionException(Exception innerException) : base(Strings.InvalidHoldContentAction, innerException)
		{
		}

		protected InvalidHoldContentActionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
