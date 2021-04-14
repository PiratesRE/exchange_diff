using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[ComVisible(false)]
	internal static class X500DistinguishedNameDecoder
	{
		public static IList<KeyValuePair<Oid, string>> Decode(X500DistinguishedName distinguishedName)
		{
			SafeHGlobalHandle safeHGlobalHandle = null;
			IList<KeyValuePair<Oid, string>> list = new List<KeyValuePair<Oid, string>>();
			uint num = 0U;
			IList<KeyValuePair<Oid, string>> result;
			try
			{
				CapiNativeMethods.DecodeObject(CapiNativeMethods.EncodeDecodeObjectType.X509Name, distinguishedName.RawData, out safeHGlobalHandle, out num);
				CapiNativeMethods.CryptoApiBlob cryptoApiBlob = (CapiNativeMethods.CryptoApiBlob)Marshal.PtrToStructure(safeHGlobalHandle.DangerousGetHandle(), typeof(CapiNativeMethods.CryptoApiBlob));
				IntPtr intPtr = cryptoApiBlob.DataPointer;
				int num2 = 0;
				while ((long)num2 < (long)((ulong)cryptoApiBlob.Count))
				{
					CapiNativeMethods.CryptoApiBlob cryptoApiBlob2 = (CapiNativeMethods.CryptoApiBlob)Marshal.PtrToStructure(intPtr, typeof(CapiNativeMethods.CryptoApiBlob));
					IntPtr intPtr2 = cryptoApiBlob2.DataPointer;
					int num3 = 0;
					while ((long)num3 < (long)((ulong)cryptoApiBlob2.Count))
					{
						CapiNativeMethods.CertRdnAttribute attribute = (CapiNativeMethods.CertRdnAttribute)Marshal.PtrToStructure(intPtr2, typeof(CapiNativeMethods.CertRdnAttribute));
						string value = CapiNativeMethods.RDNValueToString(attribute);
						list.Insert(0, new KeyValuePair<Oid, string>(new Oid(attribute.OID), value));
						intPtr2 = (IntPtr)((long)intPtr2 + (long)X500DistinguishedNameDecoder.cryptoApiBlobSize);
						num3++;
					}
					intPtr = (IntPtr)((long)intPtr + (long)X500DistinguishedNameDecoder.cryptoApiBlobSize);
					num2++;
				}
				result = list;
			}
			catch (CryptographicException)
			{
				result = null;
			}
			finally
			{
				safeHGlobalHandle.Dispose();
			}
			return result;
		}

		private static int cryptoApiBlobSize = Marshal.SizeOf(typeof(CapiNativeMethods.CryptoApiBlob));
	}
}
