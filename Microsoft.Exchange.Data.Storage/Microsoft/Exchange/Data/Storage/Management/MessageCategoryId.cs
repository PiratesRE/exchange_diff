using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MessageCategoryId : XsoMailboxObjectId
	{
		internal Guid? CategoryId
		{
			get
			{
				return this.categoryId;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal MessageCategoryId(ADObjectId mailboxOwnerId, string name, Guid? categoryId) : base(mailboxOwnerId)
		{
			this.categoryId = categoryId;
			this.name = name;
		}

		public override byte[] GetBytes()
		{
			byte[] array = (this.categoryId != null) ? this.categoryId.Value.ToByteArray() : Array<byte>.Empty;
			byte[] bytes = base.MailboxOwnerId.GetBytes();
			byte[] array2 = new byte[2 + bytes.Length + array.Length];
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
			int num = base.MailboxOwnerId.GetHashCode();
			if (this.categoryId != null)
			{
				num ^= this.CategoryId.GetHashCode();
			}
			return num;
		}

		public override bool Equals(XsoMailboxObjectId other)
		{
			MessageCategoryId messageCategoryId = other as MessageCategoryId;
			return !(null == messageCategoryId) && ADObjectId.Equals(base.MailboxOwnerId, other.MailboxOwnerId) && object.Equals(this.categoryId ?? Guid.Empty, messageCategoryId.CategoryId ?? Guid.Empty);
		}

		public override string ToString()
		{
			string arg;
			if (this.categoryId != null)
			{
				arg = this.categoryId.Value.ToString();
			}
			else if (!string.IsNullOrEmpty(this.name))
			{
				arg = this.name;
			}
			else
			{
				arg = string.Empty;
			}
			return string.Format("{0}{1}{2}", base.MailboxOwnerId, '\\', arg);
		}

		public const char Separator = '\\';

		private Guid? categoryId;

		private string name;
	}
}
