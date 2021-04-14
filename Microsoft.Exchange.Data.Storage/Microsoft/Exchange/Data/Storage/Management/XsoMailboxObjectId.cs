using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public abstract class XsoMailboxObjectId : ObjectId, IEquatable<XsoMailboxObjectId>
	{
		public ADObjectId MailboxOwnerId { get; private set; }

		public static bool operator ==(XsoMailboxObjectId operand1, XsoMailboxObjectId operand2)
		{
			return object.Equals(operand1, operand2);
		}

		public static bool operator !=(XsoMailboxObjectId operand1, XsoMailboxObjectId operand2)
		{
			return !object.Equals(operand1, operand2);
		}

		public virtual bool Equals(XsoMailboxObjectId other)
		{
			return !(null == other) && ADObjectId.Equals(this.MailboxOwnerId, other.MailboxOwnerId);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as XsoMailboxObjectId);
		}

		public override int GetHashCode()
		{
			return this.MailboxOwnerId.GetHashCode();
		}

		public override byte[] GetBytes()
		{
			return this.MailboxOwnerId.GetBytes();
		}

		internal XsoMailboxObjectId(ADObjectId mailboxOwnerId)
		{
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			this.MailboxOwnerId = mailboxOwnerId;
		}
	}
}
