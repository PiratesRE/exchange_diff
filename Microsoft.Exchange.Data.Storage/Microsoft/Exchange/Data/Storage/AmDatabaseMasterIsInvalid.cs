using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDatabaseMasterIsInvalid : AmServerException
	{
		public AmDatabaseMasterIsInvalid(string dbName) : base(ServerStrings.AmDatabaseMasterIsInvalid(dbName))
		{
			this.dbName = dbName;
		}

		public AmDatabaseMasterIsInvalid(string dbName, Exception innerException) : base(ServerStrings.AmDatabaseMasterIsInvalid(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected AmDatabaseMasterIsInvalid(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		private readonly string dbName;
	}
}
