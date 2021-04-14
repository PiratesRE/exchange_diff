using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class XProxyFromSmtpCommandParser
	{
		public static ParseResult Parse(CommandContext commandContext, SmtpInSessionState sessionState, ProcessTransportRole transportRole, int maxProxyHopCount, ITracer tracer, out XProxyFromParseOutput parseOutput)
		{
			ArgumentValidator.ThrowIfNull("commandContext", commandContext);
			ArgumentValidator.ThrowIfNull("sessionState", sessionState);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			parseOutput = default(XProxyFromParseOutput);
			if (!sessionState.AdvertisedEhloOptions.XProxyFrom)
			{
				return ParseResult.CommandNotImplementedProtocolError;
			}
			if (!SmtpInSessionUtils.HasSMTPAcceptXProxyFromPermission(sessionState.CombinedPermissions) && !SmtpInSessionUtils.ShouldAcceptProxyFromProtocol(transportRole, sessionState.Capabilities))
			{
				return ParseResult.NotAuthorized;
			}
			if (sessionState.TransportMailItem != null)
			{
				return ParseResult.BadCommandSequence;
			}
			SecurityIdentifier securityIdentifier;
			SmtpAddress smtpAddress;
			byte[] array;
			int? num;
			ParseResult result = XProxyParserUtils.ParseXProxyAndXProxyFromArguments(commandContext, true, sessionState.SmtpUtf8Supported, out parseOutput.ProxyParseCommonOutput.SessionId, out parseOutput.ProxyParseCommonOutput.ClientIp, out parseOutput.ProxyParseCommonOutput.ClientPort, out parseOutput.ProxyParseCommonOutput.ClientHelloDomain, out parseOutput.SequenceNumber, out parseOutput.PermissionsInt, out parseOutput.AuthSource, out securityIdentifier, out smtpAddress, out array, out num);
			if (result.IsFailed)
			{
				tracer.TraceError<SmtpResponse>(0L, "XPROXYFROM parsing failed; SMTP Response: {0}", result.SmtpResponse);
				return result;
			}
			tracer.TraceDebug(0L, "XPROXYFROM parsing completed. Session id = {0}; Client address = {1}; Client port = {2}; Client HELO/EHLO domain = {3}", new object[]
			{
				parseOutput.ProxyParseCommonOutput.SessionId,
				parseOutput.ProxyParseCommonOutput.ClientIp,
				parseOutput.ProxyParseCommonOutput.ClientPort,
				parseOutput.ProxyParseCommonOutput.ClientHelloDomain
			});
			if ((ulong)parseOutput.SequenceNumber > (ulong)((long)maxProxyHopCount))
			{
				return ParseResult.ProxyHopCountExceeded;
			}
			return ParseResult.ParsingComplete;
		}

		public const string CommandKeyword = "XPROXYFROM";
	}
}
