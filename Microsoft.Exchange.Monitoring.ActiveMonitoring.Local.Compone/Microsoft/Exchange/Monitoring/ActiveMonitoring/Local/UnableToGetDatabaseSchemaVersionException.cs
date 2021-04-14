using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnableToGetDatabaseSchemaVersionException : LocalizedException
	{
		public UnableToGetDatabaseSchemaVersionException(string database) : base(Strings.UnableToGetDatabaseSchemaVersion(database))
		{
			this.database = database;
		}

		public UnableToGetDatabaseSchemaVersionException(string database, Exception innerException) : base(Strings.UnableToGetDatabaseSchemaVersion(database), innerException)
		{
			this.database = database;
		}

		protected UnableToGetDatabaseSchemaVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		private readonly string database;
	}
}
