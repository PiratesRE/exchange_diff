using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ItemLimitExceededException : ItemLevelTransientException
	{
		public ItemLimitExceededException(string limitExceededMsg) : base(CXStrings.ItemLimitExceededExceptionMsg(limitExceededMsg))
		{
			this.limitExceededMsg = limitExceededMsg;
		}

		public ItemLimitExceededException(string limitExceededMsg, Exception innerException) : base(CXStrings.ItemLimitExceededExceptionMsg(limitExceededMsg), innerException)
		{
			this.limitExceededMsg = limitExceededMsg;
		}

		protected ItemLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
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
