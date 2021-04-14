using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidServerResponseException : LocalizedException
	{
		public InvalidServerResponseException() : base(Strings.InvalidServerResponseException)
		{
		}

		public InvalidServerResponseException(Exception innerException) : base(Strings.InvalidServerResponseException, innerException)
		{
		}

		protected InvalidServerResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
