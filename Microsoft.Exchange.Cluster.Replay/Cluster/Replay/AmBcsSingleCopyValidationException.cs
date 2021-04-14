using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmBcsSingleCopyValidationException : AmBcsException
	{
		public AmBcsSingleCopyValidationException(string bcsMessage) : base(ReplayStrings.AmBcsSingleCopyValidationException(bcsMessage))
		{
			this.bcsMessage = bcsMessage;
		}

		public AmBcsSingleCopyValidationException(string bcsMessage, Exception innerException) : base(ReplayStrings.AmBcsSingleCopyValidationException(bcsMessage), innerException)
		{
			this.bcsMessage = bcsMessage;
		}

		protected AmBcsSingleCopyValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.bcsMessage = (string)info.GetValue("bcsMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("bcsMessage", this.bcsMessage);
		}

		public string BcsMessage
		{
			get
			{
				return this.bcsMessage;
			}
		}

		private readonly string bcsMessage;
	}
}
