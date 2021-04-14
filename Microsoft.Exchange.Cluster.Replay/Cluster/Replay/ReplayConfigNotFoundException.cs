using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayConfigNotFoundException : TransientException
	{
		public ReplayConfigNotFoundException(string dbName, string serverName) : base(ReplayStrings.ReplayConfigNotFoundException(dbName, serverName))
		{
			this.dbName = dbName;
			this.serverName = serverName;
		}

		public ReplayConfigNotFoundException(string dbName, string serverName, Exception innerException) : base(ReplayStrings.ReplayConfigNotFoundException(dbName, serverName), innerException)
		{
			this.dbName = dbName;
			this.serverName = serverName;
		}

		protected ReplayConfigNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("serverName", this.serverName);
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

		private readonly string dbName;

		private readonly string serverName;
	}
}
