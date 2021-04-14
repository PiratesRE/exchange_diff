using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindDCHavingUmmUpdateTransientException : MailboxReplicationTransientException
	{
		public CouldNotFindDCHavingUmmUpdateTransientException(Guid expectedDb, string recipient) : base(MrsStrings.CouldNotFindDcHavingUmmUpdateError(expectedDb, recipient))
		{
			this.expectedDb = expectedDb;
			this.recipient = recipient;
		}

		public CouldNotFindDCHavingUmmUpdateTransientException(Guid expectedDb, string recipient, Exception innerException) : base(MrsStrings.CouldNotFindDcHavingUmmUpdateError(expectedDb, recipient), innerException)
		{
			this.expectedDb = expectedDb;
			this.recipient = recipient;
		}

		protected CouldNotFindDCHavingUmmUpdateTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.expectedDb = (Guid)info.GetValue("expectedDb", typeof(Guid));
			this.recipient = (string)info.GetValue("recipient", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("expectedDb", this.expectedDb);
			info.AddValue("recipient", this.recipient);
		}

		public Guid ExpectedDb
		{
			get
			{
				return this.expectedDb;
			}
		}

		public string Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		private readonly Guid expectedDb;

		private readonly string recipient;
	}
}
