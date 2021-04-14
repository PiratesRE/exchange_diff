using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class RemoteObject : DisposeTrackableBase
	{
		protected RemoteObject(IMailboxReplicationProxyService mrsProxy, long handle)
		{
			this.mrsProxy = mrsProxy;
			this.handle = handle;
		}

		public long Handle
		{
			get
			{
				return this.handle;
			}
			protected set
			{
				this.handle = value;
			}
		}

		public IMailboxReplicationProxyService MrsProxy
		{
			get
			{
				return this.mrsProxy;
			}
			protected set
			{
				this.mrsProxy = value;
			}
		}

		public VersionInformation ServerVersion
		{
			get
			{
				return ((MailboxReplicationProxyClient)this.mrsProxy).ServerVersion;
			}
		}

		public MailboxReplicationProxyClient MrsProxyClient
		{
			get
			{
				return (MailboxReplicationProxyClient)this.mrsProxy;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.mrsProxy != null)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.mrsProxy.CloseHandle(this.handle);
				}, null);
				this.mrsProxy = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RemoteObject>(this);
		}

		private IMailboxReplicationProxyService mrsProxy;

		private long handle;
	}
}
