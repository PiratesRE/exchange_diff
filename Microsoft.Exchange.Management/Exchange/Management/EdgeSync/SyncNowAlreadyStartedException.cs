using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SyncNowAlreadyStartedException : LocalizedException
	{
		public SyncNowAlreadyStartedException() : base(Strings.SyncNowAlreadyStartedException)
		{
		}

		public SyncNowAlreadyStartedException(Exception innerException) : base(Strings.SyncNowAlreadyStartedException, innerException)
		{
		}

		protected SyncNowAlreadyStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
