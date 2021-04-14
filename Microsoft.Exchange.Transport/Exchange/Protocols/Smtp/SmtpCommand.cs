using System;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal abstract class SmtpCommand : ILegacySmtpCommand, IDisposeTrackable, IDisposable
	{
		protected SmtpCommand(ISmtpSession session, string protocolCommandKeyword, string commandEventString, LatencyComponent commandEventComponent)
		{
			this.session = session;
			this.commandEventString = commandEventString;
			this.commandEventComponent = commandEventComponent;
			this.isResponseReady = true;
			this.protocolCommandKeyword = protocolCommandKeyword;
			ISmtpInSession smtpInSession = this.session as ISmtpInSession;
			if (smtpInSession != null)
			{
				this.tarpitInterval = smtpInSession.Connector.TarpitInterval;
			}
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal SmtpCommand(string protocolCommandKeyword)
		{
			this.protocolCommandKeyword = protocolCommandKeyword;
		}

		internal bool IsResponseReady
		{
			get
			{
				return this.isResponseReady;
			}
			set
			{
				this.isResponseReady = value;
			}
		}

		protected internal TarpitAction LowAuthenticationLevelTarpitOverride
		{
			get
			{
				return this.lowAuthenticationLevelTarpitOverride;
			}
			set
			{
				this.lowAuthenticationLevelTarpitOverride = value;
			}
		}

		protected internal TarpitAction HighAuthenticationLevelTarpitOverride
		{
			get
			{
				return this.highAuthenticationLevelTarpitOverride;
			}
			set
			{
				this.highAuthenticationLevelTarpitOverride = value;
			}
		}

		protected internal TimeSpan TarpitInterval
		{
			get
			{
				return this.tarpitInterval;
			}
			set
			{
				this.tarpitInterval = value;
			}
		}

		protected internal string TarpitReason
		{
			get
			{
				return this.tarpitReason ?? string.Empty;
			}
			set
			{
				this.tarpitReason = value;
			}
		}

		protected internal string TarpitContext
		{
			get
			{
				return this.tarpitContext;
			}
			set
			{
				this.tarpitContext = value;
			}
		}

		public byte[] ProtocolCommand
		{
			get
			{
				return this.protocolCommand;
			}
			set
			{
				this.protocolCommand = value;
				if (this.protocolCommand != null)
				{
					this.protocolCommandLength = this.protocolCommand.Length;
					return;
				}
				this.protocolCommandLength = 0;
			}
		}

		public int ProtocolCommandLength
		{
			get
			{
				return this.protocolCommandLength;
			}
		}

		internal string ProtocolCommandKeyword
		{
			get
			{
				return this.protocolCommandKeyword;
			}
		}

		public int CurrentOffset
		{
			get
			{
				return this.currentOffset;
			}
			set
			{
				this.currentOffset = value;
			}
		}

		internal string ProtocolCommandString
		{
			get
			{
				return this.protocolCommandString;
			}
			set
			{
				this.protocolCommandString = value;
			}
		}

		internal string RedactedProtocolCommandString
		{
			get
			{
				return this.redactedProtocolCommandString;
			}
			set
			{
				this.redactedProtocolCommandString = value;
			}
		}

		internal SmtpResponse SmtpResponse
		{
			get
			{
				return this.smtpResponse;
			}
			set
			{
				this.smtpResponse = value;
			}
		}

		internal bool IsResponseBuffered
		{
			get
			{
				return this.isResponseBuffered;
			}
			set
			{
				this.isResponseBuffered = value;
			}
		}

		internal ParsingStatus ParsingStatus
		{
			get
			{
				return this.parsingStatus;
			}
			set
			{
				this.parsingStatus = value;
			}
		}

		internal ISmtpSession SmtpSession
		{
			get
			{
				return this.session;
			}
		}

		internal EventArgs OriginalEventArgsWrapper
		{
			get
			{
				return this.originalEventArgsWrapper;
			}
		}

		internal LatencyComponent CommandEventComponent
		{
			get
			{
				return this.commandEventComponent;
			}
		}

		protected bool OnlyCheckMessageSizeAfterEoh
		{
			get
			{
				ISmtpInSession smtpInSession = (ISmtpInSession)this.SmtpSession;
				return SmtpInSessionUtils.HasSMTPAcceptOrgHeadersPermission(smtpInSession.Permissions) || (smtpInSession.SmtpInServer.IsBridgehead && !SmtpInSessionUtils.IsAnonymous(smtpInSession.RemoteIdentity));
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SmtpCommand>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual IAsyncResult BeginRaiseEvent(AsyncCallback callback, object state)
		{
			ISmtpInSession smtpInSession = (ISmtpInSession)this.SmtpSession;
			if (this.commandEventString == null)
			{
				return smtpInSession.AgentSession.BeginNoEvent(callback, state);
			}
			if (this.CommandEventArgs != null)
			{
				this.CommandEventArgs.Initialize(smtpInSession.SessionSource);
				this.originalEventArgsWrapper = this.CommandEventArgs;
			}
			return smtpInSession.AgentSession.BeginRaiseEvent(this.commandEventString, ReceiveCommandEventSourceImpl.Create(smtpInSession.SessionSource), this.originalEventArgsWrapper, callback, state);
		}

		internal static bool CompareArg(byte[] byteEncodedStrBuf, byte[] buffer, int beginOffset, int count)
		{
			return BufferParser.CompareArg(byteEncodedStrBuf, buffer, beginOffset, count);
		}

		internal static string GetBracketedString(string emailAddress)
		{
			if (emailAddress.Length >= 2 && emailAddress[0] == '<' && emailAddress[emailAddress.Length - 1] == '>')
			{
				return emailAddress;
			}
			return "<" + emailAddress + ">";
		}

		internal abstract void InboundParseCommand();

		internal virtual void InboundAgentEventCompleted()
		{
		}

		internal abstract void InboundProcessCommand();

		internal virtual void InboundCompleteCommand()
		{
		}

		internal abstract void OutboundCreateCommand();

		internal abstract void OutboundFormatCommand();

		internal abstract void OutboundProcessResponse();

		internal bool VerifyHelloReceived()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && !smtpInSession.SeenHelo)
			{
				this.SmtpResponse = SmtpResponse.NeedHello;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		internal bool VerifyEhloReceived()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && !smtpInSession.SeenEhlo)
			{
				this.SmtpResponse = SmtpResponse.NeedEhlo;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		internal bool VerifyMailFromReceived()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && smtpInSession.TransportMailItem == null)
			{
				this.SmtpResponse = SmtpResponse.NeedMailFrom;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		internal bool VerifyRcptToReceived()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && smtpInSession.TransportMailItem.Recipients.Count == 0)
			{
				this.SmtpResponse = SmtpResponse.NeedRcptTo;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		internal bool VerifyNoOngoingBdat()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && smtpInSession.IsBdatOngoing)
			{
				this.SmtpResponse = SmtpResponse.BadCommandSequence;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		internal string GetNextArg()
		{
			int num;
			int num2;
			if (!this.GetNextArgOffsets(out num, out num2))
			{
				return string.Empty;
			}
			return ByteString.BytesToString(this.protocolCommand, num, num2 - num, true);
		}

		internal bool GetNextArgOffsets(out int argBeginOffset, out int argEndOffset)
		{
			argBeginOffset = 0;
			argEndOffset = 0;
			if (this.IsEndOfCommand())
			{
				return false;
			}
			argBeginOffset = BufferParser.GetNextToken(this.protocolCommand, this.currentOffset, this.protocolCommand.Length - this.currentOffset, out argEndOffset);
			this.currentOffset = argEndOffset;
			return argBeginOffset != argEndOffset;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
		}

		protected bool VerifyNotAuthenticatedThroughAuthVerb()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && SmtpInSessionUtils.IsAuthenticated(smtpInSession.RemoteIdentity) && (smtpInSession.AuthMethod == MultilevelAuthMechanism.TLSAuthLogin || smtpInSession.AuthMethod == MultilevelAuthMechanism.Login || smtpInSession.AuthMethod == MultilevelAuthMechanism.NTLM || smtpInSession.AuthMethod == MultilevelAuthMechanism.GSSAPI || smtpInSession.AuthMethod == MultilevelAuthMechanism.MUTUALGSSAPI))
			{
				this.SmtpResponse = SmtpResponse.AuthAlreadySpecified;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		protected bool VerifyNotAuthenticatedThroughAuthLoginVerb()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && SmtpInSessionUtils.IsAuthenticated(smtpInSession.RemoteIdentity) && (smtpInSession.AuthMethod == MultilevelAuthMechanism.TLSAuthLogin || smtpInSession.AuthMethod == MultilevelAuthMechanism.Login))
			{
				this.SmtpResponse = SmtpResponse.AuthAlreadySpecified;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		protected bool VerifyNotAuthenticatedForInboundClientProxy()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && smtpInSession.InboundClientProxyState == InboundClientProxyStates.XProxyReceivedAndAuthenticated)
			{
				this.SmtpResponse = SmtpResponse.AuthAlreadySpecified;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		protected bool VerifyXexch50NotReceived()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && smtpInSession.IsXexch50Received)
			{
				this.SmtpResponse = SmtpResponse.BadCommandSequence;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		protected bool VerifyNoOngoingMailTransaction()
		{
			ISmtpInSession smtpInSession = this.SmtpSession as ISmtpInSession;
			if (smtpInSession != null && smtpInSession.TransportMailItem != null)
			{
				this.SmtpResponse = SmtpResponse.BadCommandSequence;
				this.ParsingStatus = ParsingStatus.ProtocolError;
				return false;
			}
			return true;
		}

		protected bool IsEndOfCommand()
		{
			return this.currentOffset >= this.protocolCommandLength;
		}

		internal const int DefaultCommandLength = 4;

		internal static readonly byte[] AUTH = Util.AsciiStringToBytes("AUTH".ToLowerInvariant());

		internal static readonly byte[] EXPS = Util.AsciiStringToBytes("X-EXPS".ToLowerInvariant());

		internal static readonly byte[] BDAT = Util.AsciiStringToBytes("BDAT".ToLowerInvariant());

		internal static readonly byte[] DATA = Util.AsciiStringToBytes("DATA".ToLowerInvariant());

		internal static readonly byte[] EHLO = Util.AsciiStringToBytes("EHLO".ToLowerInvariant());

		internal static readonly byte[] HELO = Util.AsciiStringToBytes("HELO".ToLowerInvariant());

		internal static readonly byte[] HELP = Util.AsciiStringToBytes("HELP".ToLowerInvariant());

		internal static readonly byte[] MAIL = Util.AsciiStringToBytes("MAIL".ToLowerInvariant());

		internal static readonly byte[] NOOP = Util.AsciiStringToBytes("NOOP".ToLowerInvariant());

		internal static readonly byte[] QUIT = Util.AsciiStringToBytes("QUIT".ToLowerInvariant());

		internal static readonly byte[] RCPT = Util.AsciiStringToBytes("RCPT".ToLowerInvariant());

		internal static readonly byte[] RCPT2 = Util.AsciiStringToBytes("RCPT2".ToLowerInvariant());

		internal static readonly byte[] RSET = Util.AsciiStringToBytes("RSET".ToLowerInvariant());

		internal static readonly byte[] STARTTLS = Util.AsciiStringToBytes("STARTTLS".ToLowerInvariant());

		internal static readonly byte[] ANONYMOUSTLS = Util.AsciiStringToBytes("X-ANONYMOUSTLS".ToLowerInvariant());

		internal static readonly byte[] VRFY = Util.AsciiStringToBytes("VRFY".ToLowerInvariant());

		internal static readonly byte[] EXPN = Util.AsciiStringToBytes("EXPN".ToLowerInvariant());

		internal static readonly byte[] XEXCH50 = Util.AsciiStringToBytes("XEXCH50".ToLowerInvariant());

		internal static readonly byte[] XSHADOW = Util.AsciiStringToBytes("XSHADOW".ToLowerInvariant());

		internal static readonly byte[] XSHADOWREQUEST = Util.AsciiStringToBytes("XSHADOWREQUEST".ToLowerInvariant());

		internal static readonly byte[] XQDISCARD = Util.AsciiStringToBytes("XQDISCARD".ToLowerInvariant());

		internal static readonly byte[] XPROXY = Util.AsciiStringToBytes("XPROXY".ToLowerInvariant());

		internal static readonly byte[] XPROXYFROM = Util.AsciiStringToBytes("XPROXYFROM".ToLowerInvariant());

		internal static readonly byte[] XSESSIONPARAMS = Util.AsciiStringToBytes("XSESSIONPARAMS".ToLowerInvariant());

		internal static readonly byte[] XPROXYTO = Util.AsciiStringToBytes("XPROXYTO".ToLowerInvariant());

		internal static ExEventLog EventLogger = new ExEventLog(ExTraceGlobals.SmtpSendTracer.Category, TransportEventLog.GetEventSource());

		internal static IExEventLog EventLog = new ExEventLogWrapper(SmtpCommand.EventLogger);

		internal ReceiveCommandEventArgs CommandEventArgs;

		protected EventArgs originalEventArgsWrapper;

		private readonly DisposeTracker disposeTracker;

		private byte[] protocolCommand;

		private readonly string protocolCommandKeyword;

		private int currentOffset;

		private int protocolCommandLength;

		private string protocolCommandString;

		private string redactedProtocolCommandString;

		private SmtpResponse smtpResponse = SmtpResponse.Empty;

		private ParsingStatus parsingStatus;

		private bool isResponseReady;

		private bool isResponseBuffered;

		private TarpitAction lowAuthenticationLevelTarpitOverride;

		private TarpitAction highAuthenticationLevelTarpitOverride;

		private TimeSpan tarpitInterval;

		private string tarpitReason;

		private string tarpitContext;

		private readonly ISmtpSession session;

		private readonly string commandEventString;

		private readonly LatencyComponent commandEventComponent;
	}
}
