using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SUC_NotMailboxServer : LocalizedException
	{
		public SUC_NotMailboxServer() : base(Strings.NotMailboxServer)
		{
		}

		public SUC_NotMailboxServer(Exception innerException) : base(Strings.NotMailboxServer, innerException)
		{
		}

		protected SUC_NotMailboxServer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
