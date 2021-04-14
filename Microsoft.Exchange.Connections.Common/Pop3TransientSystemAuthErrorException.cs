using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3TransientSystemAuthErrorException : LocalizedException
	{
		public Pop3TransientSystemAuthErrorException() : base(CXStrings.Pop3TransientSystemAuthErrorMsg)
		{
		}

		public Pop3TransientSystemAuthErrorException(Exception innerException) : base(CXStrings.Pop3TransientSystemAuthErrorMsg, innerException)
		{
		}

		protected Pop3TransientSystemAuthErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
