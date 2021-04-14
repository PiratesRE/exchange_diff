using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmPreMountCallbackFailedNoReplicaInstanceErrorException : AmServerException
	{
		public AmPreMountCallbackFailedNoReplicaInstanceErrorException(string dbName, string server, string errMsg) : base(ReplayStrings.AmPreMountCallbackFailedNoReplicaInstanceErrorException(dbName, server, errMsg))
		{
			this.dbName = dbName;
			this.server = server;
			this.errMsg = errMsg;
		}

		public AmPreMountCallbackFailedNoReplicaInstanceErrorException(string dbName, string server, string errMsg, Exception innerException) : base(ReplayStrings.AmPreMountCallbackFailedNoReplicaInstanceErrorException(dbName, server, errMsg), innerException)
		{
			this.dbName = dbName;
			this.server = server;
			this.errMsg = errMsg;
		}

		protected AmPreMountCallbackFailedNoReplicaInstanceErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("server", this.server);
			info.AddValue("errMsg", this.errMsg);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string dbName;

		private readonly string server;

		private readonly string errMsg;
	}
}
