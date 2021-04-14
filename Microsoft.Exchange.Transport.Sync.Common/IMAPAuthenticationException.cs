using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class IMAPAuthenticationException : IMAPException
	{
		public IMAPAuthenticationException() : base(Strings.IMAPAuthenticationException)
		{
		}

		public IMAPAuthenticationException(Exception innerException) : base(Strings.IMAPAuthenticationException, innerException)
		{
		}

		protected IMAPAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
