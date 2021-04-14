using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogRepairTransientException : TransientException
	{
		public LogRepairTransientException(string reason) : base(ReplayStrings.LogRepairFailedTransient(reason))
		{
			this.reason = reason;
		}

		public LogRepairTransientException(string reason, Exception innerException) : base(ReplayStrings.LogRepairFailedTransient(reason), innerException)
		{
			this.reason = reason;
		}

		protected LogRepairTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
