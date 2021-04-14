using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3MirroredAccountNotPossibleException : LocalizedException
	{
		public Pop3MirroredAccountNotPossibleException() : base(CXStrings.Pop3MirroredAccountNotPossibleMsg)
		{
		}

		public Pop3MirroredAccountNotPossibleException(Exception innerException) : base(CXStrings.Pop3MirroredAccountNotPossibleMsg, innerException)
		{
		}

		protected Pop3MirroredAccountNotPossibleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
