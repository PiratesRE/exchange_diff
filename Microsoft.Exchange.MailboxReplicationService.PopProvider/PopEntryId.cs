using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class PopEntryId
	{
		public static byte[] CreateFolderEntryId(string input)
		{
			string hash = PopEntryId.GetHash(input);
			return Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}={1};{2}={3};{4}={5}", new object[]
			{
				"V",
				"1",
				"P",
				"POP",
				"FP",
				hash
			}));
		}

		public static byte[] CreateMessageEntryId(string uid)
		{
			return Encoding.UTF8.GetBytes(uid);
		}

		public static string ParseUid(byte[] messageEntryId)
		{
			return Encoding.UTF8.GetString(messageEntryId);
		}

		private static string GetHash(string input)
		{
			byte[] sha1Hash = CommonUtils.GetSHA1Hash(input.ToLowerInvariant());
			StringBuilder stringBuilder = new StringBuilder(BitConverter.ToString(sha1Hash));
			stringBuilder = stringBuilder.Replace("-", string.Empty);
			return stringBuilder.ToString();
		}

		public const char EntryIdDelimiter = ';';

		public const char EntryIdKeyValueSeparator = '=';

		public const string EntryIdVersionKey = "V";

		public const string EntryIdVersionValue = "1";

		public const string EntryIdProtocolKey = "P";

		public const string EntryIdProtocolValue = "POP";

		private const string EntryIdFolderPathHashKey = "FP";

		private static readonly string[] MessageEntryIdKeys = new string[]
		{
			"V",
			"P"
		};
	}
}
