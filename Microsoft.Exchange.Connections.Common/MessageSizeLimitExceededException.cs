using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MessageSizeLimitExceededException : ItemLevelTransientException
	{
		public MessageSizeLimitExceededException(string limitExceededMsg) : base(CXStrings.MessageSizeLimitExceededError(limitExceededMsg))
		{
			this.limitExceededMsg = limitExceededMsg;
		}

		public MessageSizeLimitExceededException(string limitExceededMsg, Exception innerException) : base(CXStrings.MessageSizeLimitExceededError(limitExceededMsg), innerException)
		{
			this.limitExceededMsg = limitExceededMsg;
		}

		protected MessageSizeLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.limitExceededMsg = (string)info.GetValue("limitExceededMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("limitExceededMsg", this.limitExceededMsg);
		}

		public string LimitExceededMsg
		{
			get
			{
				return this.limitExceededMsg;
			}
		}

		private readonly string limitExceededMsg;
	}
}
