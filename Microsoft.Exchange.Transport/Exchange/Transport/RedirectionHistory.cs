using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal class RedirectionHistory
	{
		public static byte[] GenerateRedirectionHistory(string originalAddress, RedirectionHistoryReason type, DateTime time)
		{
			byte[] result;
			try
			{
				using (RedirectionHistory.RedirectionHistoryElement redirectionHistoryElement = new RedirectionHistory.RedirectionHistoryElement(originalAddress, type, time))
				{
					byte[] bytes = redirectionHistoryElement.GetBytes();
					RedirectionHistory.diag.TraceDebug<int>(0L, "RedirectionHistory Element structure has {0} bytes", bytes.Length);
					using (RedirectionHistory.FlatEntry flatEntry = new RedirectionHistory.FlatEntry(bytes))
					{
						byte[] bytes2 = flatEntry.GetBytes();
						using (RedirectionHistory.FlatEntryList flatEntryList = new RedirectionHistory.FlatEntryList(bytes2))
						{
							result = flatEntryList.GetBytes();
						}
					}
				}
			}
			catch (IOException arg)
			{
				RedirectionHistory.diag.TraceError<IOException>(0L, "Failed to generate redirection history from ORCPT due to exception {0}", arg);
				result = null;
			}
			return result;
		}

		public static byte[] GenerateRedirectionHistoryFromOrcpt(string orcpt)
		{
			int num = orcpt.IndexOf(';');
			if (num == -1 || num == orcpt.Length - 1)
			{
				RedirectionHistory.diag.TraceError<string>(0L, "Failed to generate redirection history from ORCPT due to invalid ORCPT {0}", orcpt);
				return null;
			}
			return RedirectionHistory.GenerateRedirectionHistory(orcpt.Substring(num + 1), RedirectionHistoryReason.Rsar, DateTime.UtcNow);
		}

		public static bool TryDecodeRedirectionHistory(byte[] redirectionHistoryBlob, out string address, out RedirectionHistoryReason type, out DateTime time)
		{
			address = string.Empty;
			type = RedirectionHistoryReason.Rsar;
			time = DateTime.MinValue;
			if (redirectionHistoryBlob == null || redirectionHistoryBlob.Length < 28)
			{
				RedirectionHistory.diag.TraceError(0L, "Failed to decode redirection history blob, redirectionHistory is not valid");
				return false;
			}
			if (BitConverter.ToUInt32(redirectionHistoryBlob, 0) != 1U)
			{
				RedirectionHistory.diag.TraceError(0L, "Failed to decode redirection history blob, the number of FlatEntry is not 1");
				return false;
			}
			if ((ulong)BitConverter.ToUInt32(redirectionHistoryBlob, 4) != (ulong)((long)(redirectionHistoryBlob.Length - 8)))
			{
				RedirectionHistory.diag.TraceError(0L, "Failed to decode redirection history blob, the length of FlatEntry is not correct");
				return false;
			}
			if ((ulong)BitConverter.ToUInt32(redirectionHistoryBlob, 8) != (ulong)((long)(redirectionHistoryBlob.Length - 12)))
			{
				RedirectionHistory.diag.TraceError(0L, "Failed to decode redirection history blob, the length of RedirectionHistoryElement is not correct");
				return false;
			}
			switch (BitConverter.ToUInt32(redirectionHistoryBlob, 12))
			{
			case 0U:
				type = RedirectionHistoryReason.Rsar;
				break;
			case 1U:
				type = RedirectionHistoryReason.Orar;
				break;
			default:
				RedirectionHistory.diag.TraceError(0L, "Failed to decode redirection history blob, Unknow redirection type");
				return false;
			}
			try
			{
				time = DateTime.FromFileTimeUtc(BitConverter.ToInt64(redirectionHistoryBlob, 16));
				string @string = Encoding.ASCII.GetString(redirectionHistoryBlob, 24, redirectionHistoryBlob.Length - 24);
				string text = @string;
				char[] trimChars = new char[1];
				address = text.TrimEnd(trimChars);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				RedirectionHistory.diag.TraceError<string>(0L, "Failed to decode redirection history blob due to exception {0}", ex.Message);
				return false;
			}
			return true;
		}

		public static void SetRedirectionHistoryOnRecipient(MailRecipient recipient, string originalAddressString)
		{
			bool flag = false;
			if (!string.IsNullOrEmpty(recipient.ORcpt))
			{
				int num = recipient.ORcpt.IndexOf(';');
				if (num != -1 && num != recipient.ORcpt.Length - 1)
				{
					return;
				}
			}
			if (!flag)
			{
				RedirectionHistory.SetORcpt(recipient, originalAddressString);
			}
		}

		public static void SetORcpt(MailRecipient recipient, string originalAddressString)
		{
			ProxyAddress proxyAddress;
			if (!SmtpProxyAddress.TryDeencapsulate(originalAddressString, out proxyAddress))
			{
				recipient.ORcpt = "rfc822;" + originalAddressString;
				return;
			}
			if (proxyAddress.Prefix == ProxyAddressPrefix.X400)
			{
				recipient.ORcpt = "x400;" + proxyAddress.AddressString;
				return;
			}
			recipient.ORcpt = "rfc822;" + originalAddressString;
		}

		private const int MiniumFlatEntryListLength = 28;

		private const int FlatEntryListLengthOffset = 4;

		private const int FlatEntryLengthOffset = 8;

		private const int RedirectionHistoryOffset = 12;

		private const int RedirectionHistoryTimeOffset = 16;

		private const int RedirectionHistoryAddressOffset = 24;

		private const string Rfc822Prefix = "rfc822;";

		private const string X400Prefix = "x400;";

		private static readonly Trace diag = ExTraceGlobals.OrarTracer;

		private class Writer : BinaryWriter
		{
			public Writer() : base(new MemoryStream())
			{
			}

			public byte[] GetBytes()
			{
				this.Flush();
				return ((MemoryStream)this.BaseStream).ToArray();
			}
		}

		private class FlatEntryList : RedirectionHistory.Writer
		{
			public FlatEntryList(byte[] abEntries)
			{
				this.Write(1U);
				this.Write((uint)abEntries.Length);
				this.Write(abEntries, 0, abEntries.Length);
			}
		}

		private class FlatEntry : RedirectionHistory.Writer
		{
			public FlatEntry(byte[] abEntry)
			{
				this.Write((uint)abEntry.Length);
				this.Write(abEntry, 0, abEntry.Length);
			}
		}

		private class RedirectionHistoryElement : RedirectionHistory.Writer
		{
			public RedirectionHistoryElement(string originalAddress, RedirectionHistoryReason type, DateTime time)
			{
				this.Write((uint)type);
				this.Write(time.ToFileTimeUtc());
				byte[] bytes = Encoding.ASCII.GetBytes(originalAddress + '\0');
				this.Write(bytes, 0, bytes.Length);
				int num = 4 - bytes.Length % 4;
				if (num != 4)
				{
					this.Write(new byte[num]);
				}
				RedirectionHistory.diag.TraceDebug<int>((long)this.GetHashCode(), "RedirectionHistory {0} bytes padding", num);
			}
		}
	}
}
