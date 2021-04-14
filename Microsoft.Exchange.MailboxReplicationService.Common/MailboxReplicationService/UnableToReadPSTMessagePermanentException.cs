using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToReadPSTMessagePermanentException : MailboxReplicationPermanentException
	{
		public UnableToReadPSTMessagePermanentException(string filePath, uint messageId) : base(MrsStrings.UnableToReadPSTMessage(filePath, messageId))
		{
			this.filePath = filePath;
			this.messageId = messageId;
		}

		public UnableToReadPSTMessagePermanentException(string filePath, uint messageId, Exception innerException) : base(MrsStrings.UnableToReadPSTMessage(filePath, messageId), innerException)
		{
			this.filePath = filePath;
			this.messageId = messageId;
		}

		protected UnableToReadPSTMessagePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filePath = (string)info.GetValue("filePath", typeof(string));
			this.messageId = (uint)info.GetValue("messageId", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filePath", this.filePath);
			info.AddValue("messageId", this.messageId);
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		public uint MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		private readonly string filePath;

		private readonly uint messageId;
	}
}
