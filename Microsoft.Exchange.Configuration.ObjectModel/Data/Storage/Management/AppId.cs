using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class AppId : XsoMailboxObjectId
	{
		internal string AppIdValue { get; private set; }

		internal string DisplayName { get; private set; }

		internal AppId(ADObjectId mailboxOwnerId, string displayName, string extensionId) : base(mailboxOwnerId ?? new ADObjectId())
		{
			if (string.IsNullOrEmpty(extensionId))
			{
				throw new ArgumentException(Strings.ErrorEmptyParameter(typeof(string).ToString()), "extensionId");
			}
			if (!GuidHelper.TryParseGuid(extensionId, out this.extensionGuid))
			{
				throw new ArgumentException(Strings.InvalidGuidParameter(extensionId), "extensionId");
			}
			this.AppIdValue = extensionId;
			this.DisplayName = displayName;
		}

		public override byte[] GetBytes()
		{
			byte[] array = new byte[16];
			ExBitConverter.Write(this.extensionGuid, array, 0);
			byte[] bytes = base.MailboxOwnerId.GetBytes();
			byte[] array2 = new byte[array.Length + bytes.Length + 2];
			ExBitConverter.Write((short)bytes.Length, array2, 0);
			int num = 2;
			Array.Copy(bytes, 0, array2, num, bytes.Length);
			num += bytes.Length;
			Array.Copy(array, 0, array2, num, array.Length);
			return array2;
		}

		public override int GetHashCode()
		{
			return base.MailboxOwnerId.GetHashCode() ^ this.AppIdValue.GetHashCode();
		}

		public override bool Equals(XsoMailboxObjectId other)
		{
			AppId appId = other as AppId;
			return !(null == appId) && ADObjectId.Equals(base.MailboxOwnerId, other.MailboxOwnerId) && string.Equals(this.AppIdValue, appId.AppIdValue);
		}

		public override string ToString()
		{
			return string.Format("{0}{1}{2}", base.MailboxOwnerId, '\\', this.extensionGuid.ToString());
		}

		private const int BytesForGuid = 16;

		public const char MailboxAndExtensionSeparator = '\\';

		public const string ExtensionNameEscapedSeparator = "\\\\";

		private Guid extensionGuid;
	}
}
