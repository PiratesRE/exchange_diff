using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RecipientMissingLegDNPermanentException : MailboxReplicationPermanentException
	{
		public RecipientMissingLegDNPermanentException(string recipient) : base(MrsStrings.RecipientMissingLegDN(recipient))
		{
			this.recipient = recipient;
		}

		public RecipientMissingLegDNPermanentException(string recipient, Exception innerException) : base(MrsStrings.RecipientMissingLegDN(recipient), innerException)
		{
			this.recipient = recipient;
		}

		protected RecipientMissingLegDNPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.recipient = (string)info.GetValue("recipient", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("recipient", this.recipient);
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		private readonly string recipient;
	}
}
