using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DownloadPermanentException : LocalizedException
	{
		public DownloadPermanentException() : base(HttpStrings.DownloadPermanentException)
		{
		}

		public DownloadPermanentException(Exception innerException) : base(HttpStrings.DownloadPermanentException, innerException)
		{
		}

		protected DownloadPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
