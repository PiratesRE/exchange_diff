using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayDbOperationWrapperTransientException : ReplayDbOperationTransientException
	{
		public ReplayDbOperationWrapperTransientException(string operationError) : base(ReplayStrings.ReplayDbOperationWrapperTransientException(operationError))
		{
			this.operationError = operationError;
		}

		public ReplayDbOperationWrapperTransientException(string operationError, Exception innerException) : base(ReplayStrings.ReplayDbOperationWrapperTransientException(operationError), innerException)
		{
			this.operationError = operationError;
		}

		protected ReplayDbOperationWrapperTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationError = (string)info.GetValue("operationError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationError", this.operationError);
		}

		public string OperationError
		{
			get
			{
				return this.operationError;
			}
		}

		private readonly string operationError;
	}
}
