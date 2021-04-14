using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AppPasswordLoginException : AuthorizationException
	{
		public AppPasswordLoginException(LocalizedString message) : base(message)
		{
		}

		public AppPasswordLoginException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AppPasswordLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
