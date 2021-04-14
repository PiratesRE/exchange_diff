using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3AuthErrorException : LocalizedException
	{
		public Pop3AuthErrorException() : base(CXStrings.Pop3AuthErrorMsg)
		{
		}

		public Pop3AuthErrorException(Exception innerException) : base(CXStrings.Pop3AuthErrorMsg, innerException)
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
