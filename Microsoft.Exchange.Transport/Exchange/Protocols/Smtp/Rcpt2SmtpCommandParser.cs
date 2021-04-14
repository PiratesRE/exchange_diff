using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.ExSmtpClient;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class Rcpt2SmtpCommandParser
	{
		public static ParseResult Parse(CommandContext commandContext, SmtpInSessionState sessionState, RoutingAddress rcptToRoutingAddress, bool isDataRedactionNecessary, out Rcpt2ParseOutput rcpt2ParseOutput)
		{
			ArgumentValidator.ThrowIfNull("commandContext", commandContext);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			rcpt2ParseOutput = new Rcpt2ParseOutput();
			int num;
			RoutingAddress routingAddress;
			string text;
			ParseResult result = Rcpt2SmtpCommandParser.TryParseAddress(commandContext, out num, out routingAddress, out text);
			if (result.IsFailed)
			{
				commandContext.LogReceivedCommand(sessionState.ProtocolLogSession);
				return result;
			}
			if (!routingAddress.Equals(rcptToRoutingAddress))
			{
				commandContext.LogReceivedCommand(sessionState.ProtocolLogSession);
				return ParseResult.IgnorableValidRcpt2ButDifferentFromRcptAddress;
			}
			Dictionary<string, string> consumerMailOptionalArguments;
			byte[] bytes;
			result = Rcpt2SmtpCommandParser.ParseOptionalArguments(commandContext, out consumerMailOptionalArguments, out bytes);
			if (result.IsFailed)
			{
				commandContext.LogReceivedCommand(sessionState.ProtocolLogSession);
				return result;
			}
			if (isDataRedactionNecessary)
			{
				byte[] array = new byte[num - commandContext.OriginalOffset];
				Buffer.BlockCopy(commandContext.Command, commandContext.OriginalOffset, array, 0, array.Length);
				sessionState.ProtocolLogSession.LogReceive(Util.CreateRedactedCommand(array, routingAddress, ByteString.BytesToString(bytes, true), true));
			}
			else
			{
				commandContext.LogReceivedCommand(sessionState.ProtocolLogSession);
			}
			rcpt2ParseOutput.RecipientAddress = routingAddress;
			rcpt2ParseOutput.ConsumerMailOptionalArguments = consumerMailOptionalArguments;
			return ParseResult.ParsingComplete;
		}

		private static ParseResult TryParseAddress(CommandContext commandContext, out int dataOffset, out RoutingAddress address, out string tail)
		{
			dataOffset = 0;
			address = RoutingAddress.Empty;
			tail = null;
			if (!commandContext.ParseTokenAndVerifyCommand(Rcpt2SmtpCommandParser.TO, 58))
			{
				return ParseResult.UnrecognizedParameter;
			}
			commandContext.TrimLeadingWhitespace();
			dataOffset = commandContext.Offset;
			string protocolText;
			if (!commandContext.GetCommandArguments(out protocolText))
			{
				return ParseResult.IgnorableRcpt2InvalidAddress;
			}
			Parse821.TryParseAddressLine(protocolText, out address, out tail);
			if (tail != null)
			{
				commandContext.PushBackOffset(ByteString.StringToBytesCount(tail, true));
			}
			if (!address.IsValid)
			{
				return ParseResult.IgnorableRcpt2InvalidAddress;
			}
			if (address == RoutingAddress.NullReversePath)
			{
				return ParseResult.IgnorableRcpt2InvalidAddress;
			}
			return ParseResult.ParsingComplete;
		}

		private static ParseResult ParseOptionalArguments(CommandContext commandContext, out Dictionary<string, string> consumerMailOptionalArguments, out byte[] tailToLogArray)
		{
			tailToLogArray = null;
			List<byte> list = new List<byte>();
			byte item = Convert.ToByte(' ');
			consumerMailOptionalArguments = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			commandContext.TrimLeadingWhitespace();
			while (!commandContext.IsEndOfCommand)
			{
				Offset offset;
				if (!commandContext.GetNextArgumentOffset(out offset))
				{
					return ParseResult.InvalidArguments;
				}
				int nameValuePairSeparatorIndex = CommandParsingHelper.GetNameValuePairSeparatorIndex(commandContext.Command, offset, 61);
				if (nameValuePairSeparatorIndex <= 0 || nameValuePairSeparatorIndex == offset.Start || nameValuePairSeparatorIndex >= offset.End - 1)
				{
					return ParseResult.IgnorableRcpt2InvalidArguments;
				}
				int length = nameValuePairSeparatorIndex - offset.Start;
				int length2 = offset.End - (nameValuePairSeparatorIndex + 1);
				string text = SmtpUtils.FromXtextString(commandContext.Command, offset.Start, length, false);
				string value = SmtpUtils.FromXtextString(commandContext.Command, nameValuePairSeparatorIndex + 1, length2, false);
				if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value))
				{
					return ParseResult.IgnorableRcpt2InvalidArguments;
				}
				if (list.Any<byte>())
				{
					list.Add(item);
				}
				if (consumerMailOptionalArguments.ContainsKey(text))
				{
					consumerMailOptionalArguments[text] = value;
				}
				else
				{
					consumerMailOptionalArguments.Add(text, value);
				}
				for (int i = offset.Start; i < offset.End; i++)
				{
					list.Add(commandContext.Command[i]);
				}
				commandContext.TrimLeadingWhitespace();
			}
			tailToLogArray = list.ToArray();
			return ParseResult.ParsingComplete;
		}

		public const string CommandKeyword = "RCPT2";

		private static readonly byte[] TO = Util.AsciiStringToBytes("to");
	}
}
