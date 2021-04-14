using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxDoesNotExistPermanentException : MailboxReplicationPermanentException
	{
		public MailboxDoesNotExistPermanentException(LocalizedString mbxId) : base(MrsStrings.MailboxDoesNotExist(mbxId))
		{
			this.mbxId = mbxId;
		}

		public MailboxDoesNotExistPermanentException(LocalizedString mbxId, Exception innerException) : base(MrsStrings.MailboxDoesNotExist(mbxId), innerException)
		{
			this.mbxId = mbxId;
		}

		protected MailboxDoesNotExistPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mbxId = (LocalizedString)info.GetValue("mbxId", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mbxId", this.mbxId);
		}

		public LocalizedString MbxId
		{
			get
			{
				return this.mbxId;
			}
		}

		private readonly LocalizedString mbxId;
	}
}
