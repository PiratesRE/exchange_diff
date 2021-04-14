using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MailboxStoreIdentity : ObjectId, ISerializable, IEquatable<MailboxStoreIdentity>
	{
		public MailboxStoreIdentity(ADObjectId mailboxOwnerId)
		{
			this.mailboxOwnerId = mailboxOwnerId;
		}

		public MailboxStoreIdentity() : this(null)
		{
		}

		public ADObjectId MailboxOwnerId
		{
			get
			{
				return this.mailboxOwnerId;
			}
			internal set
			{
				this.mailboxOwnerId = value;
			}
		}

		public override byte[] GetBytes()
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			if (this.mailboxOwnerId != null)
			{
				return this.mailboxOwnerId.ToString();
			}
			return null;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MailboxStoreIdentity);
		}

		public virtual bool Equals(MailboxStoreIdentity other)
		{
			return other != null && this.mailboxOwnerId != null && this.mailboxOwnerId.Equals(other.mailboxOwnerId);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		public override int GetHashCode()
		{
			if (this.mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			return this.mailboxOwnerId.GetHashCode();
		}

		private ADObjectId mailboxOwnerId;
	}
}
