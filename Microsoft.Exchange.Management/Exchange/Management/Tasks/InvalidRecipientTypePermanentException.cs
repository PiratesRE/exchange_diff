using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidRecipientTypePermanentException : MailboxReplicationPermanentException
	{
		public InvalidRecipientTypePermanentException(string recipientName, string recipientType) : base(Strings.ErrorInvalidRecipientType(recipientName, recipientType))
		{
			this.recipientName = recipientName;
			this.recipientType = recipientType;
		}

		public InvalidRecipientTypePermanentException(string recipientName, string recipientType, Exception innerException) : base(Strings.ErrorInvalidRecipientType(recipientName, recipientType), innerException)
		{
			this.recipientName = recipientName;
			this.recipientType = recipientType;
		}

		protected InvalidRecipientTypePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipientName = (string)info.GetValue("recipientName", typeof(string));
			this.recipientType = (string)info.GetValue("recipientType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipientName", this.recipientName);
			info.AddValue("recipientType", this.recipientType);
		}

		public string RecipientName
		{
			get
			{
				return this.recipientName;
			}
		}

		public string RecipientType
		{
			get
			{
				return this.recipientType;
			}
		}

		private readonly string recipientName;

		private readonly string recipientType;
	}
}
