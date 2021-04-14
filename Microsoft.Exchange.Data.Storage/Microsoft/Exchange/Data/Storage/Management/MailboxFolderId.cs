using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxFolderId : XsoMailboxObjectId
	{
		public MapiFolderPath MailboxFolderPath { get; private set; }

		internal StoreObjectId StoreObjectIdValue { get; private set; }

		public string StoreObjectId
		{
			get
			{
				if (string.IsNullOrEmpty(this.storeObjectId))
				{
					this.storeObjectId = ((this.StoreObjectIdValue == null) ? string.Empty : this.StoreObjectIdValue.ToString());
				}
				return this.storeObjectId;
			}
		}

		internal MailboxFolderId(ADObjectId mailboxOwnerId, StoreObjectId storeObjectId, MapiFolderPath folderPath) : base(mailboxOwnerId)
		{
			if (null == folderPath && storeObjectId == null)
			{
				throw new ArgumentException(ServerStrings.ErrorNoStoreObjectIdAndFolderPath);
			}
			this.StoreObjectIdValue = storeObjectId;
			this.MailboxFolderPath = folderPath;
		}

		public override byte[] GetBytes()
		{
			byte[] array = (this.StoreObjectIdValue == null) ? Array<byte>.Empty : this.StoreObjectIdValue.GetBytes();
			byte[] bytes = base.MailboxOwnerId.GetBytes();
			byte[] array2 = new byte[array.Length + bytes.Length + 2];
			int num = 0;
			array2[num++] = (byte)(bytes.Length & 255);
			array2[num++] = (byte)(bytes.Length >> 8 & 255);
			Array.Copy(bytes, 0, array2, num, bytes.Length);
			num += bytes.Length;
			Array.Copy(array, 0, array2, num, array.Length);
			return array2;
		}

		public override int GetHashCode()
		{
			return base.MailboxOwnerId.GetHashCode() ^ ((this.StoreObjectIdValue == null) ? 0 : this.StoreObjectIdValue.GetHashCode());
		}

		public override bool Equals(XsoMailboxObjectId other)
		{
			MailboxFolderId mailboxFolderId = other as MailboxFolderId;
			if (null == mailboxFolderId)
			{
				return false;
			}
			if (!ADObjectId.Equals(base.MailboxOwnerId, other.MailboxOwnerId))
			{
				return false;
			}
			bool flag = object.Equals(this.StoreObjectIdValue, mailboxFolderId.StoreObjectIdValue);
			if (flag && this.StoreObjectIdValue != null)
			{
				return true;
			}
			bool flag2 = object.Equals(this.MailboxFolderPath, mailboxFolderId.MailboxFolderPath);
			return (flag2 && null != this.MailboxFolderPath) || (flag2 && flag);
		}

		public override string ToString()
		{
			return string.Format("{0}{1}{2}", base.MailboxOwnerId, ':', (null == this.MailboxFolderPath) ? this.StoreObjectId : this.MailboxFolderPath.ToString());
		}

		public const char MailboxAndFolderSeperator = ':';

		private string storeObjectId;
	}
}
