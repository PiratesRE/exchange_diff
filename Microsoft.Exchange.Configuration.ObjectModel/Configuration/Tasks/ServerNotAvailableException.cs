using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerNotAvailableException : LocalizedException
	{
		public ServerNotAvailableException() : base(Strings.ServerNotAvailable)
		{
		}

		public ServerNotAvailableException(Exception innerException) : base(Strings.ServerNotAvailable, innerException)
		{
		}

		protected ServerNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
