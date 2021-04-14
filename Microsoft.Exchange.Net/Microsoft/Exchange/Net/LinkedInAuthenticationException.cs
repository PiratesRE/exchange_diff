using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class LinkedInAuthenticationException : LocalizedException
	{
		public LinkedInAuthenticationException(LocalizedString message) : base(message)
		{
		}

		public LinkedInAuthenticationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected LinkedInAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
