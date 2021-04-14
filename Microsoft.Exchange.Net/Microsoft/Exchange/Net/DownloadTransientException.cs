using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DownloadTransientException : TransientException
	{
		public DownloadTransientException() : base(HttpStrings.DownloadTransientException)
		{
		}

		public DownloadTransientException(Exception innerException) : base(HttpStrings.DownloadTransientException, innerException)
		{
		}

		protected DownloadTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
