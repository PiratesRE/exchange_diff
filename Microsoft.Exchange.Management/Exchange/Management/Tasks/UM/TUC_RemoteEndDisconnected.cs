using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TUC_RemoteEndDisconnected : LocalizedException
	{
		public TUC_RemoteEndDisconnected() : base(Strings.RemoteEndDisconnected)
		{
		}

		public TUC_RemoteEndDisconnected(Exception innerException) : base(Strings.RemoteEndDisconnected, innerException)
		{
		}

		protected TUC_RemoteEndDisconnected(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
