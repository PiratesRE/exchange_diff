using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class MailFromCommand : DavCommand
	{
		public MailFromCommand(byte[] commandBytes) : base(commandBytes)
		{
		}

		public BodyType BodyType
		{
			get
			{
				return this.bodyType;
			}
		}

		public DsnFormat Ret
		{
			get
			{
				return this.ret;
			}
		}

		public string EnvId
		{
			get
			{
				return this.envId;
			}
		}

		public string Auth
		{
			get
			{
				return this.auth;
			}
		}

		protected override byte[] FirstToken
		{
			get
			{
				return MailFromCommand.Mail;
			}
		}

		protected override byte[] SecondToken
		{
			get
			{
				return MailFromCommand.From;
			}
		}

		protected override void ParseArguments()
		{
			MailParseOutput mailParseOutput;
			ParseResult parseResult = MailSmtpCommandParser.ParseOptionalArguments(new RoutingAddress("notused@contoso.com"), CommandContext.FromByteArrayLegacyCodeOnly(base.CommandBytes, base.CurrentOffset), ExTraceGlobals.SmtpReceiveTracer, Components.TransportAppConfig.SmtpMailCommandConfiguration, out mailParseOutput, null);
			if (parseResult.IsFailed)
			{
				throw new FormatException(parseResult.SmtpResponse.ToString());
			}
			this.bodyType = mailParseOutput.MailBodyType;
			this.ret = mailParseOutput.DsnFormat;
			this.envId = mailParseOutput.EnvelopeId;
			this.auth = mailParseOutput.Auth;
		}

		private static readonly byte[] Mail = Util.AsciiStringToBytes("mail");

		private static readonly byte[] From = Util.AsciiStringToBytes("from");

		private BodyType bodyType;

		private DsnFormat ret;

		private string envId;

		private string auth;
	}
}
