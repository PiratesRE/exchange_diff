using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MessageSizeLimitExceededException : LocalizedException
	{
		public MessageSizeLimitExceededException() : base(Strings.MessageSizeLimitExceededException)
		{
		}

		public MessageSizeLimitExceededException(Exception innerException) : base(Strings.MessageSizeLimitExceededException, innerException)
		{
		}

		protected MessageSizeLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
