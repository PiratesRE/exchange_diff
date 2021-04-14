using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class IMAPInvalidServerException : IMAPException
	{
		public IMAPInvalidServerException() : base(Strings.IMAPInvalidServerException)
		{
		}

		public IMAPInvalidServerException(Exception innerException) : base(Strings.IMAPInvalidServerException, innerException)
		{
		}

		protected IMAPInvalidServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
