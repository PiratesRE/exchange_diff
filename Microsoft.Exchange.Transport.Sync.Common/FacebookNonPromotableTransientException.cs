using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FacebookNonPromotableTransientException : NonPromotableTransientException
	{
		public FacebookNonPromotableTransientException() : base(Strings.FacebookNonPromotableTransientException)
		{
		}

		public FacebookNonPromotableTransientException(Exception innerException) : base(Strings.FacebookNonPromotableTransientException, innerException)
		{
		}

		protected FacebookNonPromotableTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
