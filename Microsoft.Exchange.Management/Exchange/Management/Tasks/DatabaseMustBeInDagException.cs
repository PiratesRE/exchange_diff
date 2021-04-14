using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseMustBeInDagException : LocalizedException
	{
		public DatabaseMustBeInDagException(string dbName) : base(Strings.DatabaseMustBeInDagException(dbName))
		{
			this.dbName = dbName;
		}

		public DatabaseMustBeInDagException(string dbName, Exception innerException) : base(Strings.DatabaseMustBeInDagException(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected DatabaseMustBeInDagException(SerializationInfo info, StreamingContext context) : base(info, context)
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
