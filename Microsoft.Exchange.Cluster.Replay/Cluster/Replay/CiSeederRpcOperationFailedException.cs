using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CiSeederRpcOperationFailedException : SeederServerException
	{
		public CiSeederRpcOperationFailedException(string errMessage) : base(ReplayStrings.CiSeederRpcOperationFailedException(errMessage))
		{
			this.errMessage = errMessage;
		}

		public CiSeederRpcOperationFailedException(string errMessage, Exception innerException) : base(ReplayStrings.CiSeederRpcOperationFailedException(errMessage), innerException)
		{
			this.errMessage = errMessage;
		}

		protected CiSeederRpcOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
