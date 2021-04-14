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
	internal class SqlServerUnreachableException : LocalizedException
	{
		public SqlServerUnreachableException(string connectionString) : base(MigrationMonitorStrings.ErrorSqlServerUnreachableException(connectionString))
		{
			this.connectionString = connectionString;
		}

		public SqlServerUnreachableException(string connectionString, Exception innerException) : base(MigrationMonitorStrings.ErrorSqlServerUnreachableException(connectionString), innerException)
		{
			this.connectionString = connectionString;
		}

		protected SqlServerUnreachableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.connectionString = (string)info.GetValue("connectionString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("connectionString", this.connectionString);
		}

		public string ConnectionString
		{
			get
			{
				return this.connectionString;
			}
		}

		private readonly string connectionString;
	}
}
