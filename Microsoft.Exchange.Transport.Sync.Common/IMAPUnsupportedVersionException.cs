using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class IMAPUnsupportedVersionException : IMAPException
	{
		public IMAPUnsupportedVersionException() : base(Strings.IMAPUnsupportedVersionException)
		{
		}

		public IMAPUnsupportedVersionException(Exception innerException) : base(Strings.IMAPUnsupportedVersionException, innerException)
		{
		}

		protected IMAPUnsupportedVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
