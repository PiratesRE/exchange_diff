using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3MirroredAccountNotPossibleException : LocalizedException
	{
		public Pop3MirroredAccountNotPossibleException() : base(Strings.Pop3MirroredAccountNotPossibleException)
		{
		}

		public Pop3MirroredAccountNotPossibleException(Exception innerException) : base(Strings.Pop3MirroredAccountNotPossibleException, innerException)
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
