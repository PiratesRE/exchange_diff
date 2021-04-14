using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidALParameterException : LocalizedException
	{
		public InvalidALParameterException() : base(Strings.InvalidALParameterException)
		{
		}

		public InvalidALParameterException(Exception innerException) : base(Strings.InvalidALParameterException, innerException)
		{
		}

		protected InvalidALParameterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
