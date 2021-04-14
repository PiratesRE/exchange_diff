using System;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.ExSmtpClient;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class XSessionParamsSmtpCommandParser
	{
		public static ParseResult Parse(CommandContext commandContext, SmtpInSessionState sessionState, out XSessionParams xsessionParams)
		{
			ArgumentValidator.ThrowIfNull("CommandContext", commandContext);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			if (!SmtpInSessionUtils.HasSMTPAcceptXSessionParamsPermission(sessionState.CombinedPermissions))
			{
				xsessionParams = null;
				return ParseResult.NotAuthorized;
			}
			ParseResult result = XSessionParamsSmtpCommandParser.ParseArguments(commandContext, sessionState.AdvertisedEhloOptions, out xsessionParams);
			if (result.IsFailed)
			{
				sessionState.Tracer.TraceError<SmtpResponse>(0L, "XSESSIONPARAMS parsing failed; SMTP Response: {0}", result.SmtpResponse);
			}
			return result;
		}

		private static ParseResult ParseArguments(CommandContext commandContext, IEhloOptions advertisedEhloOptions, out XSessionParams xsessionParams)
		{
			Guid empty = Guid.Empty;
			XSessionType sessionType = XSessionType.Undefined;
			commandContext.TrimLeadingWhitespace();
			while (!commandContext.IsEndOfCommand)
			{
				Offset offset;
				if (!commandContext.GetNextArgumentOffset(out offset))
				{
					xsessionParams = null;
					return ParseResult.InvalidArguments;
				}
				int nameValuePairSeparatorIndex = CommandParsingHelper.GetNameValuePairSeparatorIndex(commandContext.Command, offset, 61);
				if (nameValuePairSeparatorIndex <= 0 || nameValuePairSeparatorIndex >= offset.End - 1)
				{
					xsessionParams = null;
					return ParseResult.InvalidArguments;
				}
				int count = nameValuePairSeparatorIndex - offset.Start;
				int argValueLen = offset.End - (nameValuePairSeparatorIndex + 1);
				char c = (char)Util.LowerC[(int)commandContext.Command[offset.Start]];
				char c2 = c;
				if (c2 != 'm')
				{
					if (c2 == 't')
					{
						if (BufferParser.CompareArg(XSessionParamsSmtpCommandParser.XSessionTypeParameterBytes, commandContext.Command, offset.Start, count))
						{
							if (!advertisedEhloOptions.XSessionType)
							{
								xsessionParams = null;
								return ParseResult.CommandNotImplemented;
							}
							sessionType = XSessionParamsSmtpCommandParser.GetSessionType(commandContext, nameValuePairSeparatorIndex, argValueLen);
						}
					}
				}
				else if (BufferParser.CompareArg(XSessionParamsSmtpCommandParser.MdbGuidParameterBytes, commandContext.Command, offset.Start, count))
				{
					if (!advertisedEhloOptions.XSessionMdbGuid)
					{
						xsessionParams = null;
						return ParseResult.CommandNotImplemented;
					}
					if (empty != Guid.Empty || !XSessionParamsSmtpCommandParser.ParseMdbGuid(commandContext, nameValuePairSeparatorIndex, argValueLen, out empty))
					{
						xsessionParams = null;
						return ParseResult.InvalidArguments;
					}
				}
				commandContext.TrimLeadingWhitespace();
			}
			if (empty == Guid.Empty)
			{
				xsessionParams = null;
				return ParseResult.InvalidArguments;
			}
			xsessionParams = new XSessionParams(empty, sessionType);
			return ParseResult.ParsingComplete;
		}

		private static bool ParseMdbGuid(CommandContext commandContext, int separatorIndex, int argValueLen, out Guid mdbGuid)
		{
			if (argValueLen > 32)
			{
				mdbGuid = Guid.Empty;
				return false;
			}
			string text = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			if (string.IsNullOrEmpty(text) || !Guid.TryParse(text, out mdbGuid))
			{
				mdbGuid = Guid.Empty;
				return false;
			}
			return true;
		}

		private static XSessionType GetSessionType(CommandContext commandContext, int separatorIndex, int argValueLen)
		{
			string value = SmtpUtils.FromXtextString(commandContext.Command, separatorIndex + 1, argValueLen, false);
			if (string.IsNullOrEmpty(value))
			{
				return XSessionType.Undefined;
			}
			XSessionType result;
			if (!Enum.TryParse<XSessionType>(value, out result))
			{
				result = XSessionType.Undefined;
			}
			return result;
		}

		public const string CommandKeyword = "XSESSIONPARAMS";

		public const string MdbGuidParameterKeyword = "MDBGUID";

		public const string XSessionTypeParameterKeyword = "TYPE";

		private const int GuidLength = 32;

		private static readonly byte[] MdbGuidParameterBytes = Util.AsciiStringToBytes("MDBGUID".ToLower());

		private static readonly byte[] XSessionTypeParameterBytes = Util.AsciiStringToBytes("TYPE".ToLower());
	}
}
