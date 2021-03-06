using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskOperationFailedException : DagTaskServerException
	{
		public DagTaskOperationFailedException(string errMessage) : base(ReplayStrings.DagTaskOperationFailedException(errMessage))
		{
			this.errMessage = errMessage;
		}

		public DagTaskOperationFailedException(string errMessage, Exception innerException) : base(ReplayStrings.DagTaskOperationFailedException(errMessage), innerException)
		{
			this.errMessage = errMessage;
		}

		protected DagTaskOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMessage = (string)info.GetValue("errMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMessage", this.errMessage);
		}

		public string ErrMessage
		{
			get
			{
				return this.errMessage;
			}
		}

		private readonly string errMessage;
	}
}
