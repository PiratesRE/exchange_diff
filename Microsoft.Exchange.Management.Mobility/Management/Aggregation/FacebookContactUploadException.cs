using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FacebookContactUploadException : LocalizedException
	{
		public FacebookContactUploadException(LocalizedString message) : base(message)
		{
		}

		public FacebookContactUploadException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FacebookContactUploadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
