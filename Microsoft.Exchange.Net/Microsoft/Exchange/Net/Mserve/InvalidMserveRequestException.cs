using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.Mserve
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidMserveRequestException : LocalizedException
	{
		public InvalidMserveRequestException() : base(NetServerException.InvalidMserveRequest)
		{
		}

		public InvalidMserveRequestException(Exception innerException) : base(NetServerException.InvalidMserveRequest, innerException)
		{
		}

		protected InvalidMserveRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
