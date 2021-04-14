using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecipientPropertyIsNotWriteablePermanentException : MailboxReplicationPermanentException
	{
		public RecipientPropertyIsNotWriteablePermanentException(string recipient, string propertyName) : base(MrsStrings.RecipientPropertyIsNotWriteable(recipient, propertyName))
		{
			this.recipient = recipient;
			this.propertyName = propertyName;
		}

		public RecipientPropertyIsNotWriteablePermanentException(string recipient, string propertyName, Exception innerException) : base(MrsStrings.RecipientPropertyIsNotWriteable(recipient, propertyName), innerException)
		{
			this.recipient = recipient;
			this.propertyName = propertyName;
		}

		protected RecipientPropertyIsNotWriteablePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipient = (string)info.GetValue("recipient", typeof(string));
			this.propertyName = (string)info.GetValue("propertyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipient", this.recipient);
			info.AddValue("propertyName", this.propertyName);
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string recipient;

		private readonly string propertyName;
	}
}
