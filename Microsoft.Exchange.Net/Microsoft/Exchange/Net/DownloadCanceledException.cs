using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DownloadCanceledException : TransientException
	{
		public DownloadCanceledException() : base(HttpStrings.DownloadCanceledException)
		{
		}

		public DownloadCanceledException(Exception innerException) : base(HttpStrings.DownloadCanceledException, innerException)
		{
		}

		protected DownloadCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
