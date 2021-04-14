using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PoisonousRemoteServerException : LocalizedException
	{
		public PoisonousRemoteServerException() : base(Strings.PoisonousRemoteServerException)
		{
		}

		public PoisonousRemoteServerException(Exception innerException) : base(Strings.PoisonousRemoteServerException, innerException)
		{
		}

		protected PoisonousRemoteServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
