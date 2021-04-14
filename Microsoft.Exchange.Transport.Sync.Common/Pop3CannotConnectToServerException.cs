using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3CannotConnectToServerException : LocalizedException
	{
		public Pop3CannotConnectToServerException() : base(Strings.Pop3CannotConnectToServerException)
		{
		}

		public Pop3CannotConnectToServerException(Exception innerException) : base(Strings.Pop3CannotConnectToServerException, innerException)
		{
		}

		protected Pop3CannotConnectToServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
