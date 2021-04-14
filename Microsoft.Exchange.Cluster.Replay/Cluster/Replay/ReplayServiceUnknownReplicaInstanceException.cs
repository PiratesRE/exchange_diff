using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceUnknownReplicaInstanceException : TaskServerException
	{
		public ReplayServiceUnknownReplicaInstanceException(string operationName, string db) : base(ReplayStrings.ReplayServiceUnknownReplicaInstanceException(operationName, db))
		{
			this.operationName = operationName;
			this.db = db;
		}

		public ReplayServiceUnknownReplicaInstanceException(string operationName, string db, Exception innerException) : base(ReplayStrings.ReplayServiceUnknownReplicaInstanceException(operationName, db), innerException)
		{
			this.operationName = operationName;
			this.db = db;
		}

		protected ReplayServiceUnknownReplicaInstanceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationName = (string)info.GetValue("operationName", typeof(string));
			this.db = (string)info.GetValue("db", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationName", this.operationName);
			info.AddValue("db", this.db);
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		public string Db
		{
			get
			{
				return this.db;
			}
		}

		private readonly string operationName;

		private readonly string db;
	}
}
