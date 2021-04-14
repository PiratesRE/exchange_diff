using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;

namespace Microsoft.Exchange.HttpProxy
{
	internal class TargetCalculationCallbackBeacon
	{
		public TargetCalculationCallbackBeacon(AnchoredRoutingTarget anchoredRoutingTarget)
		{
			if (anchoredRoutingTarget == null)
			{
				throw new ArgumentNullException("anchoredRoutingTarget");
			}
			this.AnchoredRoutingTarget = anchoredRoutingTarget;
			this.AnchorMailbox = this.AnchoredRoutingTarget.AnchorMailbox;
			this.State = TargetCalculationCallbackState.TargetResolved;
		}

		public TargetCalculationCallbackBeacon(AnchorMailbox anchorMailbox, BackEndServer mailboxServer)
		{
			if (anchorMailbox == null)
			{
				throw new ArgumentNullException("anchorMailbox");
			}
			if (mailboxServer == null)
			{
				throw new ArgumentNullException("mailboxServer");
			}
			this.AnchorMailbox = anchorMailbox;
			this.MailboxServer = mailboxServer;
			this.State = TargetCalculationCallbackState.MailboxServerResolved;
		}

		public TargetCalculationCallbackBeacon(MailboxServerLocatorAsyncState mailboxServerLocatorAsyncState, IAsyncResult mailboxServerLocatorAsyncResult)
		{
			if (mailboxServerLocatorAsyncState == null)
			{
				throw new ArgumentNullException("mailboxServerLocatorAsyncState");
			}
			if (mailboxServerLocatorAsyncResult == null)
			{
				throw new ArgumentNullException("mailboxServerLocatorAsyncResult");
			}
			this.MailboxServerLocatorAsyncState = mailboxServerLocatorAsyncState;
			this.MailboxServerLocatorAsyncResult = mailboxServerLocatorAsyncResult;
			this.AnchorMailbox = this.MailboxServerLocatorAsyncState.AnchorMailbox;
			this.State = TargetCalculationCallbackState.LocatorCallback;
		}

		public TargetCalculationCallbackState State { get; private set; }

		public AnchoredRoutingTarget AnchoredRoutingTarget { get; private set; }

		public AnchorMailbox AnchorMailbox { get; private set; }

		public BackEndServer MailboxServer { get; private set; }

		public MailboxServerLocatorAsyncState MailboxServerLocatorAsyncState { get; private set; }

		public IAsyncResult MailboxServerLocatorAsyncResult { get; private set; }
	}
}
