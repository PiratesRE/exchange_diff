using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class StoreRestartedException : TransientException
	{
		public StoreRestartedException() : base(Strings.StoreRestartedException)
		{
		}

		public StoreRestartedException(Exception innerException) : base(Strings.StoreRestartedException, innerException)
		{
		}

		protected StoreRestartedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
