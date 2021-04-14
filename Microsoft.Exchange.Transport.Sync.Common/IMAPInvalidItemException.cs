using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class IMAPInvalidItemException : IMAPException
	{
		public IMAPInvalidItemException() : base(Strings.IMAPInvalidItemException)
		{
		}

		public IMAPInvalidItemException(Exception innerException) : base(Strings.IMAPInvalidItemException, innerException)
		{
		}

		protected IMAPInvalidItemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
