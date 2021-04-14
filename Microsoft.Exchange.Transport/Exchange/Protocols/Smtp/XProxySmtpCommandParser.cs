using System;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal static class XProxySmtpCommandParser
	{
		public static ParseResult ParseArguments(CommandContext commandContext, bool allowUtf8, out string sessionId, out IPAddress clientIP, out int clientPort, out string clientHelloDomain, out SecurityIdentifier securityId, out SmtpAddress clientProxyAddress, out byte[] redactedBuffer, out int? capabilitiesInt)
		{
			ArgumentValidator.ThrowIfNull("CommandContext", commandContext);
			uint num;
			uint? num2;
			AuthenticationSource? authenticationSource;
			return XProxyParserUtils.ParseXProxyAndXProxyFromArguments(commandContext, false, allowUtf8, out sessionId, out clientIP, out clientPort, out clientHelloDomain, out num, out num2, out authenticationSource, out securityId, out clientProxyAddress, out redactedBuffer, out capabilitiesInt);
		}

		public const string CommandKeyword = "XPROXY";
	}
}
