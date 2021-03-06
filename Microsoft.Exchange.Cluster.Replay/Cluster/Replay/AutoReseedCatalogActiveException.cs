using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoReseedCatalogActiveException : AutoReseedException
	{
		public AutoReseedCatalogActiveException(string databaseName, string serverName) : base(ReplayStrings.AutoReseedCatalogActiveException(databaseName, serverName))
		{
			this.databaseName = databaseName;
			this.serverName = serverName;
		}

		public AutoReseedCatalogActiveException(string databaseName, string serverName, Exception innerException) : base(ReplayStrings.AutoReseedCatalogActiveException(databaseName, serverName), innerException)
		{
			this.databaseName = databaseName;
			this.serverName = serverName;
		}

		protected AutoReseedCatalogActiveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("serverName", this.serverName);
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

		private readonly string databaseName;

		private readonly string serverName;
	}
}
