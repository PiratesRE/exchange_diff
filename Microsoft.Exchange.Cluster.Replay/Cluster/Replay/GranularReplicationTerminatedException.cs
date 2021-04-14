﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GranularReplicationTerminatedException : TransientException
	{
		public GranularReplicationTerminatedException(string reason) : base(ReplayStrings.GranularReplicationTerminated(reason))
		{
			this.reason = reason;
		}

		public GranularReplicationTerminatedException(string reason, Exception innerException) : base(ReplayStrings.GranularReplicationTerminated(reason), innerException)
		{
			this.reason = reason;
		}

		protected GranularReplicationTerminatedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
