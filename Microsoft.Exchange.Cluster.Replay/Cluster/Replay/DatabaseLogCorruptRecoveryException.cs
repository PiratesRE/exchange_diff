using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseLogCorruptRecoveryException : LocalizedException
	{
		public DatabaseLogCorruptRecoveryException(string dbName, string msg) : base(ReplayStrings.DatabaseLogCorruptRecoveryFailed(dbName, msg))
		{
			this.dbName = dbName;
			this.msg = msg;
		}

		public DatabaseLogCorruptRecoveryException(string dbName, string msg, Exception innerException) : base(ReplayStrings.DatabaseLogCorruptRecoveryFailed(dbName, msg), innerException)
		{
			this.dbName = dbName;
			this.msg = msg;
		}

		protected DatabaseLogCorruptRecoveryException(SerializationInfo info, StreamingContext context) : base(info, context)
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
