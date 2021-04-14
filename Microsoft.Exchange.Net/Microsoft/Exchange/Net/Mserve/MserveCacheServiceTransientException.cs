using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.Mserve
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MserveCacheServiceTransientException : TransientException
	{
		public MserveCacheServiceTransientException(LocalizedString message) : base(message)
		{
		}

		public MserveCacheServiceTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MserveCacheServiceTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
