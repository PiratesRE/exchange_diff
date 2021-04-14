using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidOrgCapabilityParameterException : LocalizedException
	{
		public InvalidOrgCapabilityParameterException() : base(Strings.InvalidOrgCapabilityParameter)
		{
		}

		public InvalidOrgCapabilityParameterException(Exception innerException) : base(Strings.InvalidOrgCapabilityParameter, innerException)
		{
		}

		protected InvalidOrgCapabilityParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
