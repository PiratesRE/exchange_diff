using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SyncStateSizeExceededException : LocalizedException
	{
		public SyncStateSizeExceededException(LocalizedString message) : base(message)
		{
		}

		public SyncStateSizeExceededException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SyncStateSizeExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
