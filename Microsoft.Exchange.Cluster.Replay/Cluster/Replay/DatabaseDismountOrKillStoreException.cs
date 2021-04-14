using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseDismountOrKillStoreException : TransientException
	{
		public DatabaseDismountOrKillStoreException(string databaseName, string serverName, string errorText) : base(ReplayStrings.DatabaseDismountOrKillStoreException(databaseName, serverName, errorText))
		{
			this.databaseName = databaseName;
			this.serverName = serverName;
			this.errorText = errorText;
		}

		public DatabaseDismountOrKillStoreException(string databaseName, string serverName, string errorText, Exception innerException) : base(ReplayStrings.DatabaseDismountOrKillStoreException(databaseName, serverName, errorText), innerException)
		{
			this.databaseName = databaseName;
			this.serverName = serverName;
			this.errorText = errorText;
		}

		protected DatabaseDismountOrKillStoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.errorText = (string)info.GetValue("errorText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("serverName", this.serverName);
			info.AddValue("errorText", this.errorText);
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

		public string ErrorText
		{
			get
			{
				return this.errorText;
			}
		}

		private readonly string databaseName;

		private readonly string serverName;

		private readonly string errorText;
	}
}
