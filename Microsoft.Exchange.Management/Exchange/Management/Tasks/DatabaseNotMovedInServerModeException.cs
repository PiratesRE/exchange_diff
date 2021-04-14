using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseNotMovedInServerModeException : LocalizedException
	{
		public DatabaseNotMovedInServerModeException(string database, string sourceServer) : base(Strings.DatabaseNotMovedInServerModeException(database, sourceServer))
		{
			this.database = database;
			this.sourceServer = sourceServer;
		}

		public DatabaseNotMovedInServerModeException(string database, string sourceServer, Exception innerException) : base(Strings.DatabaseNotMovedInServerModeException(database, sourceServer), innerException)
		{
			this.database = database;
			this.sourceServer = sourceServer;
		}

		protected DatabaseNotMovedInServerModeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.sourceServer = (string)info.GetValue("sourceServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("sourceServer", this.sourceServer);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string SourceServer
		{
			get
			{
				return this.sourceServer;
			}
		}

		private readonly string database;

		private readonly string sourceServer;
	}
}
