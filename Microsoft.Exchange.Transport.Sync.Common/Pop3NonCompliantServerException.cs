using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class Pop3NonCompliantServerException : LocalizedException
	{
		public Pop3NonCompliantServerException() : base(Strings.Pop3NonCompliantServerException)
		{
		}

		public Pop3NonCompliantServerException(Exception innerException) : base(Strings.Pop3NonCompliantServerException, innerException)
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
