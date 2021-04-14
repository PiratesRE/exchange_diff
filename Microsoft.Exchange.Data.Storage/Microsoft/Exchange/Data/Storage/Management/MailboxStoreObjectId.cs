using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxStoreObjectId : XsoMailboxObjectId
	{
		internal StoreObjectId StoreObjectId
		{
			get
			{
				return this.storeObjectId;
			}
		}

		internal MailboxStoreObjectId(ADObjectId mailboxOwnerId, StoreObjectId storeObjectId) : base(mailboxOwnerId)
		{
			this.storeObjectId = storeObjectId;
		}

		public override byte[] GetBytes()
		{
			byte[] bytes = this.StoreObjectId.GetBytes();
			byte[] bytes2 = base.MailboxOwnerId.GetBytes();
			byte[] array = new byte[bytes.Length + bytes2.Length + 2];
			int num = 0;
			array[num++] = (byte)(bytes2.Length & 255);
			array[num++] = (byte)(bytes2.Length >> 8 & 255);
			Array.Copy(bytes2, 0, array, num, bytes2.Length);
			num += bytes2.Length;
			Array.Copy(bytes, 0, array, num, bytes.Length);
			return array;
		}

		public override int GetHashCode()
		{
			return base.MailboxOwnerId.GetHashCode() ^ this.StoreObjectId.GetHashCode();
		}

		public override bool Equals(XsoMailboxObjectId other)
		{
			MailboxStoreObjectId mailboxStoreObjectId = other as MailboxStoreObjectId;
			return !(null == mailboxStoreObjectId) && ADObjectId.Equals(base.MailboxOwnerId, other.MailboxOwnerId) && object.Equals(this.storeObjectId, mailboxStoreObjectId.StoreObjectId);
		}

		public override string ToString()
		{
			return string.Format("{0}{1}{2}", base.MailboxOwnerId, '\\', this.StoreObjectId.ToString());
		}

		public const char MailboxAndStoreObjectSeperator = '\\';

		private StoreObjectId storeObjectId;
	}
}
