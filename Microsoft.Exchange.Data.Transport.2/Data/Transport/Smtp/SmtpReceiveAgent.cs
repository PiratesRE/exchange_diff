using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public abstract class SmtpReceiveAgent : Agent
	{
		internal event ProcessAuthenticationEventHandler OnProcessAuthentication
		{
			add
			{
				base.AddHandler("OnProcessAuthentication", value);
			}
			remove
			{
				base.RemoveHandler("OnProcessAuthentication");
			}
		}

		internal event ProxyInboundMessageEventHandler OnProxyInboundMessage
		{
			add
			{
				base.AddHandler("OnProxyInboundMessage", value);
			}
			remove
			{
				base.RemoveHandler("OnProxyInboundMessage");
			}
		}

		internal event Rcpt2CommandEventHandler OnRcpt2Command
		{
			add
			{
				base.AddHandler("OnRcpt2Command", value);
			}
			remove
			{
				base.RemoveHandler("OnRcpt2Command");
			}
		}

		internal event XSessionParamsCommandEventHandler OnXSessionParamsCommand
		{
			add
			{
				base.AddHandler("OnXSessionParamsCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnXSessionParamsCommand");
			}
		}

		protected event AuthCommandEventHandler OnAuthCommand
		{
			add
			{
				base.AddHandler("OnAuthCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnAuthCommand");
			}
		}

		protected event DataCommandEventHandler OnDataCommand
		{
			add
			{
				base.AddHandler("OnDataCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnDataCommand");
			}
		}

		protected event EhloCommandEventHandler OnEhloCommand
		{
			add
			{
				base.AddHandler("OnEhloCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnEhloCommand");
			}
		}

		protected event EndOfAuthenticationEventHandler OnEndOfAuthentication
		{
			add
			{
				base.AddHandler("OnEndOfAuthentication", value);
			}
			remove
			{
				base.RemoveHandler("OnEndOfAuthentication");
			}
		}

		protected event EndOfDataEventHandler OnEndOfData
		{
			add
			{
				base.AddHandler("OnEndOfData", value);
			}
			remove
			{
				base.RemoveHandler("OnEndOfData");
			}
		}

		protected event EndOfHeadersEventHandler OnEndOfHeaders
		{
			add
			{
				base.AddHandler("OnEndOfHeaders", value);
			}
			remove
			{
				base.RemoveHandler("OnEndOfHeaders");
			}
		}

		protected event HeloCommandEventHandler OnHeloCommand
		{
			add
			{
				base.AddHandler("OnHeloCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnHeloCommand");
			}
		}

		protected event HelpCommandEventHandler OnHelpCommand
		{
			add
			{
				base.AddHandler("OnHelpCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnHelpCommand");
			}
		}

		protected event MailCommandEventHandler OnMailCommand
		{
			add
			{
				base.AddHandler("OnMailCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnMailCommand");
			}
		}

		protected event NoopCommandEventHandler OnNoopCommand
		{
			add
			{
				base.AddHandler("OnNoopCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnNoopCommand");
			}
		}

		protected event RcptCommandEventHandler OnRcptCommand
		{
			add
			{
				base.AddHandler("OnRcptCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnRcptCommand");
			}
		}

		protected event RejectEventHandler OnReject
		{
			add
			{
				base.AddHandler("OnReject", value);
			}
			remove
			{
				base.RemoveHandler("OnReject");
			}
		}

		protected event RsetCommandEventHandler OnRsetCommand
		{
			add
			{
				base.AddHandler("OnRsetCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnRsetCommand");
			}
		}

		protected event ConnectEventHandler OnConnect
		{
			add
			{
				base.AddHandler("OnConnectEvent", value);
			}
			remove
			{
				base.RemoveHandler("OnConnectEvent");
			}
		}

		protected event DisconnectEventHandler OnDisconnect
		{
			add
			{
				base.AddHandler("OnDisconnectEvent", value);
			}
			remove
			{
				base.RemoveHandler("OnDisconnectEvent");
			}
		}

		protected event StartTlsEventHandler OnStartTlsCommand
		{
			add
			{
				base.AddHandler("OnStartTlsCommand", value);
			}
			remove
			{
				base.RemoveHandler("OnStartTlsCommand");
			}
		}

		internal override string SnapshotPrefix
		{
			get
			{
				return "SmtpReceive";
			}
		}

		internal override object HostState
		{
			get
			{
				return base.HostStateInternal;
			}
			set
			{
				base.HostStateInternal = value;
				((SmtpServer)base.HostStateInternal).AssociatedAgent = this;
			}
		}

		internal override void AsyncComplete()
		{
			if (base.EventTopic == "OnEndOfData")
			{
				base.EnsureMimeWriteStreamClosed();
				base.MailItem = null;
			}
			((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
			base.EventArgId = null;
			base.SnapshotWriter = null;
		}

		internal override void Invoke(string eventTopic, object source, object e)
		{
			SnapshotWriter snapshotWriter = null;
			base.MailItem = SmtpReceiveAgent.GetMailItem((EventArgs)e);
			if (base.MailItem != null && base.MailItem.SnapshotWriter != null && base.MailItem.PipelineTracingEnabled && base.SnapshotEnabled)
			{
				base.SnapshotWriter = base.MailItem.SnapshotWriter;
				this.WriteOriginalDataSnapshot(source, (EventArgs)e, eventTopic);
				snapshotWriter = base.SnapshotWriter;
				base.SnapshotWriter = null;
			}
			Delegate @delegate = (Delegate)base.Handlers[eventTopic];
			if (@delegate == null)
			{
				return;
			}
			switch (eventTopic)
			{
			case "OnAuthCommand":
				((AuthCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (AuthCommandEventArgs)e);
				break;
			case "OnDataCommand":
				if (!((ReceiveCommandEventSource)source).SmtpSession.DiscardingMessage)
				{
					((SmtpServer)this.HostState).AddressBook.RecipientCache = base.MailItem.RecipientCache;
					((DataCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (DataCommandEventArgs)e);
				}
				break;
			case "OnEhloCommand":
				((EhloCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (EhloCommandEventArgs)e);
				break;
			case "OnProcessAuthentication":
				((ProcessAuthenticationEventHandler)@delegate)((ReceiveCommandEventSource)source, (ProcessAuthenticationEventArgs)e);
				break;
			case "OnEndOfAuthentication":
				((EndOfAuthenticationEventHandler)@delegate)((EndOfAuthenticationEventSource)source, (EndOfAuthenticationEventArgs)e);
				break;
			case "OnEndOfData":
				if (!((ReceiveMessageEventSource)source).SmtpSession.DiscardingMessage)
				{
					base.SnapshotWriter = snapshotWriter;
					((SmtpServer)this.HostState).AddressBook.RecipientCache = base.MailItem.RecipientCache;
					base.EventArgId = ((ReceiveEventSource)source).SmtpSession.CurrentMessageTemporaryId;
					if (base.SnapshotWriter != null)
					{
						base.SnapshotWriter.WritePreProcessedData(this.GetHashCode(), "SmtpReceive", base.EventArgId, eventTopic, base.MailItem);
					}
					((EndOfDataEventHandler)@delegate)((ReceiveMessageEventSource)source, (EndOfDataEventArgs)e);
					if (base.Synchronous)
					{
						base.EnsureMimeWriteStreamClosed();
						if (base.SnapshotWriter != null)
						{
							base.SnapshotWriter.WriteProcessedData("SmtpReceive", base.EventArgId, eventTopic, base.Name, base.MailItem);
						}
					}
				}
				break;
			case "OnEndOfHeaders":
				if (!((ReceiveMessageEventSource)source).SmtpSession.DiscardingMessage)
				{
					base.SnapshotWriter = snapshotWriter;
					((SmtpServer)this.HostState).AddressBook.RecipientCache = base.MailItem.RecipientCache;
					base.EventArgId = ((ReceiveEventSource)source).SmtpSession.CurrentMessageTemporaryId;
					if (base.SnapshotWriter != null)
					{
						base.SnapshotWriter.WritePreProcessedData(this.GetHashCode(), "SmtpReceive", base.EventArgId, eventTopic, base.MailItem);
					}
					((EndOfHeadersEventHandler)@delegate)((ReceiveMessageEventSource)source, (EndOfHeadersEventArgs)e);
					if (base.Synchronous && base.SnapshotWriter != null)
					{
						ReceiveMessageEventSource receiveMessageEventSource = (ReceiveMessageEventSource)source;
						base.SnapshotWriter.WriteProcessedData("SmtpReceive", base.EventArgId, eventTopic, base.Name, base.MailItem);
					}
				}
				break;
			case "OnHeloCommand":
				((HeloCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (HeloCommandEventArgs)e);
				break;
			case "OnHelpCommand":
				((HelpCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (HelpCommandEventArgs)e);
				break;
			case "OnMailCommand":
				((MailCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (MailCommandEventArgs)e);
				break;
			case "OnNoopCommand":
				((NoopCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (NoopCommandEventArgs)e);
				break;
			case "OnRcptCommand":
				((SmtpServer)this.HostState).AddressBook.RecipientCache = base.MailItem.RecipientCache;
				((RcptCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (RcptCommandEventArgs)e);
				break;
			case "OnRcpt2Command":
				((SmtpServer)this.HostState).AddressBook.RecipientCache = base.MailItem.RecipientCache;
				((Rcpt2CommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (Rcpt2CommandEventArgs)e);
				break;
			case "OnReject":
				((RejectEventHandler)@delegate)((RejectEventSource)source, (RejectEventArgs)e);
				break;
			case "OnRsetCommand":
				((RsetCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (RsetCommandEventArgs)e);
				break;
			case "OnConnectEvent":
				((ConnectEventHandler)@delegate)((ConnectEventSource)source, (ConnectEventArgs)e);
				break;
			case "OnDisconnectEvent":
				((DisconnectEventHandler)@delegate)((DisconnectEventSource)source, (DisconnectEventArgs)e);
				break;
			case "OnStartTlsCommand":
				((StartTlsEventHandler)@delegate)((ReceiveCommandEventSource)source, (StartTlsCommandEventArgs)e);
				break;
			case "OnProxyInboundMessage":
				((ProxyInboundMessageEventHandler)@delegate)((ProxyInboundMessageEventSource)source, (ProxyInboundMessageEventArgs)e);
				break;
			case "OnXSessionParamsCommand":
				((XSessionParamsCommandEventHandler)@delegate)((ReceiveCommandEventSource)source, (XSessionParamsCommandEventArgs)e);
				break;
			}
			if (base.Synchronous)
			{
				((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
				base.EventArgId = null;
				base.SnapshotWriter = null;
				base.MailItem = null;
			}
		}

		private static MailItem GetMailItem(EventArgs e)
		{
			RcptCommandEventArgs rcptCommandEventArgs = e as RcptCommandEventArgs;
			if (rcptCommandEventArgs != null)
			{
				return rcptCommandEventArgs.MailItem;
			}
			Rcpt2CommandEventArgs rcpt2CommandEventArgs = e as Rcpt2CommandEventArgs;
			if (rcpt2CommandEventArgs != null)
			{
				return rcpt2CommandEventArgs.MailItem;
			}
			DataCommandEventArgs dataCommandEventArgs = e as DataCommandEventArgs;
			if (dataCommandEventArgs != null)
			{
				return dataCommandEventArgs.MailItem;
			}
			EndOfDataEventArgs endOfDataEventArgs = e as EndOfDataEventArgs;
			if (endOfDataEventArgs != null)
			{
				return endOfDataEventArgs.MailItem;
			}
			EndOfHeadersEventArgs endOfHeadersEventArgs = e as EndOfHeadersEventArgs;
			if (endOfHeadersEventArgs != null)
			{
				return endOfHeadersEventArgs.MailItem;
			}
			return null;
		}

		private void WriteOriginalDataSnapshot(object source, EventArgs e, string eventTopic)
		{
			string currentMessageTemporaryId = ((ReceiveEventSource)source).SmtpSession.CurrentMessageTemporaryId;
			string address = null;
			if (eventTopic == "OnRcptCommand")
			{
				address = ((RcptCommandEventArgs)e).RecipientAddress.ToString();
			}
			base.SnapshotWriter.WriteOriginalData(this.GetHashCode(), currentMessageTemporaryId, eventTopic, address, base.MailItem);
		}

		private const string SnapshotFileNamePrefix = "SmtpReceive";
	}
}
