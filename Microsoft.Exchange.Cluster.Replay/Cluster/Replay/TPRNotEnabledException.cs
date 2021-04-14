using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TPRNotEnabledException : TransientException
	{
		public TPRNotEnabledException() : base(ReplayStrings.TPRNotEnabled)
		{
		}

		public TPRNotEnabledException(Exception innerException) : base(ReplayStrings.TPRNotEnabled, innerException)
		{
		}

		protected TPRNotEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
