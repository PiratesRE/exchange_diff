using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Audio
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidWmaFormatException : LocalizedException
	{
		public InvalidWmaFormatException() : base(NetException.InvalidWmaFormat)
		{
		}

		public InvalidWmaFormatException(Exception innerException) : base(NetException.InvalidWmaFormat, innerException)
		{
		}

		protected InvalidWmaFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
