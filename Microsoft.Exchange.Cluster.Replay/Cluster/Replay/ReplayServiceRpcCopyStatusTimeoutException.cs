using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceRpcCopyStatusTimeoutException : TaskServerException
	{
		public ReplayServiceRpcCopyStatusTimeoutException(string dbCopyName, int timeoutSecs) : base(ReplayStrings.ReplayServiceRpcCopyStatusTimeoutException(dbCopyName, timeoutSecs))
		{
			this.dbCopyName = dbCopyName;
			this.timeoutSecs = timeoutSecs;
		}

		public ReplayServiceRpcCopyStatusTimeoutException(string dbCopyName, int timeoutSecs, Exception innerException) : base(ReplayStrings.ReplayServiceRpcCopyStatusTimeoutException(dbCopyName, timeoutSecs), innerException)
		{
			this.dbCopyName = dbCopyName;
			this.timeoutSecs = timeoutSecs;
		}

		protected ReplayServiceRpcCopyStatusTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopyName = (string)info.GetValue("dbCopyName", typeof(string));
			this.timeoutSecs = (int)info.GetValue("timeoutSecs", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopyName", this.dbCopyName);
			info.AddValue("timeoutSecs", this.timeoutSecs);
		}

		public string DbCopyName
		{
			get
			{
				return this.dbCopyName;
			}
		}

		public int TimeoutSecs
		{
			get
			{
				return this.timeoutSecs;
			}
		}

		private readonly string dbCopyName;

		private readonly int timeoutSecs;
	}
}
