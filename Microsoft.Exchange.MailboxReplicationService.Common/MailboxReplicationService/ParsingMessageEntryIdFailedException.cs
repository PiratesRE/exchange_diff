using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParsingMessageEntryIdFailedException : MailboxReplicationPermanentException
	{
		public ParsingMessageEntryIdFailedException(string messageEntryId) : base(MrsStrings.ParsingMessageEntryIdFailed(messageEntryId))
		{
			this.messageEntryId = messageEntryId;
		}

		public ParsingMessageEntryIdFailedException(string messageEntryId, Exception innerException) : base(MrsStrings.ParsingMessageEntryIdFailed(messageEntryId), innerException)
		{
			this.messageEntryId = messageEntryId;
		}

		protected ParsingMessageEntryIdFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.messageEntryId = (string)info.GetValue("messageEntryId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("messageEntryId", this.messageEntryId);
		}

		public string MessageEntryId
		{
			get
			{
				return this.messageEntryId;
			}
		}

		private readonly string messageEntryId;
	}
}
