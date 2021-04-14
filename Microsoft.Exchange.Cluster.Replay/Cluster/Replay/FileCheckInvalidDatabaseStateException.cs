using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckInvalidDatabaseStateException : FileCheckException
	{
		public FileCheckInvalidDatabaseStateException(string database, string state) : base(ReplayStrings.FileCheckInvalidDatabaseState(database, state))
		{
			this.database = database;
			this.state = state;
		}

		public FileCheckInvalidDatabaseStateException(string database, string state, Exception innerException) : base(ReplayStrings.FileCheckInvalidDatabaseState(database, state), innerException)
		{
			this.database = database;
			this.state = state;
		}

		protected FileCheckInvalidDatabaseStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.state = (string)info.GetValue("state", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("state", this.state);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string State
		{
			get
			{
				return this.state;
			}
		}

		private readonly string database;

		private readonly string state;
	}
}
