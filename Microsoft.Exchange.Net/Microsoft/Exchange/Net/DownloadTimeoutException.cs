using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DownloadTimeoutException : TransientException
	{
		public DownloadTimeoutException() : base(HttpStrings.DownloadTimeoutException)
		{
		}

		public DownloadTimeoutException(Exception innerException) : base(HttpStrings.DownloadTimeoutException, innerException)
		{
		}

		protected DownloadTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
