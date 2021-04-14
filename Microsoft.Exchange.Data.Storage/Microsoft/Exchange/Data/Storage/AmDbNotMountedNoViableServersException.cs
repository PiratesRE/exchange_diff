using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbNotMountedNoViableServersException : AmServerException
	{
		public AmDbNotMountedNoViableServersException(string dbName) : base(ServerStrings.AmDbNotMountedNoViableServersException(dbName))
		{
			this.dbName = dbName;
		}

		public AmDbNotMountedNoViableServersException(string dbName, Exception innerException) : base(ServerStrings.AmDbNotMountedNoViableServersException(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected AmDbNotMountedNoViableServersException(SerializationInfo info, StreamingContext context) : base(info, context)
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
