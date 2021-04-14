using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3LeaveOnServerNotPossibleException : LocalizedException
	{
		public Pop3LeaveOnServerNotPossibleException() : base(Strings.Pop3LeaveOnServerNotPossibleException)
		{
		}

		public Pop3LeaveOnServerNotPossibleException(Exception innerException) : base(Strings.Pop3LeaveOnServerNotPossibleException, innerException)
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
