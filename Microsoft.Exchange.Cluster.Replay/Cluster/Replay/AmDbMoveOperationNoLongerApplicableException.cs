using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMoveOperationNoLongerApplicableException : AmDbActionException
	{
		public AmDbMoveOperationNoLongerApplicableException(string dbName, string fromServer, string activeServer) : base(ReplayStrings.AmDbMoveOperationNoLongerApplicableException(dbName, fromServer, activeServer))
		{
			this.dbName = dbName;
			this.fromServer = fromServer;
			this.activeServer = activeServer;
		}

		public AmDbMoveOperationNoLongerApplicableException(string dbName, string fromServer, string activeServer, Exception innerException) : base(ReplayStrings.AmDbMoveOperationNoLongerApplicableException(dbName, fromServer, activeServer), innerException)
		{
			this.dbName = dbName;
			this.fromServer = fromServer;
			this.activeServer = activeServer;
		}

		protected AmDbMoveOperationNoLongerApplicableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.fromServer = (string)info.GetValue("fromServer", typeof(string));
			this.activeServer = (string)info.GetValue("activeServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("fromServer", this.fromServer);
			info.AddValue("activeServer", this.activeServer);
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

		public string ActiveServer
		{
			get
			{
				return this.activeServer;
			}
		}

		private readonly string dbName;

		private readonly string fromServer;

		private readonly string activeServer;
	}
}
