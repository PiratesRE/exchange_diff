using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailureItemRecoveryException : LocalizedException
	{
		public FailureItemRecoveryException(string dbName, string msg) : base(ReplayStrings.FailureItemRecoveryFailed(dbName, msg))
		{
			this.dbName = dbName;
			this.msg = msg;
		}

		public FailureItemRecoveryException(string dbName, string msg, Exception innerException) : base(ReplayStrings.FailureItemRecoveryFailed(dbName, msg), innerException)
		{
			this.dbName = dbName;
			this.msg = msg;
		}

		protected FailureItemRecoveryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("msg", this.msg);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string dbName;

		private readonly string msg;
	}
}
