using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Security.Cryptography
{
	internal static class HashUtilities
	{
		public static SafeCryptProvHandle StaticAESHandle
		{
			get
			{
				if (HashUtilities.staticAESProvider == null)
				{
					lock (HashUtilities.InternalSyncObject)
					{
						if (HashUtilities.staticAESProvider == null)
						{
							SafeCryptProvHandle safeCryptProvHandle;
							if (!CapiNativeMethods.CryptAcquireContext(out safeCryptProvHandle, null, null, CapiNativeMethods.ProviderType.AES, (CapiNativeMethods.AcquireContext)4026531840U))
							{
								throw new CryptographicException(Marshal.GetLastWin32Error());
							}
							HashUtilities.staticAESProvider = safeCryptProvHandle;
						}
					}
				}
				return HashUtilities.staticAESProvider;
			}
		}

		private static object InternalSyncObject
		{
			get
			{
				if (HashUtilities.internalSyncObject == null)
				{
					object value = new object();
					Interlocked.CompareExchange(ref HashUtilities.internalSyncObject, value, null);
				}
				return HashUtilities.internalSyncObject;
			}
		}

		private static object internalSyncObject;

		private static SafeCryptProvHandle staticAESProvider;
	}
}
