using System;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XQDiscardSmtpCommand : SmtpCommand
	{
		public XQDiscardSmtpCommand(ISmtpSession session, IShadowRedundancyManager shadowRedundancyManager) : base(session, "XQDISCARD", null, LatencyComponent.None)
		{
			this.shadowRedundancyManager = shadowRedundancyManager;
		}

		internal override void InboundParseCommand()
		{
			this.ParseCommand();
			if (base.ParsingStatus != ParsingStatus.Complete)
			{
				ShadowRedundancyManager.ReceiveTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "XQDISCARD parsing failed; SMTP Response: {0}", base.SmtpResponse);
				return;
			}
			ShadowRedundancyManager.ReceiveTracer.TraceDebug<int>((long)this.GetHashCode(), "XQDISCARD parsing completed; Max discard count = {0}", this.maxDiscardCount);
		}

		internal override void InboundProcessCommand()
		{
			if (base.ParsingStatus != ParsingStatus.Complete)
			{
				return;
			}
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XQDiscardInboundProcessCommand);
			string[] array = this.shadowRedundancyManager.QueryDiscardEvents(smtpInSession.SenderShadowContext, this.maxDiscardCount);
			if (array == null)
			{
				throw new InvalidOperationException("discardIds array not expected to be null");
			}
			string statusCode;
			if (array.Length == 0)
			{
				array = XQDiscardSmtpCommand.EmptyDiscardListResponseText;
				statusCode = "251";
			}
			else
			{
				statusCode = "250";
			}
			base.SmtpResponse = new SmtpResponse(statusCode, string.Empty, array);
			ShadowRedundancyManager.ReceiveTracer.TraceDebug<int>((long)this.GetHashCode(), "XQDISCARD command responded with discard count of {0}", array.Length);
		}

		internal override void OutboundCreateCommand()
		{
		}

		internal override void OutboundFormatCommand()
		{
			base.ProtocolCommandString = string.Format("XQDISCARD {0}", this.shadowRedundancyManager.Configuration.MaxDiscardIdsPerSmtpCommand);
			ShadowRedundancyManager.SendTracer.TraceDebug<string>((long)this.GetHashCode(), "Formatted command : {0}", base.ProtocolCommandString);
		}

		internal override void OutboundProcessResponse()
		{
			SmtpOutSession smtpOutSession = (SmtpOutSession)base.SmtpSession;
			bool flag = true;
			bool flag2 = false;
			string smtpHost = smtpOutSession.SmtpHost;
			if (base.SmtpResponse.SmtpResponseType != SmtpResponseType.Success)
			{
				ShadowRedundancyManager.SendTracer.TraceError<SmtpResponse>((long)this.GetHashCode(), "XQDISCARD rejected by the remote server: {0}", base.SmtpResponse);
			}
			else if ("251".Equals(base.SmtpResponse.StatusCode))
			{
				ShadowRedundancyManager.SendTracer.TraceDebug((long)this.GetHashCode(), "XQDISCARD accepted by the remote server. Empty discard list received.");
				flag2 = true;
			}
			else if (base.SmtpResponse.StatusText == null)
			{
				ShadowRedundancyManager.SendTracer.TraceDebug((long)this.GetHashCode(), "XQDISCARD accepted by the remote server. Received empty response.");
			}
			else
			{
				ShadowRedundancyManager.SendTracer.TraceDebug((long)this.GetHashCode(), "XQDISCARD accepted by the remote server. Parsing the response.");
				bool flag3 = !this.ValidateDiscardIds();
				if (!flag3)
				{
					int num;
					int num2;
					this.shadowRedundancyManager.ApplyDiscardEvents(smtpHost, smtpOutSession.NextHopConnection.Key, base.SmtpResponse.StatusText, out num, out num2);
					smtpOutSession.DiscardIdsReceived += (ulong)((long)base.SmtpResponse.StatusText.Length);
					if (num > 0)
					{
						ShadowRedundancyManager.SendTracer.TraceError<int>((long)this.GetHashCode(), "XQDISCARD response contains {0} invalid discard IDs", num);
						flag3 = true;
					}
				}
				if (flag3)
				{
					smtpOutSession.FailoverConnection(base.SmtpResponse);
					smtpOutSession.SetNextStateToQuit();
					this.shadowRedundancyManager.NotifyServerViolatedSmtpContract(smtpHost);
					return;
				}
				flag2 = true;
				if (base.SmtpResponse.StatusText.Length >= this.shadowRedundancyManager.Configuration.MaxDiscardIdsPerSmtpCommand && this.shadowRedundancyManager.ShouldSmtpOutSendXQDiscard(smtpHost))
				{
					flag = false;
				}
			}
			if (flag)
			{
				ShadowRedundancyManager.SendTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "XQDISCARD response processed. Heartbeat to server {0} succeeded: {1}", smtpHost, flag2);
				this.shadowRedundancyManager.NotifyHeartbeatStatus(smtpHost, smtpOutSession.NextHopConnection.Key, flag2);
				smtpOutSession.SetNextStateToQuit();
				return;
			}
			ShadowRedundancyManager.SendTracer.TraceDebug((long)this.GetHashCode(), "XQDISCARD response processed; Will issue another XQDISCARD");
			smtpOutSession.NextState = SmtpOutSession.SessionState.XQDiscard;
		}

		private void ParseCommand()
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)base.SmtpSession;
			smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XQDiscardInboundParseCommand);
			if (!smtpInSession.AdvertisedEhloOptions.XShadow && !smtpInSession.AdvertisedEhloOptions.XShadowRequest)
			{
				base.SmtpResponse = SmtpResponse.CommandNotImplemented;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.XShadowNotEnabled);
				return;
			}
			if (!smtpInSession.IsShadowedBySender)
			{
				base.SmtpResponse = SmtpResponse.BadCommandSequence;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				smtpInSession.DropBreadcrumb(SmtpInSessionBreadcrumbs.WrongSequence);
				return;
			}
			string nextArg = base.GetNextArg();
			if (string.IsNullOrEmpty(nextArg))
			{
				base.SmtpResponse = SmtpResponse.XQDiscardMaxDiscardCountRequired;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			if (!int.TryParse(nextArg, out this.maxDiscardCount) || this.maxDiscardCount <= 0)
			{
				base.SmtpResponse = SmtpResponse.InvalidArguments;
				base.ParsingStatus = ParsingStatus.ProtocolError;
				return;
			}
			base.ParsingStatus = ParsingStatus.Complete;
		}

		private bool ValidateDiscardIds()
		{
			string[] statusText = base.SmtpResponse.StatusText;
			int i = 0;
			while (i < statusText.Length)
			{
				string text = statusText[i];
				bool result;
				if (string.IsNullOrEmpty(text))
				{
					ShadowRedundancyManager.SendTracer.TraceError((long)this.GetHashCode(), "XQDISCARD response contains a null or empty discard ID");
					result = false;
				}
				else
				{
					if (text.Length <= 255)
					{
						i++;
						continue;
					}
					ShadowRedundancyManager.SendTracer.TraceError<string>((long)this.GetHashCode(), "XQDISCARD response contains a discard ID that exceeds max allowed length: {0}", text);
					result = false;
				}
				return result;
			}
			return true;
		}

		private const int MaxDiscardIdLen = 255;

		private const string OkResponseCode = "250";

		private const string EmptyDiscardListResponseCode = "251";

		private const string XQDiscardCommandFormatString = "XQDISCARD {0}";

		private static readonly string[] EmptyDiscardListResponseText = new string[]
		{
			"OK, no discard events"
		};

		private readonly IShadowRedundancyManager shadowRedundancyManager;

		private int maxDiscardCount;
	}
}
