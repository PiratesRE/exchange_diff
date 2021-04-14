using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SqlConnectionStringException : LocalizedException
	{
		public SqlConnectionStringException(string connection) : base(MigrationMonitorStrings.ErrorSqlConnectionString(connection))
		{
			this.connection = connection;
		}

		public SqlConnectionStringException(string connection, Exception innerException) : base(MigrationMonitorStrings.ErrorSqlConnectionString(connection), innerException)
		{
			this.connection = connection;
		}

		protected SqlConnectionStringException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.connection = (string)info.GetValue("connection", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("connection", this.connection);
		}

		public string Connection
		{
			get
			{
				return this.connection;
			}
		}

		private readonly string connection;
	}
}
