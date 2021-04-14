using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncomingServerTooLongException : LocalizedException
	{
		public IncomingServerTooLongException() : base(Strings.IncomingServerTooLong)
		{
		}

		public IncomingServerTooLongException(Exception innerException) : base(Strings.IncomingServerTooLong, innerException)
		{
		}

		protected IncomingServerTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
