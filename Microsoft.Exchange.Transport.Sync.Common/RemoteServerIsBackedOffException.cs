using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RemoteServerIsBackedOffException : TransientException
	{
		public RemoteServerIsBackedOffException() : base(Strings.RemoteServerIsBackedOffException)
		{
		}

		public RemoteServerIsBackedOffException(Exception innerException) : base(Strings.RemoteServerIsBackedOffException, innerException)
		{
		}

		protected RemoteServerIsBackedOffException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
