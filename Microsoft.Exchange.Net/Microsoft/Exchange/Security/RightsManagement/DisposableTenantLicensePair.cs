using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DisposableTenantLicensePair : IDisposeTrackable, IDisposable
	{
		internal SafeRightsManagementHandle EnablingPrincipalRac
		{
			get
			{
				this.ThrowIfDisposed();
				return this.tenantLicenses.EnablingPrincipalRac;
			}
		}

		internal SafeRightsManagementHandle BoundLicenseClc
		{
			get
			{
				this.ThrowIfDisposed();
				return this.tenantLicenses.BoundLicenseClc;
			}
		}

		internal XmlNode[] Rac
		{
			get
			{
				this.ThrowIfDisposed();
				return this.tenantLicenses.Rac;
			}
		}

		private DisposableTenantLicensePair(TenantLicensePair licensePair)
		{
			this.tenantLicenses = licensePair;
			this.disposeTracker = this.GetDisposeTracker();
		}

		private void ThrowIfDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("Object already disposed.");
			}
		}

		public static DisposableTenantLicensePair CreateDisposableTenantLicenses(TenantLicensePair licensePair)
		{
			if (licensePair == null)
			{
				throw new ArgumentNullException("licensePair");
			}
			licensePair.AddRef();
			if (!licensePair.IsCleanedUp)
			{
				return new DisposableTenantLicensePair(licensePair);
			}
			licensePair.Release();
			return null;
		}

		public DisposableTenantLicensePair Clone()
		{
			this.ThrowIfDisposed();
			return DisposableTenantLicensePair.CreateDisposableTenantLicenses(this.tenantLicenses);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<DisposableTenantLicensePair>(this);
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
			if (this.disposed)
			{
				return;
			}
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.tenantLicenses != null)
			{
				this.tenantLicenses.Release();
				this.tenantLicenses = null;
			}
			this.disposed = true;
		}

		private DisposeTracker disposeTracker;

		private bool disposed;

		private TenantLicensePair tenantLicenses;
	}
}
