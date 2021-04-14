using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class ChainMatchIssuer
	{
		private ChainMatchIssuer()
		{
		}

		protected ChainMatchIssuer(ChainMatchIssuer.Operator type, Oid[] oids)
		{
			this.type = type;
			this.usages = oids;
		}

		public Oid[] Usages
		{
			get
			{
				return this.usages;
			}
			set
			{
				this.usages = value;
			}
		}

		internal CapiNativeMethods.CertUsageMatch GetCertUsageMatch(ref SafeHGlobalHandle bytes)
		{
			CapiNativeMethods.CryptoApiBlob usage = new CapiNativeMethods.CryptoApiBlob(0U, bytes);
			if (this.Usages != null && this.Usages.Length > 0)
			{
				bytes = this.GetBytes();
				usage = new CapiNativeMethods.CryptoApiBlob((uint)this.Usages.Length, bytes);
			}
			return new CapiNativeMethods.CertUsageMatch((CapiNativeMethods.CertUsageMatch.Operator)this.type, usage);
		}

		private SafeHGlobalHandle GetBytes()
		{
			if (this.usages == null || this.usages.Length == 0)
			{
				return SafeHGlobalHandle.InvalidHandle;
			}
			int num = Marshal.SizeOf(typeof(IntPtr));
			int num2 = num * this.usages.Length;
			foreach (Oid oid in this.usages)
			{
				num2 += Encoding.ASCII.GetByteCount(oid.Value) + 1;
			}
			SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal(num2);
			IntPtr intPtr = safeHGlobalHandle.DangerousGetHandle();
			IntPtr intPtr2 = (IntPtr)((long)intPtr + (long)(num * this.usages.Length));
			foreach (Oid oid2 in this.usages)
			{
				byte[] bytes = Encoding.ASCII.GetBytes(oid2.Value);
				Marshal.WriteIntPtr(intPtr, intPtr2);
				Marshal.Copy(bytes, 0, intPtr2, bytes.Length);
				Marshal.WriteByte(intPtr2, bytes.Length, 0);
				intPtr = (IntPtr)((long)intPtr + (long)num);
				intPtr2 = (IntPtr)((long)intPtr2 + (long)bytes.Length + 1L);
			}
			return safeHGlobalHandle;
		}

		private ChainMatchIssuer.Operator type;

		private Oid[] usages;

		protected enum Operator : uint
		{
			And,
			Or
		}
	}
}
