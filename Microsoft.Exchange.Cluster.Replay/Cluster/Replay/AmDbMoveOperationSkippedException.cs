using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMoveOperationSkippedException : AmDbActionException
	{
		public AmDbMoveOperationSkippedException(string dbName, string reason) : base(ReplayStrings.AmDbMoveOperationSkippedException(dbName, reason))
		{
			this.dbName = dbName;
			this.reason = reason;
		}

		public AmDbMoveOperationSkippedException(string dbName, string reason, Exception innerException) : base(ReplayStrings.AmDbMoveOperationSkippedException(dbName, reason), innerException)
		{
			this.dbName = dbName;
			this.reason = reason;
		}

		protected AmDbMoveOperationSkippedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("reason", this.reason);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string dbName;

		private readonly string reason;
	}
}
