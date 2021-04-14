using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OpenClusterTimedoutException : ClusterException
	{
		public OpenClusterTimedoutException(string serverName, int timeoutInSeconds, string context) : base(Strings.OpenClusterTimedoutException(serverName, timeoutInSeconds, context))
		{
			this.serverName = serverName;
			this.timeoutInSeconds = timeoutInSeconds;
			this.context = context;
		}

		public OpenClusterTimedoutException(string serverName, int timeoutInSeconds, string context, Exception innerException) : base(Strings.OpenClusterTimedoutException(serverName, timeoutInSeconds, context), innerException)
		{
			this.serverName = serverName;
			this.timeoutInSeconds = timeoutInSeconds;
			this.context = context;
		}

		protected OpenClusterTimedoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.timeoutInSeconds = (int)info.GetValue("timeoutInSeconds", typeof(int));
			this.context = (string)info.GetValue("context", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("timeoutInSeconds", this.timeoutInSeconds);
			info.AddValue("context", this.context);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public int TimeoutInSeconds
		{
			get
			{
				return this.timeoutInSeconds;
			}
		}

		public string Context
		{
			get
			{
				return this.context;
			}
		}

		private readonly string serverName;

		private readonly int timeoutInSeconds;

		private readonly string context;
	}
}
