using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedAtReplacingLogFiles : TransientException
	{
		public FailedAtReplacingLogFiles() : base(ReplayStrings.FailedAtReplacingLogFiles)
		{
		}

		public FailedAtReplacingLogFiles(Exception innerException) : base(ReplayStrings.FailedAtReplacingLogFiles, innerException)
		{
		}

		protected FailedAtReplacingLogFiles(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
