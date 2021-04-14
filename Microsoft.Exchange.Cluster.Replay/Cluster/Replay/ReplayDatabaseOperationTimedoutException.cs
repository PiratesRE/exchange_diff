using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayDatabaseOperationTimedoutException : TaskServerException
	{
		public ReplayDatabaseOperationTimedoutException(string operationName, string db, TimeSpan timeout) : base(ReplayStrings.ReplayDatabaseOperationTimedoutException(operationName, db, timeout))
		{
			this.operationName = operationName;
			this.db = db;
			this.timeout = timeout;
		}

		public ReplayDatabaseOperationTimedoutException(string operationName, string db, TimeSpan timeout, Exception innerException) : base(ReplayStrings.ReplayDatabaseOperationTimedoutException(operationName, db, timeout), innerException)
		{
			this.operationName = operationName;
			this.db = db;
			this.timeout = timeout;
		}

		protected ReplayDatabaseOperationTimedoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationName = (string)info.GetValue("operationName", typeof(string));
			this.db = (string)info.GetValue("db", typeof(string));
			this.timeout = (TimeSpan)info.GetValue("timeout", typeof(TimeSpan));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationName", this.operationName);
			info.AddValue("db", this.db);
			info.AddValue("timeout", this.timeout);
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

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		private readonly string operationName;

		private readonly string db;

		private readonly TimeSpan timeout;
	}
}
