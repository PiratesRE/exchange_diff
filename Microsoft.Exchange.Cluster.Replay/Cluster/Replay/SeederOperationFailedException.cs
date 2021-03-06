using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederOperationFailedException : SeederServerException
	{
		public SeederOperationFailedException(string errMessage) : base(ReplayStrings.SeederOperationFailedException(errMessage))
		{
			this.errMessage = errMessage;
		}

		public SeederOperationFailedException(string errMessage, Exception innerException) : base(ReplayStrings.SeederOperationFailedException(errMessage), innerException)
		{
			this.errMessage = errMessage;
		}

		protected SeederOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
