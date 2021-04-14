using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class RevocationInformation
	{
		private RevocationInformation(RevocationInformation.CertRevocationInfo item)
		{
			this.status = item.Status;
			this.oid = item.RevocationOid;
			this.expires = item.Expires;
			this.crlInfo = item.CrlInfo;
		}

		public RevocationStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public Oid Oid
		{
			get
			{
				if (!string.IsNullOrEmpty(this.oid))
				{
					return new Oid(this.oid);
				}
				return null;
			}
		}

		public DateTime Expires
		{
			get
			{
				return this.expires;
			}
		}

		public RevocationCrlInformation CRLInformation
		{
			get
			{
				return this.crlInfo;
			}
		}

		internal static RevocationInformation Create(IntPtr bytes)
		{
			RevocationInformation.CertRevocationInfo item = (RevocationInformation.CertRevocationInfo)Marshal.PtrToStructure(bytes, typeof(RevocationInformation.CertRevocationInfo));
			return new RevocationInformation(item);
		}

		private RevocationStatus status;

		private string oid;

		private DateTime expires;

		private RevocationCrlInformation crlInfo;

		private struct CertRevocationInfo
		{
			public RevocationStatus Status
			{
				get
				{
					return this.status;
				}
			}

			public string RevocationOid
			{
				get
				{
					return this.revocationOid;
				}
			}

			public DateTime Expires
			{
				get
				{
					if (!this.hasFreshnessTime)
					{
						return DateTime.UtcNow;
					}
					return DateTime.UtcNow + TimeSpan.FromSeconds((double)this.freshnessTime);
				}
			}

			public RevocationCrlInformation CrlInfo
			{
				get
				{
					if (!(this.crlInfo != IntPtr.Zero))
					{
						return null;
					}
					return RevocationCrlInformation.Create(this.crlInfo);
				}
			}

			private uint size;

			private RevocationStatus status;

			[MarshalAs(UnmanagedType.LPStr)]
			private string revocationOid;

			private IntPtr oidSpecificInfo;

			[MarshalAs(UnmanagedType.Bool)]
			private bool hasFreshnessTime;

			private int freshnessTime;

			private IntPtr crlInfo;
		}
	}
}
