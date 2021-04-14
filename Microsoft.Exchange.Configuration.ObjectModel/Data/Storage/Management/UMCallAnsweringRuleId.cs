using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class UMCallAnsweringRuleId : XsoMailboxObjectId
	{
		public Guid RuleIdGuid { get; private set; }

		internal UMCallAnsweringRuleId(ADObjectId mailboxOwnerId, Guid ruleId) : base(mailboxOwnerId)
		{
			this.RuleIdGuid = ruleId;
		}

		public override int GetHashCode()
		{
			return base.MailboxOwnerId.GetHashCode() ^ this.RuleIdGuid.GetHashCode();
		}

		public override bool Equals(XsoMailboxObjectId other)
		{
			UMCallAnsweringRuleId umcallAnsweringRuleId = other as UMCallAnsweringRuleId;
			return !(null == umcallAnsweringRuleId) && ADObjectId.Equals(base.MailboxOwnerId, other.MailboxOwnerId) && object.Equals(this.RuleIdGuid, umcallAnsweringRuleId.RuleIdGuid);
		}

		public override byte[] GetBytes()
		{
			byte[] bytes = base.MailboxOwnerId.GetBytes();
			byte[] array = new byte[16 + bytes.Length + 2];
			ExBitConverter.Write((short)bytes.Length, array, 0);
			int num = 2;
			Array.Copy(bytes, 0, array, num, bytes.Length);
			num += bytes.Length;
			ExBitConverter.Write(this.RuleIdGuid, array, num);
			return array;
		}

		public override string ToString()
		{
			return string.Format("{0}{1}{2}", base.MailboxOwnerId, "\\", this.RuleIdGuid);
		}

		public const string MailboxIdSeparator = "\\";
	}
}
