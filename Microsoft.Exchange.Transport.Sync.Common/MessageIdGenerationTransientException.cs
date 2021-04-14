using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MessageIdGenerationTransientException : NonPromotableTransientException
	{
		public MessageIdGenerationTransientException() : base(Strings.MessageIdGenerationTransientException)
		{
		}

		public MessageIdGenerationTransientException(Exception innerException) : base(Strings.MessageIdGenerationTransientException, innerException)
		{
		}

		protected MessageIdGenerationTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
