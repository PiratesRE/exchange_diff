using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AuthenticationException : LocalizedException
	{
		public AuthenticationException() : base(AuthenticationStrings.AuthenticationException)
		{
		}

		public AuthenticationException(Exception innerException) : base(AuthenticationStrings.AuthenticationException, innerException)
		{
		}

		protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
