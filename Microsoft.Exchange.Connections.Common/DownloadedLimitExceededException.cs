using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DownloadedLimitExceededException : NonPromotableTransientException
	{
		public DownloadedLimitExceededException() : base(CXStrings.DownloadedLimitExceededError)
		{
		}

		public DownloadedLimitExceededException(Exception innerException) : base(CXStrings.DownloadedLimitExceededError, innerException)
		{
		}

		protected DownloadedLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
