using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AuthorizationException : LocalizedException
	{
		public AuthorizationException(LocalizedString message) : base(message)
		{
		}

		public AuthorizationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AuthorizationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
