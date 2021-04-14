using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3NonCompliantServerException : LocalizedException
	{
		public Pop3NonCompliantServerException() : base(CXStrings.Pop3NonCompliantServerMsg)
		{
		}

		public Pop3NonCompliantServerException(Exception innerException) : base(CXStrings.Pop3NonCompliantServerMsg, innerException)
		{
		}

		protected Pop3NonCompliantServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
