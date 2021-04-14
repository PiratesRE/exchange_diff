using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasObjectNotFoundException : MailboxReplicationPermanentException
	{
		public EasObjectNotFoundException(string entryId) : base(MrsStrings.EasObjectNotFound(entryId))
		{
			this.entryId = entryId;
		}

		public EasObjectNotFoundException(string entryId, Exception innerException) : base(MrsStrings.EasObjectNotFound(entryId), innerException)
		{
			this.entryId = entryId;
		}

		protected EasObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
