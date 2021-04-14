using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RightsSignatureBuilder : IDisposeTrackable, IDisposable
	{
		public RightsSignatureBuilder(string useLicense, string publishLicense, SafeRightsManagementEnvironmentHandle environmentHandle, DisposableTenantLicensePair tenantLicenses)
		{
			if (string.IsNullOrEmpty(useLicense))
			{
				throw new ArgumentNullException("useLicense");
			}
			if (string.IsNullOrEmpty(publishLicense))
			{
				throw new ArgumentNullException("publishLicense");
			}
			if (environmentHandle == null)
			{
				throw new ArgumentNullException("environmentHandle");
			}
			if (tenantLicenses == null)
			{
				throw new ArgumentNullException("tenantLicenses");
			}
			this.publishLicense = publishLicense;
			this.useLicense = useLicense;
			this.environmentHandle = environmentHandle;
			this.tenantLicensePair = tenantLicenses.Clone();
			this.disposeTracker = this.GetDisposeTracker();
		}

		public byte[] Sign(ContentRight rights, ExDateTime expiryTime, SecurityIdentifier sid)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			if (this.disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			this.EnsureInitialized();
			return DrmClientUtils.SignDRMProps(rights, expiryTime, sid, this.encryptor, this.decryptor);
		}

		public SafeRightsManagementHandle GetDuplicateDecryptorHandle()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			if (this.decryptor == null)
			{
				throw new InvalidOperationException("Decryptor handle not created");
			}
			SafeRightsManagementHandle result;
			int hr = SafeNativeMethods.DRMDuplicateHandle(this.decryptor, out result);
			Errors.ThrowOnErrorCode(hr);
			return result;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RightsSignatureBuilder>(this);
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
			if (this.tenantLicensePair != null)
			{
				this.tenantLicensePair.Dispose();
				this.tenantLicensePair = null;
			}
			if (this.encryptor != null)
			{
				this.encryptor.Close();
				this.encryptor = null;
			}
			if (this.decryptor != null)
			{
				this.decryptor.Close();
				this.decryptor = null;
			}
			this.disposed = true;
		}

		private void EnsureInitialized()
		{
			if (this.initialized)
			{
				return;
			}
			DrmClientUtils.BindUseLicense(this.useLicense, this.publishLicense, "EDIT", true, this.tenantLicensePair.EnablingPrincipalRac, this.environmentHandle, out this.encryptor, out this.decryptor);
			this.initialized = true;
		}

		private SafeRightsManagementHandle encryptor;

		private SafeRightsManagementHandle decryptor;

		private bool disposed;

		private DisposableTenantLicensePair tenantLicensePair;

		private SafeRightsManagementEnvironmentHandle environmentHandle;

		private string publishLicense;

		private string useLicense;

		private bool initialized;

		private DisposeTracker disposeTracker;
	}
}
