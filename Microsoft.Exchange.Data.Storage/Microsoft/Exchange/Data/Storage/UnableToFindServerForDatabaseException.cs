using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnableToFindServerForDatabaseException : ObjectNotFoundException
	{
		public UnableToFindServerForDatabaseException(string dbName, string databaseGuid) : base(ServerStrings.ServerForDatabaseNotFound(dbName, databaseGuid))
		{
			this.dbName = dbName;
			this.databaseGuid = databaseGuid;
		}

		public UnableToFindServerForDatabaseException(string dbName, string databaseGuid, Exception innerException) : base(ServerStrings.ServerForDatabaseNotFound(dbName, databaseGuid), innerException)
		{
			this.dbName = dbName;
			this.databaseGuid = databaseGuid;
		}

		protected UnableToFindServerForDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.databaseGuid = (string)info.GetValue("databaseGuid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("databaseGuid", this.databaseGuid);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		private readonly string dbName;

		private readonly string databaseGuid;
	}
}
