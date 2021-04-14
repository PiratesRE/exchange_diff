using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ImapObjectNotFoundException : MailboxReplicationPermanentException
	{
		public ImapObjectNotFoundException(string entryId) : base(MrsStrings.ImapObjectNotFound(entryId))
		{
			this.entryId = entryId;
		}

		public ImapObjectNotFoundException(string entryId, Exception innerException) : base(MrsStrings.ImapObjectNotFound(entryId), innerException)
		{
			this.entryId = entryId;
		}

		protected ImapObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.entryId = (string)info.GetValue("entryId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("entryId", this.entryId);
		}

		public string EntryId
		{
			get
			{
				return this.entryId;
			}
		}

		private readonly string entryId;
	}
}
