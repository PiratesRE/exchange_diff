using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync
{
	internal class SecurityContextAndSession : IDisposable
	{
		public SecurityContextAndSession(ClientSecurityContextWrapper wrapper, MailboxSession session)
		{
			if (wrapper == null)
			{
				throw new ArgumentNullException("wrapper");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			this.SecurityContextWrapper = wrapper;
			this.MailboxSession = session;
			this.SecurityContextWrapper.AddRef();
		}

		public ClientSecurityContextWrapper SecurityContextWrapper { get; private set; }

		public MailboxSession MailboxSession { get; private set; }

		public void Dispose()
		{
			this.InternalDispose(true);
		}

		private void InternalDispose(bool fromDispose)
		{
			if (fromDispose && !this.disposed)
			{
				lock (this.lockObject)
				{
					if (!this.disposed)
					{
						try
						{
							this.MailboxSession.Dispose();
							this.MailboxSession = null;
						}
						finally
						{
							this.SecurityContextWrapper.Dispose();
							this.SecurityContextWrapper = null;
							this.disposed = true;
						}
					}
				}
			}
		}

		private bool disposed;

		private object lockObject = new object();
	}
}
