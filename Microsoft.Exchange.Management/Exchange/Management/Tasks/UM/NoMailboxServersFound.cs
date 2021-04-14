using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoMailboxServersFound : LocalizedException
	{
		public NoMailboxServersFound() : base(Strings.NoMailboxServersFound)
		{
		}

		public NoMailboxServersFound(Exception innerException) : base(Strings.NoMailboxServersFound, innerException)
		{
		}

		protected NoMailboxServersFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
