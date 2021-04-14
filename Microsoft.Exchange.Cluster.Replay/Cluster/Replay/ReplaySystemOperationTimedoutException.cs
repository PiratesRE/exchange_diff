using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplaySystemOperationTimedoutException : TaskServerException
	{
		public ReplaySystemOperationTimedoutException(string operationName, TimeSpan timeout) : base(ReplayStrings.ReplaySystemOperationTimedoutException(operationName, timeout))
		{
			this.operationName = operationName;
			this.timeout = timeout;
		}

		public ReplaySystemOperationTimedoutException(string operationName, TimeSpan timeout, Exception innerException) : base(ReplayStrings.ReplaySystemOperationTimedoutException(operationName, timeout), innerException)
		{
			this.operationName = operationName;
			this.timeout = timeout;
		}

		protected ReplaySystemOperationTimedoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationName = (string)info.GetValue("operationName", typeof(string));
			this.timeout = (TimeSpan)info.GetValue("timeout", typeof(TimeSpan));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationName", this.operationName);
			info.AddValue("timeout", this.timeout);
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
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

		private readonly TimeSpan timeout;
	}
}
