using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MessageDecompressionFailedException : LocalizedException
	{
		public MessageDecompressionFailedException(string serverId) : base(Strings.MessageDecompressionFailedException(serverId))
		{
			this.serverId = serverId;
		}

		public MessageDecompressionFailedException(string serverId, Exception innerException) : base(Strings.MessageDecompressionFailedException(serverId), innerException)
		{
			this.serverId = serverId;
		}

		protected MessageDecompressionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverId = (string)info.GetValue("serverId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverId", this.serverId);
		}

		public string ServerId
		{
			get
			{
				return this.serverId;
			}
		}

		private readonly string serverId;
	}
}
