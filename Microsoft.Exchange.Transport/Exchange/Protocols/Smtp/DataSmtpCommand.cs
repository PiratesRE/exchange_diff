using System;
using System.IO;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class DataSmtpCommand : BaseDataSmtpCommand
	{
		public DataSmtpCommand(ISmtpSession session, ITransportAppConfig transportAppConfig) : base(session, "DATA", "OnDataCommand", LatencyComponent.SmtpReceiveOnDataCommand, transportAppConfig)
		{
		}

		internal DataSmtpCommand() : base("DATA")
		{
		}

		protected override SmtpInParser SmtpInParser
		{
			get
			{
				return this.smtpInDataParser;
			}
		}

		protected override long AccumulatedMessageSize
		{
			get
			{
				return this.SmtpInParser.TotalBytesWritten;
			}
		}

		internal static bool RunInboundParseChecks(ISmtpInSession session, SmtpCommand command)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfNull("command", command);
			if (!command.VerifyHelloReceived() || !command.VerifyMailFromReceived() || !command.VerifyRcptToReceived() || !command.VerifyNoOngoingBdat())
			{
				return false;
			}
			ArgumentValidator.ThrowIfNull("session.TransportMailItem", session.TransportMailItem);
			ParseResult parseResult = DataSmtpCommandParser.Parse(CommandContext.FromSmtpCommand(command), SmtpInSessionState.FromSmtpInSession(session));
			command.SmtpResponse = parseResult.SmtpResponse;
			command.ParsingStatus = parseResult.ParsingStatus;
			return command.ParsingStatus == ParsingStatus.MoreDataRequired;
		}

		protected override void InboundParseCommandInternal()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "DATA.InboundParseCommand");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DATAInboundParseCommand);
			if (!DataSmtpCommand.RunInboundParseChecks(smtpInSession, this))
			{
				return;
			}
			this.isLastChunk = true;
			this.isFirstChunk = true;
		}

		internal override void InboundProcessCommand()
		{
			ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)this.GetHashCode(), "DATA.InboundProcessCommand");
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.DATAInboundProcessCommand);
			if (base.ParsingStatus != ParsingStatus.MoreDataRequired)
			{
				return;
			}
			bool expectBinaryContent = smtpInSession.Connector.EightBitMimeEnabled || !this.transportAppConfig.SmtpDataConfiguration.InboundApplyMissingEncoding;
			if (!base.SetupMessageStream(expectBinaryContent))
			{
				base.ParsingStatus = ParsingStatus.Error;
				return;
			}
			this.smtpInDataParser = new SmtpInDataParser
			{
				BodyStream = this.bodyStream,
				ExceptionFilter = new ExceptionFilter(base.ParserException)
			};
			smtpInSession.MimeDocument.EndOfHeaders = new MimeDocument.EndOfHeadersCallback(base.ParserEndOfHeadersCallback);
			base.SmtpResponse = SmtpResponse.StartData;
			smtpInSession.SetRawModeAfterCommandCompleted(new RawDataHandler(this.RawDataReceived));
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			if (this.sentDataVerb)
			{
				return;
			}
			bool flag = false;
			try
			{
				Stream firewalledStream = this.GetFirewalledStream();
				if (firewalledStream != null)
				{
					this.bodyStream = new DotStuffingStream(firewalledStream, this.transportAppConfig.SmtpDataConfiguration.OutboundRejectBareLinefeeds);
					long length = firewalledStream.Length;
					if (!smtpOutSession.RemoteIsAuthenticated && !smtpOutSession.IsAuthenticated && smtpOutSession.AdvertisedEhloOptions.Size == SizeMode.Enabled && smtpOutSession.AdvertisedEhloOptions.MaxSize > 0L && length > smtpOutSession.AdvertisedEhloOptions.MaxSize)
					{
						flag = true;
						ExTraceGlobals.SmtpSendTracer.TraceError((long)this.GetHashCode(), "Message from {0} was NDR'ed because the size was {1} whereas the maximum allowed size by the receiving server (at {2}) was {3}", new object[]
						{
							smtpOutSession.RoutedMailItem.From.ToString(),
							length,
							smtpOutSession.SessionProps.RemoteEndPoint,
							smtpOutSession.AdvertisedEhloOptions.MaxSize
						});
						smtpOutSession.RoutedMailItem.AddDsnParameters("MaxMessageSizeInKB", smtpOutSession.AdvertisedEhloOptions.MaxSize >> 10);
						smtpOutSession.RoutedMailItem.AddDsnParameters("CurrentMessageSizeInKB", length >> 10);
						smtpOutSession.AckMessage(AckStatus.Fail, AckReason.DataOverAdvertisedSizeLimit);
					}
				}
				else
				{
					flag = true;
					ExTraceGlobals.SmtpSendTracer.TraceError<string>((long)this.GetHashCode(), "Message from {0} was NDR'ed because the body stream was null", smtpOutSession.RoutedMailItem.From.ToString());
					smtpOutSession.AckMessage(AckStatus.Fail, AckReason.UnexpectedException);
				}
			}
			catch (IOException ex)
			{
				flag = true;
				ExTraceGlobals.SmtpSendTracer.TraceError<string, string>((long)this.GetHashCode(), "Unable to open message-stream, for message from: {0}, acking it retry and moving on to the next one. The IOException message was: {1}", smtpOutSession.RoutedMailItem.From.ToString(), ex.Message);
				smtpOutSession.AckMessage(AckStatus.Retry, SmtpResponse.CTSParseError);
			}
			if (flag)
			{
				if (this.bodyStream != null)
				{
					this.bodyStream.Close();
					this.bodyStream = null;
				}
				smtpOutSession.PrepareForNextMessage(true);
				return;
			}
			base.ProtocolCommandString = "DATA";
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			string statusCode = base.SmtpResponse.StatusCode;
			if (smtpOutSession.NextHopConnection == null || smtpOutSession.RoutedMailItem == null)
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<SmtpResponse>((long)this.GetHashCode(), "Connection already marked for Failover or the message has already been acked for a non-success status.  Not processing response for the DATA command: {0}", base.SmtpResponse);
				return;
			}
			if ((base.SmtpResponse.SmtpResponseType == SmtpResponseType.PermanentError && !Util.DowngradeCustomPermanentFailure(smtpOutSession.Connector.ErrorPolicies, base.SmtpResponse, this.transportAppConfig)) || (base.SmtpResponse.SmtpResponseType == SmtpResponseType.TransientError && Util.UpgradeCustomPermanentFailure(smtpOutSession.Connector.ErrorPolicies, base.SmtpResponse, this.transportAppConfig)))
			{
				ExTraceGlobals.SmtpSendTracer.TraceDebug<string, SmtpResponse>((long)this.GetHashCode(), "DATA command for message from {0} failed with: {1}", smtpOutSession.RoutedMailItem.From.ToString(), base.SmtpResponse);
				smtpOutSession.AckMessage(AckStatus.Fail, base.SmtpResponse);
				smtpOutSession.PrepareForNextMessage(true);
				return;
			}
			if (this.sentDataVerb)
			{
				bool issueBetweenMsgRset = false;
				if (statusCode[0] != '2')
				{
					ExTraceGlobals.SmtpSendTracer.TraceError<string, IPEndPoint, SmtpResponse>((long)this.GetHashCode(), "Failed to deliver message from {0} to {1}, Status: {2}", smtpOutSession.RoutedMailItem.From.ToString(), smtpOutSession.RemoteEndPoint, base.SmtpResponse);
					smtpOutSession.AckMessage(AckStatus.Retry, base.SmtpResponse);
					issueBetweenMsgRset = true;
				}
				else
				{
					ExTraceGlobals.SmtpSendTracer.TraceDebug<string, IPEndPoint>((long)this.GetHashCode(), "Delivered message from {0} to {1}", smtpOutSession.RoutedMailItem.From.ToString(), smtpOutSession.RemoteEndPoint);
					smtpOutSession.AckMessage(AckStatus.Success, base.SmtpResponse, this.bodyStream.Position);
				}
				smtpOutSession.PrepareForNextMessage(issueBetweenMsgRset);
				return;
			}
			if (!string.Equals(statusCode, "354", StringComparison.Ordinal))
			{
				smtpOutSession.AckMessage(AckStatus.Retry, base.SmtpResponse);
				smtpOutSession.PrepareForNextMessage(true);
				return;
			}
			this.sentDataVerb = true;
			base.ParsingStatus = ParsingStatus.MoreDataRequired;
		}

		protected override AsyncReturnType SubmitMessageIfReady()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			if (!this.discardingMessage && base.SmtpResponse.StatusCode[0] == '2')
			{
				return base.SubmitMessage();
			}
			smtpInSession.AbortMailTransaction();
			return AsyncReturnType.Sync;
		}

		protected override void ContinueSubmitMessageIfReady()
		{
			base.EodDone(true);
		}

		protected override void StoreCurrentDataState()
		{
		}

		private SmtpInDataParser smtpInDataParser;

		private bool sentDataVerb;
	}
}
