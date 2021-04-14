using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3AuthErrorException : LocalizedException
	{
		public Pop3AuthErrorException() : base(Strings.Pop3AuthErrorException)
		{
		}

		public Pop3AuthErrorException(Exception innerException) : base(Strings.Pop3AuthErrorException, innerException)
		{
		}

		protected Pop3AuthErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
