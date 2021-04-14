using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMoveOperationNotSupportedException : AmDbActionException
	{
		public AmDbMoveOperationNotSupportedException(string dbName) : base(ReplayStrings.AmDbMoveOperationNotSupportedException(dbName))
		{
			this.dbName = dbName;
		}

		public AmDbMoveOperationNotSupportedException(string dbName, Exception innerException) : base(ReplayStrings.AmDbMoveOperationNotSupportedException(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected AmDbMoveOperationNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
