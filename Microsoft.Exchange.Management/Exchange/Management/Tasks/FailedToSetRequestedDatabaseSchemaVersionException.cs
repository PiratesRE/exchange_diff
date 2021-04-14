using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToSetRequestedDatabaseSchemaVersionException : LocalizedException
	{
		public FailedToSetRequestedDatabaseSchemaVersionException(string database) : base(Strings.FailedToSetRequestedDatabaseSchemaVersion(database))
		{
			this.database = database;
		}

		public FailedToSetRequestedDatabaseSchemaVersionException(string database, Exception innerException) : base(Strings.FailedToSetRequestedDatabaseSchemaVersion(database), innerException)
		{
			this.database = database;
		}

		protected FailedToSetRequestedDatabaseSchemaVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
