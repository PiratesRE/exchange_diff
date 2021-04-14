using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3DisabledResponseException : LocalizedException
	{
		public Pop3DisabledResponseException() : base(CXStrings.Pop3DisabledResponseMsg)
		{
		}

		public Pop3DisabledResponseException(Exception innerException) : base(CXStrings.Pop3DisabledResponseMsg, innerException)
		{
		}

		protected Pop3DisabledResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
