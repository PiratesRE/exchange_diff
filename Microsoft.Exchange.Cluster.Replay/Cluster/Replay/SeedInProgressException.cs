using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeedInProgressException : SeederServerException
	{
		public SeedInProgressException(string errMessage) : base(ReplayStrings.SeedInProgressException(errMessage))
		{
			this.errMessage = errMessage;
		}

		public SeedInProgressException(string errMessage, Exception innerException) : base(ReplayStrings.SeedInProgressException(errMessage), innerException)
		{
			this.errMessage = errMessage;
		}

		protected SeedInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
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
