using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayDbOperationWrapperException : ReplayDbOperationException
	{
		public ReplayDbOperationWrapperException(string operationError) : base(ReplayStrings.ReplayDbOperationWrapperException(operationError))
		{
			this.operationError = operationError;
		}

		public ReplayDbOperationWrapperException(string operationError, Exception innerException) : base(ReplayStrings.ReplayDbOperationWrapperException(operationError), innerException)
		{
			this.operationError = operationError;
		}

		protected ReplayDbOperationWrapperException(SerializationInfo info, StreamingContext context) : base(info, context)
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
