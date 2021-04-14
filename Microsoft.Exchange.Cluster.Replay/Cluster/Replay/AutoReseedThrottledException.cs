using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoReseedThrottledException : AutoReseedException
	{
		public AutoReseedThrottledException(string databaseName, string serverName, string throttlingInterval) : base(ReplayStrings.AutoReseedThrottledException(databaseName, serverName, throttlingInterval))
		{
			this.databaseName = databaseName;
			this.serverName = serverName;
			this.throttlingInterval = throttlingInterval;
		}

		public AutoReseedThrottledException(string databaseName, string serverName, string throttlingInterval, Exception innerException) : base(ReplayStrings.AutoReseedThrottledException(databaseName, serverName, throttlingInterval), innerException)
		{
			this.databaseName = databaseName;
			this.serverName = serverName;
			this.throttlingInterval = throttlingInterval;
		}

		protected AutoReseedThrottledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.throttlingInterval = (string)info.GetValue("throttlingInterval", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("serverName", this.serverName);
			info.AddValue("throttlingInterval", this.throttlingInterval);
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string ThrottlingInterval
		{
			get
			{
				return this.throttlingInterval;
			}
		}

		private readonly string databaseName;

		private readonly string serverName;

		private readonly string throttlingInterval;
	}
}
