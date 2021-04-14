using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UrlNotFoundOrNoAccessException : AuthorizationException
	{
		public UrlNotFoundOrNoAccessException(LocalizedString message) : base(message)
		{
		}

		public UrlNotFoundOrNoAccessException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected UrlNotFoundOrNoAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
