using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDatabaseADException : AmServerException
	{
		public AmDatabaseADException(string dbName, string error) : base(ServerStrings.AmDatabaseADException(dbName, error))
		{
			this.dbName = dbName;
			this.error = error;
		}

		public AmDatabaseADException(string dbName, string error, Exception innerException) : base(ServerStrings.AmDatabaseADException(dbName, error), innerException)
		{
			this.dbName = dbName;
			this.error = error;
		}

		protected AmDatabaseADException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("error", this.error);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string dbName;

		private readonly string error;
	}
}
