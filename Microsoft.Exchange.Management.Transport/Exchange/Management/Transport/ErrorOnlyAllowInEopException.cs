using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorOnlyAllowInEopException : LocalizedException
	{
		public ErrorOnlyAllowInEopException() : base(Strings.ErrorOnlyAllowInEop)
		{
		}

		public ErrorOnlyAllowInEopException(Exception innerException) : base(Strings.ErrorOnlyAllowInEop, innerException)
		{
		}

		protected ErrorOnlyAllowInEopException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
