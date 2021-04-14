using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayStoreOperationAbortedException : LocalizedException
	{
		public ReplayStoreOperationAbortedException(string operationName) : base(ReplayStrings.ReplayStoreOperationAbortedException(operationName))
		{
			this.operationName = operationName;
		}

		public ReplayStoreOperationAbortedException(string operationName, Exception innerException) : base(ReplayStrings.ReplayStoreOperationAbortedException(operationName), innerException)
		{
			this.operationName = operationName;
		}

		protected ReplayStoreOperationAbortedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
