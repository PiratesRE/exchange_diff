using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3LeaveOnServerNotPossibleException : LocalizedException
	{
		public Pop3LeaveOnServerNotPossibleException() : base(CXStrings.Pop3LeaveOnServerNotPossibleMsg)
		{
		}

		public Pop3LeaveOnServerNotPossibleException(Exception innerException) : base(CXStrings.Pop3LeaveOnServerNotPossibleMsg, innerException)
		{
		}

		protected Pop3LeaveOnServerNotPossibleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
