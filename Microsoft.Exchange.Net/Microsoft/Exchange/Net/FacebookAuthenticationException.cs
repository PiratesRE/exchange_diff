using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FacebookAuthenticationException : LocalizedException
	{
		public FacebookAuthenticationException(LocalizedString message) : base(message)
		{
		}

		public FacebookAuthenticationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FacebookAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
