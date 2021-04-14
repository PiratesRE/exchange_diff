using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoReseedPrereqFailedException : AutoReseedException
	{
		public AutoReseedPrereqFailedException(string databaseName, string serverName, string error) : base(ReplayStrings.AutoReseedPrereqFailedException(databaseName, serverName, error))
		{
			this.databaseName = databaseName;
			this.serverName = serverName;
			this.error = error;
		}

		public AutoReseedPrereqFailedException(string databaseName, string serverName, string error, Exception innerException) : base(ReplayStrings.AutoReseedPrereqFailedException(databaseName, serverName, error), innerException)
		{
			this.databaseName = databaseName;
			this.serverName = serverName;
			this.error = error;
		}

		protected AutoReseedPrereqFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("serverName", this.serverName);
			info.AddValue("error", this.error);
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

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string databaseName;

		private readonly string serverName;

		private readonly string error;
	}
}
