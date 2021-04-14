using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbOperationAttempedTooSoonException : AmDbOperationException
	{
		public AmDbOperationAttempedTooSoonException(string dbName) : base(ReplayStrings.AmDbOperationAttempedTooSoonException(dbName))
		{
			this.dbName = dbName;
		}

		public AmDbOperationAttempedTooSoonException(string dbName, Exception innerException) : base(ReplayStrings.AmDbOperationAttempedTooSoonException(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected AmDbOperationAttempedTooSoonException(SerializationInfo info, StreamingContext context) : base(info, context)
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
