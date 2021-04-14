using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplaySystemOperationCancelledException : TaskServerException
	{
		public ReplaySystemOperationCancelledException(string operationName) : base(ReplayStrings.ReplaySystemOperationCancelledException(operationName))
		{
			this.operationName = operationName;
		}

		public ReplaySystemOperationCancelledException(string operationName, Exception innerException) : base(ReplayStrings.ReplaySystemOperationCancelledException(operationName), innerException)
		{
			this.operationName = operationName;
		}

		protected ReplaySystemOperationCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationName = (string)info.GetValue("operationName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationName", this.operationName);
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		private readonly string operationName;
	}
}
