using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.Mserve
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MserveCacheServicePermanentException : LocalizedException
	{
		public MserveCacheServicePermanentException(LocalizedString message) : base(message)
		{
		}

		public MserveCacheServicePermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MserveCacheServicePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
