using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidProductKeyException : LocalizedException
	{
		public InvalidProductKeyException() : base(Strings.InvalidProductKey)
		{
		}

		public InvalidProductKeyException(Exception innerException) : base(Strings.InvalidProductKey, innerException)
		{
		}

		protected InvalidProductKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
