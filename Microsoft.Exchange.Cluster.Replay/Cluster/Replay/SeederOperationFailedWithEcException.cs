using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederOperationFailedWithEcException : SeederServerException
	{
		public SeederOperationFailedWithEcException(int ec, string errMessage) : base(ReplayStrings.SeederOperationFailedWithEcException(ec, errMessage))
		{
			this.ec = ec;
			this.errMessage = errMessage;
		}

		public SeederOperationFailedWithEcException(int ec, string errMessage, Exception innerException) : base(ReplayStrings.SeederOperationFailedWithEcException(ec, errMessage), innerException)
		{
			this.ec = ec;
			this.errMessage = errMessage;
		}

		protected SeederOperationFailedWithEcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ec = (int)info.GetValue("ec", typeof(int));
			this.errMessage = (string)info.GetValue("errMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ec", this.ec);
			info.AddValue("errMessage", this.errMessage);
		}

		public int Ec
		{
			get
			{
				return this.ec;
			}
		}

		public string ErrMessage
		{
			get
			{
				return this.errMessage;
			}
		}

		private readonly int ec;

		private readonly string errMessage;
	}
}
