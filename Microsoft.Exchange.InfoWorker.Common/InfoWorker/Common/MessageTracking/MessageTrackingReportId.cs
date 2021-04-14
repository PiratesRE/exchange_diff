using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	[Serializable]
	internal sealed class MessageTrackingReportId
	{
		public MessageTrackingReportId()
		{
		}

		public MessageTrackingReportId(string messageId, string server, long internalMessageId, SmtpAddress mailbox, string domain, bool isSender)
		{
			if (!mailbox.IsValidAddress)
			{
				throw new ArgumentException("Invalid mailbox", "mailbox");
			}
			this.mailbox = mailbox;
			this.server = server;
			this.messageId = messageId;
			this.internalMessageId = internalMessageId;
			this.domain = domain;
			this.isSender = isSender;
		}

		public MessageTrackingReportId(string messageId, string server, long internalMessageId, Guid guid, string domain, bool isSender)
		{
			this.userGuid = guid;
			this.server = server;
			this.messageId = messageId;
			this.internalMessageId = internalMessageId;
			this.domain = domain;
			this.isSender = isSender;
		}

		public static bool TryParse(string identity, out MessageTrackingReportId identityObject)
		{
			if (identity == MessageTrackingReportId.LegacyExchangeValue)
			{
				identityObject = MessageTrackingReportId.LegacyExchange;
				return true;
			}
			string value = null;
			string value2 = null;
			long? num = null;
			Guid? guid = null;
			bool? flag = null;
			string text = null;
			identityObject = null;
			string[] array = identity.Split(new char[]
			{
				','
			});
			string[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				string text2 = array2[i];
				string[] array3 = text2.Split(new char[]
				{
					'='
				});
				bool result;
				string text3;
				if (array3.Length != 2 || string.IsNullOrEmpty(array3[1]))
				{
					result = false;
				}
				else if (!MessageTrackingReportId.TryUnescapeSpecialCharacters(array3[1], out text3))
				{
					result = false;
				}
				else
				{
					string key;
					if ((key = array3[0]) != null)
					{
						if (<PrivateImplementationDetails>{555FE7EF-A8C8-4AA8-9F87-0745861BF96F}.$$method0x6001364-1 == null)
						{
							<PrivateImplementationDetails>{555FE7EF-A8C8-4AA8-9F87-0745861BF96F}.$$method0x6001364-1 = new Dictionary<string, int>(6)
							{
								{
									"Message-Id",
									0
								},
								{
									"Server",
									1
								},
								{
									"Internal-Id",
									2
								},
								{
									"Sender",
									3
								},
								{
									"Recipient",
									4
								},
								{
									"Domain",
									5
								}
							};
						}
						int num2;
						if (<PrivateImplementationDetails>{555FE7EF-A8C8-4AA8-9F87-0745861BF96F}.$$method0x6001364-1.TryGetValue(key, out num2))
						{
							switch (num2)
							{
							case 0:
								value = text3;
								break;
							case 1:
								value2 = text3;
								break;
							case 2:
							{
								long value3;
								if (!long.TryParse(text3, out value3))
								{
									return false;
								}
								num = new long?(value3);
								break;
							}
							case 3:
							{
								Guid value4;
								if (!GuidHelper.TryParseGuid(text3, out value4))
								{
									TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "The value {0} does not represent a valid GUID", text3);
									return false;
								}
								guid = new Guid?(value4);
								flag = new bool?(true);
								break;
							}
							case 4:
								guid = new Guid?(new Guid(text3));
								flag = new bool?(false);
								break;
							case 5:
								text = text3;
								break;
							default:
								goto IL_1C6;
							}
							i++;
							continue;
						}
					}
					IL_1C6:
					result = false;
				}
				return result;
			}
			if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value2) || num == null || flag == null)
			{
				return false;
			}
			if (guid == null)
			{
				return false;
			}
			Guid value5 = guid.Value;
			identityObject = new MessageTrackingReportId(value, value2, num.Value, value5, text, flag.Value);
			return true;
		}

		public override string ToString()
		{
			if (this == MessageTrackingReportId.LegacyExchange)
			{
				return MessageTrackingReportId.LegacyExchangeValue;
			}
			string text = this.isSender ? "Sender" : "Recipient";
			return string.Format("{0}={1},{2}={3},{4}={5},{6}={7},{8}={9}", new object[]
			{
				"Message-Id",
				MessageTrackingReportId.EscapeSpecialCharacters(this.messageId),
				"Server",
				MessageTrackingReportId.EscapeSpecialCharacters(this.server),
				"Internal-Id",
				MessageTrackingReportId.EscapeSpecialCharacters(this.internalMessageId.ToString()),
				text,
				MessageTrackingReportId.EscapeSpecialCharacters(this.userGuid.ToString()),
				"Domain",
				MessageTrackingReportId.EscapeSpecialCharacters(this.domain)
			});
		}

		public string MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public long InternalMessageId
		{
			get
			{
				return this.internalMessageId;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public SmtpAddress Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public Guid UserGuid
		{
			get
			{
				return this.userGuid;
			}
		}

		public bool IsSender
		{
			get
			{
				return this.isSender;
			}
		}

		private static string EscapeSpecialCharacters(string value)
		{
			StringBuilder stringBuilder = null;
			for (int i = 0; i < value.Length; i++)
			{
				if (MessageTrackingReportId.IsSpecial(value[i]))
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
						stringBuilder.Append(value, 0, i);
					}
					if ((ulong)value[i] >= 256UL)
					{
						throw new FormatException(string.Format("Position: {0}", i));
					}
					stringBuilder.AppendFormat("+{0:x2}", (long)((ulong)value[i]));
				}
				else if (stringBuilder != null)
				{
					stringBuilder.Append(value[i]);
				}
			}
			if (stringBuilder != null)
			{
				return stringBuilder.ToString();
			}
			return value;
		}

		private static bool TryUnescapeSpecialCharacters(string value, out string unescapedValue)
		{
			unescapedValue = value;
			StringBuilder stringBuilder = null;
			int i = 0;
			while (i < value.Length)
			{
				if (value[i] != '+')
				{
					goto IL_51;
				}
				if (i >= value.Length - 2)
				{
					return false;
				}
				if (stringBuilder != null)
				{
					goto IL_51;
				}
				stringBuilder = new StringBuilder();
				stringBuilder.Append(value, 0, i);
				char value2;
				if (!MessageTrackingReportId.TryParseHex(value, i + 1, out value2))
				{
					return false;
				}
				stringBuilder.Append(value2);
				i += 2;
				IL_62:
				i++;
				continue;
				IL_51:
				if (stringBuilder != null)
				{
					stringBuilder.Append(value[i]);
					goto IL_62;
				}
				goto IL_62;
			}
			if (stringBuilder != null)
			{
				unescapedValue = stringBuilder.ToString();
			}
			return true;
		}

		private static bool IsSpecial(char ch)
		{
			foreach (char c in MessageTrackingReportId.SpecialChars)
			{
				if (c == ch)
				{
					return true;
				}
			}
			return false;
		}

		private static bool TryParseHex(string s, int start, out char result)
		{
			if (s.Length - start < 2)
			{
				result = '\0';
				return false;
			}
			string s2 = s.Substring(start, 2);
			int num = 0;
			if (!int.TryParse(s2, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out num))
			{
				result = '\0';
				return false;
			}
			result = (char)num;
			return true;
		}

		public const string MessageIdTag = "Message-Id";

		public const string ServerTag = "Server";

		public const string InternalIdTag = "Internal-Id";

		public const string SenderTag = "Sender";

		public const string RecipientTag = "Recipient";

		public const string DomainTag = "Domain";

		public static readonly MessageTrackingReportId LegacyExchange = new MessageTrackingReportId();

		private static readonly string LegacyExchangeValue = "LegacyExchangeReportId";

		private static readonly char[] SpecialChars = new char[]
		{
			'=',
			',',
			'+'
		};

		private string messageId;

		private string server;

		private long internalMessageId;

		private SmtpAddress mailbox;

		private Guid userGuid;

		private string domain;

		private bool isSender;
	}
}
