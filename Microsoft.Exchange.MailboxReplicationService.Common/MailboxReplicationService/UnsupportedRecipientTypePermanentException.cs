using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedRecipientTypePermanentException : MailboxReplicationPermanentException
	{
		public UnsupportedRecipientTypePermanentException(string recipient, string recipientType) : base(MrsStrings.UnsupportedRecipientType(recipient, recipientType))
		{
			this.recipient = recipient;
			this.recipientType = recipientType;
		}

		public UnsupportedRecipientTypePermanentException(string recipient, string recipientType, Exception innerException) : base(MrsStrings.UnsupportedRecipientType(recipient, recipientType), innerException)
		{
			this.recipient = recipient;
			this.recipientType = recipientType;
		}

		protected UnsupportedRecipientTypePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipient = (string)info.GetValue("recipient", typeof(string));
			this.recipientType = (string)info.GetValue("recipientType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipient", this.recipient);
			info.AddValue("recipientType", this.recipientType);
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		public string RecipientType
		{
			get
			{
				return this.recipientType;
			}
		}

		private readonly string recipient;

		private readonly string recipientType;
	}
}
