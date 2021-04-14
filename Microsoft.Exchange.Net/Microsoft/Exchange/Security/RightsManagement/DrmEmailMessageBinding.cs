using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class DrmEmailMessageBinding : EncryptedEmailMessageBinding
	{
		public DrmEmailMessageBinding(string issuanceLicense, SafeRightsManagementHandle decryptorHandle)
		{
			if (string.IsNullOrEmpty(issuanceLicense))
			{
				throw new ArgumentNullException("issuanceLicense");
			}
			if (decryptorHandle == null)
			{
				throw new ArgumentNullException("decryptorHandle");
			}
			if (decryptorHandle.IsInvalid)
			{
				throw new ArgumentException("decryptorHandle is invalid");
			}
			this.encryptorHandle = SafeRightsManagementHandle.InvalidHandle;
			this.decryptorHandle = decryptorHandle;
			this.issuanceLicense = issuanceLicense;
		}

		public DrmEmailMessageBinding(string issuanceLicense, SafeRightsManagementHandle encryptorHandle, SafeRightsManagementHandle decryptorHandle)
		{
			if (string.IsNullOrEmpty(issuanceLicense))
			{
				throw new ArgumentNullException("issuanceLicense");
			}
			if (encryptorHandle == null)
			{
				throw new ArgumentNullException("encryptorHandle");
			}
			if (encryptorHandle.IsInvalid)
			{
				throw new ArgumentException("encryptorHandle is invalid");
			}
			if (decryptorHandle == null)
			{
				throw new ArgumentNullException("decryptorHandle");
			}
			if (decryptorHandle.IsInvalid)
			{
				throw new ArgumentException("decryptorHandle is invalid");
			}
			this.issuanceLicense = issuanceLicense;
			this.encryptorHandle = encryptorHandle;
			this.decryptorHandle = decryptorHandle;
		}

		public SafeRightsManagementHandle EncryptorHandle
		{
			get
			{
				return this.encryptorHandle;
			}
		}

		public SafeRightsManagementHandle DecryptorHandle
		{
			get
			{
				return this.decryptorHandle;
			}
		}

		public string IssuanceLicense
		{
			get
			{
				return this.issuanceLicense;
			}
		}

		public override IStorage ConvertToEncryptedStorage(IStream stream, bool create)
		{
			IStorage result = null;
			int errorCode = SafeNativeMethods.WrapEncryptedStorage(stream, this.encryptorHandle, this.decryptorHandle, create, out result);
			Marshal.ThrowExceptionForHR(errorCode);
			return result;
		}

		private readonly SafeRightsManagementHandle encryptorHandle;

		private readonly SafeRightsManagementHandle decryptorHandle;

		private readonly string issuanceLicense;
	}
}
