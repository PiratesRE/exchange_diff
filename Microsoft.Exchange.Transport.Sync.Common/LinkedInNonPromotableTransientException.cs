using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class LinkedInNonPromotableTransientException : NonPromotableTransientException
	{
		public LinkedInNonPromotableTransientException() : base(Strings.LinkedInNonPromotableTransientException)
		{
		}

		public LinkedInNonPromotableTransientException(Exception innerException) : base(Strings.LinkedInNonPromotableTransientException, innerException)
		{
		}

		protected LinkedInNonPromotableTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
