using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidComplianceRuleActionException : LocalizedException
	{
		public InvalidComplianceRuleActionException(LocalizedString message) : base(message)
		{
		}

		public InvalidComplianceRuleActionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidComplianceRuleActionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
