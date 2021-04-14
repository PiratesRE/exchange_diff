using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	[ComVisible(false)]
	internal sealed class X509SubjectAltNameExtension : X509Extension
	{
		public X509SubjectAltNameExtension(IEnumerable<string> names, bool critical) : base(WellKnownOid.SubjectAltName, X509SubjectAltNameExtension.EncodeExtension(names), critical)
		{
		}

		private X509SubjectAltNameExtension()
		{
		}

		private X509SubjectAltNameExtension(Oid oid, byte[] rawData, bool critical) : base(oid, rawData, critical)
		{
		}

		public IList<string> DnsNames
		{
			get
			{
				if (this.names == null)
				{
					this.DecodeExtension();
				}
				return this.names;
			}
		}

		public static X509SubjectAltNameExtension Create(X509Extension source)
		{
			if (source == null)
			{
				return null;
			}
			return new X509SubjectAltNameExtension(source.Oid, source.RawData, source.Critical);
		}

		public override void CopyFrom(AsnEncodedData asnEncodedData)
		{
			base.CopyFrom(asnEncodedData);
		}

		private static byte[] EncodeExtension(IEnumerable<string> names)
		{
			if (names == null)
			{
				throw new ArgumentNullException("name");
			}
			int num = 0;
			foreach (string value in names)
			{
				if (!string.IsNullOrEmpty(value))
				{
					num++;
				}
			}
			if (num == 0)
			{
				throw new ArgumentException(NetException.CollectionEmpty, "name");
			}
			uint num2 = (uint)(num * CapiNativeMethods.CertAltNameDnsEntry.MarshalSize);
			SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal((int)num2);
			IntPtr intPtr = safeHGlobalHandle.DangerousGetHandle();
			foreach (string text in names)
			{
				if (!string.IsNullOrEmpty(text))
				{
					Marshal.StructureToPtr(new CapiNativeMethods.CertAltNameDnsEntry
					{
						Type = CapiNativeMethods.CertAltNameType.DnsName,
						Name = text
					}, intPtr, false);
					intPtr = new IntPtr((long)intPtr + (long)CapiNativeMethods.CertAltNameDnsEntry.MarshalSize);
				}
			}
			CapiNativeMethods.CryptoApiBlob cryptoApiBlob = new CapiNativeMethods.CryptoApiBlob((uint)num, safeHGlobalHandle);
			byte[] array = null;
			num2 = 0U;
			try
			{
				if (!CapiNativeMethods.CryptEncodeObject(CapiNativeMethods.EncodeType.X509Asn | CapiNativeMethods.EncodeType.Pkcs7Asn, WellKnownOid.SubjectAltName.Value, ref cryptoApiBlob, array, ref num2))
				{
					throw new CryptographicException(Marshal.GetLastWin32Error());
				}
				array = new byte[num2];
				if (!CapiNativeMethods.CryptEncodeObject(CapiNativeMethods.EncodeType.X509Asn | CapiNativeMethods.EncodeType.Pkcs7Asn, WellKnownOid.SubjectAltName.Value, ref cryptoApiBlob, array, ref num2))
				{
					throw new CryptographicException(Marshal.GetLastWin32Error());
				}
			}
			finally
			{
				intPtr = safeHGlobalHandle.DangerousGetHandle();
				for (int i = 0; i < num; i++)
				{
					Marshal.DestroyStructure(intPtr, typeof(CapiNativeMethods.CertAltNameDnsEntry));
					intPtr = new IntPtr((long)intPtr + (long)CapiNativeMethods.CertAltNameDnsEntry.MarshalSize);
				}
			}
			safeHGlobalHandle.Close();
			return array;
		}

		private void DecodeExtension()
		{
			uint num = 0U;
			SafeHGlobalHandle safeHGlobalHandle = null;
			CapiNativeMethods.DecodeObject(base.Oid.Value, base.RawData, out safeHGlobalHandle, out num);
			CapiNativeMethods.CryptoApiBlob cryptoApiBlob = (CapiNativeMethods.CryptoApiBlob)Marshal.PtrToStructure(safeHGlobalHandle.DangerousGetHandle(), typeof(CapiNativeMethods.CryptoApiBlob));
			List<string> list = new List<string>((int)cryptoApiBlob.Count);
			IntPtr intPtr = cryptoApiBlob.DataPointer;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)cryptoApiBlob.Count))
			{
				CapiNativeMethods.CertAltNameType certAltNameType = (CapiNativeMethods.CertAltNameType)Marshal.ReadInt32(intPtr);
				if (certAltNameType == CapiNativeMethods.CertAltNameType.DnsName)
				{
					CapiNativeMethods.CertAltNameDnsEntry certAltNameDnsEntry = (CapiNativeMethods.CertAltNameDnsEntry)Marshal.PtrToStructure(intPtr, typeof(CapiNativeMethods.CertAltNameDnsEntry));
					if (certAltNameDnsEntry.Type == CapiNativeMethods.CertAltNameType.DnsName)
					{
						list.Add(certAltNameDnsEntry.Name);
					}
				}
				intPtr = (IntPtr)((long)intPtr + (long)Marshal.SizeOf(typeof(CapiNativeMethods.CertAltNameDnsEntry)));
				num2++;
			}
			this.names = list.AsReadOnly();
			safeHGlobalHandle.Dispose();
		}

		private IList<string> names;
	}
}
