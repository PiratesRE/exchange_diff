using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MailboxManagementDataAdapter<TObject> : IDisposeTrackable, IDisposable where TObject : UserConfigurationObject, new()
	{
		public MailboxManagementDataAdapter(MailboxSession session, string configuration)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			this.Session = session;
			this.Configuration = configuration;
			this.DisposeTracker = this.GetDisposeTracker();
		}

		protected MailboxSession Session { get; set; }

		protected string Configuration { get; set; }

		private bool Disposed { get; set; }

		private DisposeTracker DisposeTracker { get; set; }

		public TObject Read(IExchangePrincipal principal)
		{
			this.CheckDisposed();
			MailboxManagementDataAdapter<TObject>.CheckPrincipal(principal);
			return this.InternalRead(principal);
		}

		protected virtual TObject InternalRead(IExchangePrincipal principal)
		{
			TObject tobject = Activator.CreateInstance<TObject>();
			tobject.Principal = principal;
			this.InternalFill(tobject);
			tobject.ResetChangeTracking();
			return tobject;
		}

		public void Fill(TObject configObject)
		{
			this.CheckDisposed();
			MailboxManagementDataAdapter<TObject>.CheckPrincipal(configObject.Principal);
			this.InternalFill(configObject);
		}

		protected virtual void InternalFill(TObject configObject)
		{
		}

		public void Delete(IExchangePrincipal principal)
		{
			this.CheckDisposed();
			MailboxManagementDataAdapter<TObject>.CheckPrincipal(principal);
			this.InternalDelete(principal);
		}

		protected virtual void InternalDelete(IExchangePrincipal principal)
		{
		}

		public void Save(TObject configObj)
		{
			this.CheckDisposed();
			if (configObj == null)
			{
				throw new ArgumentNullException("configObj");
			}
			MailboxManagementDataAdapter<TObject>.CheckPrincipal(configObj.Principal);
			this.InternalSave(configObj);
		}

		protected virtual void InternalSave(TObject configObj)
		{
		}

		private static void CheckPrincipal(IExchangePrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			if (principal.ObjectId == null)
			{
				throw new ArgumentNullException("principal", "null == principal.ADObjectId");
			}
			if (Guid.Empty == principal.ObjectId.ObjectGuid)
			{
				throw new ArgumentOutOfRangeException("principal", "Guid.Empty == principal.ADObjectId.ObjectGuid");
			}
		}

		private void CheckDisposed()
		{
			if (this.Disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (this.Session != null)
			{
				this.Session = null;
			}
			if (this.DisposeTracker != null)
			{
				this.DisposeTracker.Dispose();
			}
			this.Disposed = true;
		}

		public void Dispose()
		{
			if (!this.Disposed)
			{
				this.Dispose(true);
			}
			GC.SuppressFinalize(this);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxManagementDataAdapter<TObject>>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.DisposeTracker != null)
			{
				this.DisposeTracker.Suppress();
			}
		}
	}
}
