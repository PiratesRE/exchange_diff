using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3TransientLoginDelayedAuthErrorException : LocalizedException
	{
		public Pop3TransientLoginDelayedAuthErrorException() : base(Strings.Pop3TransientLoginDelayedAuthErrorException)
		{
		}

		public Pop3TransientLoginDelayedAuthErrorException(Exception innerException) : base(Strings.Pop3TransientLoginDelayedAuthErrorException, innerException)
		{
		}

		protected Pop3TransientLoginDelayedAuthErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
