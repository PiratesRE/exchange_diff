using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmTransientException : AmServerTransientException
	{
		public AmTransientException(string errMessage) : base(ReplayStrings.AmTransientException(errMessage))
		{
			this.errMessage = errMessage;
		}

		public AmTransientException(string errMessage, Exception innerException) : base(ReplayStrings.AmTransientException(errMessage), innerException)
		{
			this.errMessage = errMessage;
		}

		protected AmTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
