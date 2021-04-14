using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.Protectors
{
	internal sealed class IrmPolicyInfoRms : I_IrmPolicyInfoRMS, IDisposeTrackable, IDisposable
	{
		public IrmPolicyInfoRms(BindLicenseForDecrypt bindLicenseDelegate)
		{
			if (bindLicenseDelegate == null)
			{
				throw new ArgumentNullException("bindLicenseDelegate");
			}
			this.disposeTracker = this.GetDisposeTracker();
			this.encryptorHandle = SafeRightsManagementHandle.InvalidHandle;
			this.decryptorHandle = null;
			this.issuanceLicense = null;
			this.ownDecryptorHandle = true;
			this.bindLicenseDelegate = bindLicenseDelegate;
		}

		public IrmPolicyInfoRms(SafeRightsManagementHandle decryptorHandle, string issuanceLicense)
		{
			if (decryptorHandle == null)
			{
				throw new ArgumentNullException("decryptorHandle");
			}
			if (decryptorHandle.IsInvalid)
			{
				throw new ArgumentException("handle is invalid", "decryptorHandle");
			}
			if (string.IsNullOrEmpty(issuanceLicense))
			{
				throw new ArgumentNullException("issuanceLicense");
			}
			this.disposeTracker = this.GetDisposeTracker();
			this.encryptorHandle = SafeRightsManagementHandle.InvalidHandle;
			this.decryptorHandle = decryptorHandle;
			this.ownDecryptorHandle = false;
			this.issuanceLicense = issuanceLicense;
			this.bindLicenseDelegate = null;
		}

		public IrmPolicyInfoRms(SafeRightsManagementHandle encryptorHandle, SafeRightsManagementHandle decryptorHandle, string issuanceLicense)
		{
			if (encryptorHandle == null)
			{
				throw new ArgumentNullException("encryptorHandle");
			}
			if (decryptorHandle == null)
			{
				throw new ArgumentNullException("decryptorHandle");
			}
			if (string.IsNullOrEmpty(issuanceLicense))
			{
				throw new ArgumentNullException("issuanceLicense");
			}
			if (encryptorHandle.IsInvalid)
			{
				throw new ArgumentException("handle is invalid", "encryptorHandle");
			}
			if (decryptorHandle.IsInvalid)
			{
				throw new ArgumentException("handle is invalid", "decryptorHandle");
			}
			this.disposeTracker = this.GetDisposeTracker();
			this.encryptorHandle = encryptorHandle;
			this.decryptorHandle = decryptorHandle;
			this.issuanceLicense = issuanceLicense;
			this.ownDecryptorHandle = false;
			this.bindLicenseDelegate = null;
		}

		public int HrGetICrypt(out object piic)
		{
			if (this.irmCrypt == null)
			{
				int errorCode = NativeMethods.HrCreateIrmCrypt(this.encryptorHandle, this.decryptorHandle, out this.irmCrypt);
				Marshal.ThrowExceptionForHR(errorCode);
			}
			piic = this.irmCrypt;
			return 0;
		}

		public int HrGetSignedIL(out string pbstrIL)
		{
			pbstrIL = this.issuanceLicense;
			return 0;
		}

		public int HrGetServerId(out string pbstrServerId)
		{
			pbstrServerId = string.Empty;
			return 0;
		}

		public int HrGetEULs(IntPtr rgbstrEUL, IntPtr rgbstrId, out uint pcbEULs)
		{
			pcbEULs = 0U;
			return 0;
		}

		public int HrSetSignedIL(string protectedDocumentIssuanceLicense)
		{
			if (string.IsNullOrEmpty(protectedDocumentIssuanceLicense))
			{
				return -2147024809;
			}
			if (this.bindLicenseDelegate != null)
			{
				try
				{
					this.decryptorHandle = this.bindLicenseDelegate(protectedDocumentIssuanceLicense);
				}
				catch (RightsManagementException ex)
				{
					return (int)ex.FailureCode;
				}
				return 0;
			}
			if (string.IsNullOrEmpty(this.issuanceLicense))
			{
				return -2147418113;
			}
			string a;
			string a2;
			try
			{
				DrmClientUtils.GetContentIdFromLicense(this.issuanceLicense, out a, out a2);
			}
			catch (RightsManagementException ex2)
			{
				return (int)ex2.FailureCode;
			}
			string b;
			string b2;
			try
			{
				DrmClientUtils.GetContentIdFromLicense(protectedDocumentIssuanceLicense, out b, out b2);
			}
			catch (RightsManagementException ex3)
			{
				return (int)ex3.FailureCode;
			}
			if (!string.Equals(a, b, StringComparison.OrdinalIgnoreCase) || !string.Equals(a2, b2, StringComparison.OrdinalIgnoreCase))
			{
				return -2147467259;
			}
			return 0;
		}

		public int HrSetServerEUL(string bstrEUL)
		{
			return 0;
		}

		public int HrGetRightsTemplate(out string pbstrRightsTemplate)
		{
			pbstrRightsTemplate = null;
			return -2147467263;
		}

		public int HrGetListGuid(out string pbstrListGuid)
		{
			pbstrListGuid = "00000000-0000-0000-0000-000000000000";
			return 0;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IrmPolicyInfoRms>(this);
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

		private void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.irmCrypt != null)
				{
					Marshal.ReleaseComObject(this.irmCrypt);
					this.irmCrypt = null;
				}
				if (this.ownDecryptorHandle && this.decryptorHandle != null)
				{
					this.decryptorHandle.Close();
					this.decryptorHandle = null;
				}
				this.disposed = true;
			}
		}

		private readonly SafeRightsManagementHandle encryptorHandle;

		private readonly string issuanceLicense;

		private SafeRightsManagementHandle decryptorHandle;

		private bool ownDecryptorHandle;

		private DisposeTracker disposeTracker;

		private bool disposed;

		private object irmCrypt;

		private BindLicenseForDecrypt bindLicenseDelegate;
	}
}
