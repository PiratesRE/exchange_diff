using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorDatabaseWrongVersion : LocalizedException
	{
		public ErrorDatabaseWrongVersion(string dbName) : base(Strings.ErrorDatabaseWrongVersion(dbName))
		{
			this.dbName = dbName;
		}

		public ErrorDatabaseWrongVersion(string dbName, Exception innerException) : base(Strings.ErrorDatabaseWrongVersion(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected ErrorDatabaseWrongVersion(SerializationInfo info, StreamingContext context) : base(info, context)
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
