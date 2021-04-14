using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class RcptToCommand : DavCommand
	{
		public RcptToCommand(byte[] commandBytes) : base(commandBytes)
		{
		}

		public DsnRequestedFlags Notify
		{
			get
			{
				return this.notify;
			}
		}

		public string ORcpt
		{
			get
			{
				return this.orcpt;
			}
		}

		protected override byte[] FirstToken
		{
			get
			{
				return RcptToCommand.Rcpt;
			}
		}

		protected override byte[] SecondToken
		{
			get
			{
				return RcptToCommand.To;
			}
		}

		protected override void ParseArguments()
		{
			if (base.Address == RoutingAddress.NullReversePath)
			{
				throw new FormatException("Recipient can't be empty");
			}
			RoutingAddress routingAddress;
			string text;
			byte[] array;
			ParseResult parseResult = RcptSmtpCommandParser.ParseOptionalArguments(CommandContext.FromByteArrayLegacyCodeOnly(base.CommandBytes, base.CurrentOffset), true, false, out this.notify, out this.orcpt, out routingAddress, out text, out array, null);
			if (parseResult.IsFailed)
			{
				throw new FormatException(parseResult.SmtpResponse.ToString());
			}
		}

		private static readonly byte[] Rcpt = Util.AsciiStringToBytes("rcpt");

		private static readonly byte[] To = Util.AsciiStringToBytes("to");

		private DsnRequestedFlags notify;

		private string orcpt;
	}
}
