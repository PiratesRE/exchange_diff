using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayDatabaseOperationCancelledException : TaskServerException
	{
		public ReplayDatabaseOperationCancelledException(string operationName, string db) : base(ReplayStrings.ReplayDatabaseOperationCancelledException(operationName, db))
		{
			this.operationName = operationName;
			this.db = db;
		}

		public ReplayDatabaseOperationCancelledException(string operationName, string db, Exception innerException) : base(ReplayStrings.ReplayDatabaseOperationCancelledException(operationName, db), innerException)
		{
			this.operationName = operationName;
			this.db = db;
		}

		protected ReplayDatabaseOperationCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
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
