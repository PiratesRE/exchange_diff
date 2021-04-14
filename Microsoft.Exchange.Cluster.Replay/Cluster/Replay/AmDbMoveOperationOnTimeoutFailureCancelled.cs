using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMoveOperationOnTimeoutFailureCancelled : AmDbActionException
	{
		public AmDbMoveOperationOnTimeoutFailureCancelled(string dbName, string fromServer) : base(ReplayStrings.AmDbMoveOperationOnTimeoutFailureCancelled(dbName, fromServer))
		{
			this.dbName = dbName;
			this.fromServer = fromServer;
		}

		public AmDbMoveOperationOnTimeoutFailureCancelled(string dbName, string fromServer, Exception innerException) : base(ReplayStrings.AmDbMoveOperationOnTimeoutFailureCancelled(dbName, fromServer), innerException)
		{
			this.dbName = dbName;
			this.fromServer = fromServer;
		}

		protected AmDbMoveOperationOnTimeoutFailureCancelled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.fromServer = (string)info.GetValue("fromServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("fromServer", this.fromServer);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string FromServer
		{
			get
			{
				return this.fromServer;
			}
		}

		private readonly string dbName;

		private readonly string fromServer;
	}
}
