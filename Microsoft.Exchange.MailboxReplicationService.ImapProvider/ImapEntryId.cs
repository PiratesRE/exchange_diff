using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ImapEntryId
	{
		public static byte[] CreateFolderEntryId(string input)
		{
			string hash = ImapEntryId.GetHash(input);
			return Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}={1};{2}={3};{4}={5}", new object[]
			{
				"V",
				"1",
				"P",
				"IMAP",
				"FP",
				hash
			}));
		}

		public static byte[] CreateMessageEntryId(uint uid, uint uidValidity, string folderPath, string logonName)
		{
			ImapEntryId.ValidateCreateEntryIdInput(folderPath);
			ImapEntryId.ValidateCreateEntryIdInput(logonName);
			string hash = ImapEntryId.GetHash(folderPath + logonName);
			return Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}={1};{2}={3};{4}={5};{6}={7};{8}={9}", new object[]
			{
				"V",
				"1",
				"P",
				"IMAP",
				"U",
				uid,
				"UV",
				uidValidity,
				"LF",
				hash
			}));
		}

		public static uint ParseUid(byte[] messageEntryId)
		{
			Dictionary<string, string> dictionary = ImapEntryId.ParseMessageEntryId(messageEntryId);
			string s;
			uint result;
			if (dictionary.TryGetValue("U", out s) && uint.TryParse(s, out result))
			{
				return result;
			}
			throw new ParsingMessageEntryIdFailedException(TraceUtils.DumpBytes(messageEntryId), new ArgumentException("Cannot parse uid.", "messageEntryId"));
		}

		private static string GetHash(string input)
		{
			ImapEntryId.ValidateCreateEntryIdInput(input);
			byte[] sha1Hash = CommonUtils.GetSHA1Hash(input.ToLowerInvariant());
			StringBuilder stringBuilder = new StringBuilder(BitConverter.ToString(sha1Hash));
			stringBuilder = stringBuilder.Replace("-", string.Empty);
			return stringBuilder.ToString();
		}

		private static Dictionary<string, string> ParseMessageEntryId(byte[] messageEntryId)
		{
			if (messageEntryId == null)
			{
				throw new ParsingMessageEntryIdFailedException(null, new ArgumentNullException("messageEntryId"));
			}
			string text = null;
			try
			{
				text = Encoding.UTF8.GetString(messageEntryId);
			}
			catch (Exception innerException)
			{
				throw new ParsingMessageEntryIdFailedException(TraceUtils.DumpBytes(messageEntryId), innerException);
			}
			string[] keyValuePairs = text.Split(new char[]
			{
				';'
			});
			return ImapEntryId.ParseKeyValuePairs(messageEntryId, keyValuePairs);
		}

		private static Dictionary<string, string> ParseKeyValuePairs(byte[] messageEntryId, string[] keyValuePairs)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(ImapEntryId.MessageEntryIdKeys.Length);
			foreach (string text in keyValuePairs)
			{
				string[] array = text.Split(new char[]
				{
					'='
				});
				if (array.Length == 2)
				{
					string text2 = array[0];
					string text3 = array[1];
					if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3))
					{
						throw new ParsingMessageEntryIdFailedException(TraceUtils.DumpBytes(messageEntryId), new ArgumentException("messageEntryId", string.Format("While parsing message entry id, key {0} value {1}", text2, text3)));
					}
					if (dictionary.ContainsKey(text2))
					{
						throw new ParsingMessageEntryIdFailedException(TraceUtils.DumpBytes(messageEntryId), new ArgumentException("messageEntryId", string.Format("Duplicate key {0} found while parsing message entry id.", text2)));
					}
					dictionary.Add(text2, text3);
				}
			}
			foreach (string text4 in ImapEntryId.MessageEntryIdKeys)
			{
				if (!dictionary.ContainsKey(text4))
				{
					throw new ParsingMessageEntryIdFailedException(TraceUtils.DumpBytes(messageEntryId), new ArgumentException("messageEntryId", string.Format("Key {0} not found in result.", text4)));
				}
			}
			string text5 = dictionary["V"];
			if (!text5.Equals("1", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new UnsupportedImapMessageEntryIdVersionException(text5);
			}
			string text6 = dictionary["P"];
			if (!text6.Equals("IMAP", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new UnsupportedSyncProtocolException(text6);
			}
			return dictionary;
		}

		private static void ValidateCreateEntryIdInput(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new CannotCreateEntryIdException(input, new ArgumentNullException("input"));
			}
		}

		public const char EntryIdDelimiter = ';';

		public const char EntryIdKeyValueSeparator = '=';

		public const string EntryIdVersionKey = "V";

		public const string EntryIdVersionValue = "1";

		public const string EntryIdProtocolKey = "P";

		public const string EntryIdProtocolValue = "IMAP";

		public const string EntryIdUidKey = "U";

		public const string EntryIdUidValidityKey = "UV";

		public const string EntryIdLogonNameAndFolderPathHashKey = "LF";

		private const string EntryIdFolderPathHashKey = "FP";

		private static readonly string[] MessageEntryIdKeys = new string[]
		{
			"V",
			"P",
			"U",
			"UV",
			"LF"
		};
	}
}
