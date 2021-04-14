using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class InboxRuleId : XsoMailboxObjectId
	{
		internal RuleId RuleId
		{
			get
			{
				return this.ruleId;
			}
		}

		internal ulong? StoreObjectId
		{
			get
			{
				if (this.ruleId != null)
				{
					return new ulong?(InboxRuleTaskHelper.GetRuleIdentity(this.ruleId));
				}
				return null;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal InboxRuleId(ADObjectId mailboxOwnerId, string name, RuleId ruleId) : base(mailboxOwnerId)
		{
			this.ruleId = ruleId;
			this.name = name;
		}

		public override byte[] GetBytes()
		{
			byte[] array = (this.StoreObjectId != null) ? BitConverter.GetBytes(this.StoreObjectId.Value) : new byte[0];
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
			return base.MailboxOwnerId.GetHashCode() ^ ((this.StoreObjectId != null) ? this.StoreObjectId.Value.GetHashCode() : 0);
		}

		public override bool Equals(XsoMailboxObjectId other)
		{
			InboxRuleId inboxRuleId = other as InboxRuleId;
			return !(null == inboxRuleId) && ADObjectId.Equals(base.MailboxOwnerId, other.MailboxOwnerId) && object.Equals(this.ruleId, inboxRuleId.RuleId);
		}

		public override string ToString()
		{
			string arg;
			if (this.StoreObjectId != null)
			{
				arg = this.StoreObjectId.Value.ToString();
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

		public const char MailboxAndRuleSeparator = '\\';

		public const string RuleNameEscapedSeparator = "\\\\";

		private RuleId ruleId;

		private string name;
	}
}
