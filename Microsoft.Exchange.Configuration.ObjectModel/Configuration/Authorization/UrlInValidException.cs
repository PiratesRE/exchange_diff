using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UrlInValidException : AuthorizationException
	{
		public UrlInValidException(LocalizedString message) : base(message)
		{
		}

		public UrlInValidException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected UrlInValidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
