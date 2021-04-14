using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmMountTimeoutException : AmDbActionException
	{
		public AmMountTimeoutException(string dbName, string serverName, int timeoutInSecs) : base(ReplayStrings.AmMountTimeoutError(dbName, serverName, timeoutInSecs))
		{
			this.dbName = dbName;
			this.serverName = serverName;
			this.timeoutInSecs = timeoutInSecs;
		}

		public AmMountTimeoutException(string dbName, string serverName, int timeoutInSecs, Exception innerException) : base(ReplayStrings.AmMountTimeoutError(dbName, serverName, timeoutInSecs), innerException)
		{
			this.dbName = dbName;
			this.serverName = serverName;
			this.timeoutInSecs = timeoutInSecs;
		}

		protected AmMountTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.timeoutInSecs = (int)info.GetValue("timeoutInSecs", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("serverName", this.serverName);
			info.AddValue("timeoutInSecs", this.timeoutInSecs);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public int TimeoutInSecs
		{
			get
			{
				return this.timeoutInSecs;
			}
		}

		private readonly string dbName;

		private readonly string serverName;

		private readonly int timeoutInSecs;
	}
}
