using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3TransientInUseAuthErrorException : LocalizedException
	{
		public Pop3TransientInUseAuthErrorException() : base(CXStrings.Pop3TransientInUseAuthErrorMsg)
		{
		}

		public Pop3TransientInUseAuthErrorException(Exception innerException) : base(CXStrings.Pop3TransientInUseAuthErrorMsg, innerException)
		{
		}

		protected Pop3TransientInUseAuthErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
