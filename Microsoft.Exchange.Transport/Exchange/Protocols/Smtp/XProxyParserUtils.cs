using System;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.ExSmtpClient;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class XProxyParserUtils
	{
		public static ParseResult ParseXProxyAndXProxyFromArguments(CommandContext commandContext, bool sequenceNumberRequired, bool allowUtf8, out string sessionId, out IPAddress clientIP, out int clientPort, out string clientHelloDomain, out uint sequenceNumber, out uint? permissionsInt, out AuthenticationSource? authSource, out SecurityIdentifier securityId, out SmtpAddress clientProxyAddress, out byte[] redactedBuffer, out int? capabilitiesInt)
		{
			ArgumentValidator.ThrowIfNull("CommandContext", commandContext);
			sessionId = null;
			clientIP = null;
			clientPort = -1;
			sequenceNumber = 0U;
			clientHelloDomain = null;
			permissionsInt = null;
			authSource = null;
			securityId = null;
			clientProxyAddress = SmtpAddress.Empty;
			redactedBuffer = null;
			capabilitiesInt = null;
			int num = 0;
			int num2 = 0;
			string value = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			commandContext.TrimLeadingWhitespace();
			while (!commandContext.IsEndOfCommand)
			{
				Offset offset;
				if (!commandContext.GetNextArgumentOffset(out offset))
				{
					return ParseResult.InvalidArguments;
				}
				int nameValuePairSeparatorIndex = CommandParsingHelper.GetNameValuePairSeparatorIndex(commandContext.Command, offset, 61);
				if (nameValuePairSeparatorIndex <= 0 || nameValuePairSeparatorIndex >= offset.End - 1)
				{
					return ParseResult.InvalidArguments;
				}
				int count = nameValuePairSeparatorIndex - offset.Start;
				int num3 = offset.End - (nameValuePairSeparatorIndex + 1);
				bool flag = true;
				char c = (char)Util.LowerC[(int)commandContext.Command[offset.Start]];
				char c2 = c;
				if (c2 <= 'i')
				{
					switch (c2)
					{
					case 'a':
						if (!BufferParser.CompareArg(XProxyParserUtils.AuthenticationSourceKeywordBytes, commandContext.Command, offset.Start, count))
						{
							flag = false;
						}
						else if (authSource == null)
						{
							string text = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, false);
							if (text != null)
							{
								flag = false;
								AuthenticationSource value2;
								if (Enum.TryParse<AuthenticationSource>(text, out value2))
								{
									authSource = new AuthenticationSource?(value2);
								}
							}
						}
						break;
					case 'b':
						goto IL_54A;
					case 'c':
						if (!BufferParser.CompareArg(XProxyParserUtils.CapabilitiesKeywordBytes, commandContext.Command, offset.Start, count))
						{
							flag = false;
						}
						else if (capabilitiesInt == null)
						{
							string text2 = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, false);
							int value3;
							if (text2 != null && int.TryParse(text2, out value3))
							{
								capabilitiesInt = new int?(value3);
								flag = false;
							}
						}
						break;
					case 'd':
						if (!BufferParser.CompareArg(XProxyParserUtils.ClientHelloDomainKeywordBytes, commandContext.Command, offset.Start, count))
						{
							flag = false;
						}
						else if (string.IsNullOrEmpty(clientHelloDomain))
						{
							clientHelloDomain = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, allowUtf8);
							if (clientHelloDomain != null && HeloSmtpCommandParser.IsValidHeloDomain(clientHelloDomain, HeloOrEhlo.Ehlo, allowUtf8))
							{
								flag = false;
							}
						}
						break;
					default:
						if (c2 != 'i')
						{
							goto IL_54A;
						}
						if (!BufferParser.CompareArg(XProxyParserUtils.ClientIPKeywordBytes, commandContext.Command, offset.Start, count))
						{
							flag = false;
						}
						else if (clientIP == null)
						{
							string text3 = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, false);
							if (text3 != null && IPAddress.TryParse(text3, out clientIP))
							{
								flag = false;
							}
						}
						break;
					}
				}
				else if (c2 != 'p')
				{
					switch (c2)
					{
					case 's':
						if (BufferParser.CompareArg(XProxyParserUtils.SessionIdKeywordBytes, commandContext.Command, offset.Start, count))
						{
							if (!string.IsNullOrEmpty(sessionId) || num3 > XProxyParserUtils.MaxSessionIdLength)
							{
								break;
							}
							sessionId = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, false);
							if (sessionId == null)
							{
								break;
							}
						}
						else if (BufferParser.CompareArg(XProxyParserUtils.SequenceNumberKeywordBytes, commandContext.Command, offset.Start, count))
						{
							if (!sequenceNumberRequired || sequenceNumber != 0U || num3 > 1)
							{
								break;
							}
							string text4 = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, false);
							uint num4;
							if (text4 == null || !uint.TryParse(text4, out num4) || num4 == 0U)
							{
								break;
							}
							sequenceNumber = num4;
						}
						else if (BufferParser.CompareArg(XProxyParserUtils.SecurityIdKeywordBytes, commandContext.Command, offset.Start, count))
						{
							if (securityId != null)
							{
								break;
							}
							string text5 = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, false);
							if (string.IsNullOrEmpty(text5))
							{
								break;
							}
							string @string;
							try
							{
								byte[] bytes = Convert.FromBase64String(text5);
								@string = Encoding.UTF7.GetString(bytes);
							}
							catch (FormatException)
							{
								break;
							}
							securityId = new SecurityIdentifier(@string);
						}
						flag = false;
						break;
					case 't':
						goto IL_54A;
					case 'u':
						if (!BufferParser.CompareArg(XProxyParserUtils.ClientUsernameKeywordBytes, commandContext.Command, offset.Start, count))
						{
							flag = false;
						}
						else if (clientProxyAddress.Equals(SmtpAddress.Empty))
						{
							string text6 = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, allowUtf8);
							if (!string.IsNullOrEmpty(text6) && SmtpAddress.IsValidSmtpAddress(text6))
							{
								clientProxyAddress = new SmtpAddress(text6);
								if (Util.IsDataRedactionNecessary())
								{
									num = nameValuePairSeparatorIndex + 1;
									num2 = num3;
									value = Util.RedactUserName(text6);
								}
								flag = false;
							}
						}
						break;
					default:
						goto IL_54A;
					}
				}
				else
				{
					if (BufferParser.CompareArg(XProxyParserUtils.ClientPortKeywordBytes, commandContext.Command, offset.Start, count))
					{
						if (clientPort != -1 || num3 > XProxyParserUtils.MaxClientPortLength)
						{
							goto IL_54D;
						}
						string text7 = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, false);
						if (text7 == null || !int.TryParse(text7, out clientPort) || clientPort < 0)
						{
							goto IL_54D;
						}
						if (clientPort > 65535)
						{
							goto IL_54D;
						}
					}
					else if (BufferParser.CompareArg(XProxyParserUtils.PermissionsKeywordBytes, commandContext.Command, offset.Start, count))
					{
						if (permissionsInt != null)
						{
							goto IL_54D;
						}
						string text8 = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, num3, false);
						uint value4;
						if (text8 == null || !uint.TryParse(text8, out value4))
						{
							goto IL_54D;
						}
						permissionsInt = new uint?(value4);
					}
					flag = false;
				}
				IL_54D:
				if (flag)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(Encoding.ASCII.GetString(commandContext.Command, offset.Start, count));
				}
				commandContext.TrimLeadingWhitespace();
				continue;
				IL_54A:
				flag = false;
				goto IL_54D;
			}
			if (!string.IsNullOrEmpty(value))
			{
				byte[] array = ByteString.StringToBytes(value, allowUtf8);
				redactedBuffer = new byte[commandContext.Command.Length + array.Length - num2];
				Buffer.BlockCopy(commandContext.Command, 0, redactedBuffer, 0, num);
				Buffer.BlockCopy(array, 0, redactedBuffer, num, array.Length);
				if (redactedBuffer.Length > num + array.Length)
				{
					int srcOffset = num + num2;
					int num5 = num + array.Length;
					int count2 = redactedBuffer.Length - num5;
					Buffer.BlockCopy(commandContext.Command, srcOffset, redactedBuffer, num5, count2);
				}
			}
			if (stringBuilder.Length != 0)
			{
				return new ParseResult(ParsingStatus.ProtocolError, new SmtpResponse("501", "5.5.4", new string[]
				{
					"Invalid arguments - " + stringBuilder
				}), false);
			}
			if (string.IsNullOrEmpty(sessionId) || clientPort == -1 || clientIP == null || (sequenceNumberRequired && sequenceNumber == 0U))
			{
				return ParseResult.RequiredArgumentsNotPresent;
			}
			return ParseResult.ParsingComplete;
		}

		public static readonly string SessionIdKeyword = "SID";

		public static readonly string SecurityIdKeyword = "SECID";

		public static readonly string ClientUsernameKeyword = "UNAME";

		public static readonly string ClientIPKeyword = "IP";

		public static readonly string ClientPortKeyword = "PORT";

		public static readonly string ClientHelloDomainKeyword = "DOMAIN";

		public static readonly string SequenceNumberKeyword = "SEQNUM";

		public static readonly string PermissionsKeyword = "PERMS";

		public static readonly string CapabilitiesKeyword = "CAPABILITIES";

		public static readonly string AuthenticationSourceKeyword = "AUTHSRC";

		public static readonly int MaxClientPortLength = 65535.ToString(CultureInfo.InvariantCulture).Length;

		public static readonly int MaxSessionIdLength = ulong.MaxValue.ToString(CultureInfo.InvariantCulture).Length;

		private static readonly byte[] SessionIdKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.SessionIdKeyword.ToLower());

		private static readonly byte[] SecurityIdKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.SecurityIdKeyword.ToLower());

		private static readonly byte[] ClientIPKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.ClientIPKeyword.ToLower());

		private static readonly byte[] ClientUsernameKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.ClientUsernameKeyword.ToLower());

		private static readonly byte[] ClientPortKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.ClientPortKeyword.ToLower());

		private static readonly byte[] ClientHelloDomainKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.ClientHelloDomainKeyword.ToLower());

		private static readonly byte[] SequenceNumberKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.SequenceNumberKeyword.ToLower());

		private static readonly byte[] PermissionsKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.PermissionsKeyword.ToLower());

		private static readonly byte[] CapabilitiesKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.CapabilitiesKeyword.ToLower());

		private static readonly byte[] AuthenticationSourceKeywordBytes = Util.AsciiStringToBytes(XProxyParserUtils.AuthenticationSourceKeyword.ToLower());
	}
}
