using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MTOMParsingFailedException : LocalizedException
	{
		public MTOMParsingFailedException() : base(Strings.MTOMParsingFailedException)
		{
		}

		public MTOMParsingFailedException(Exception innerException) : base(Strings.MTOMParsingFailedException, innerException)
		{
		}

		protected MTOMParsingFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
