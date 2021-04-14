using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AccessTokenNullOrEmptyException : LocalizedException
	{
		public AccessTokenNullOrEmptyException() : base(Strings.AccessTokenNullOrEmpty)
		{
		}

		public AccessTokenNullOrEmptyException(Exception innerException) : base(Strings.AccessTokenNullOrEmpty, innerException)
		{
		}

		protected AccessTokenNullOrEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
