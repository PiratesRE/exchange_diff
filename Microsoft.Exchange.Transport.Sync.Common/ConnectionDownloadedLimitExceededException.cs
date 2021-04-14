using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ConnectionDownloadedLimitExceededException : NonPromotableTransientException
	{
		public ConnectionDownloadedLimitExceededException() : base(Strings.ConnectionDownloadedLimitExceededException)
		{
		}

		public ConnectionDownloadedLimitExceededException(Exception innerException) : base(Strings.ConnectionDownloadedLimitExceededException, innerException)
		{
		}

		protected ConnectionDownloadedLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
