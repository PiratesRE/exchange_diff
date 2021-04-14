using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public struct TenantHint
	{
		public TenantHint(byte[] tenantHintBlob)
		{
			this.tenantHintBlob = tenantHintBlob;
		}

		public static TenantHint Empty
		{
			get
			{
				return new TenantHint(TenantHint.emptyBlob);
			}
		}

		public static TenantHint RootOrg
		{
			get
			{
				return new TenantHint(TenantHint.rootOrgBlob);
			}
		}

		public static byte[] RootOrgBlob
		{
			get
			{
				return TenantHint.rootOrgBlob;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.tenantHintBlob == null || this.tenantHintBlob.Length == 0;
			}
		}

		public bool IsRootOrg
		{
			get
			{
				return this.IsEmpty || ValueHelper.ArraysEqual<byte>(this.tenantHintBlob, TenantHint.rootOrgBlob);
			}
		}

		public byte[] TenantHintBlob
		{
			get
			{
				return this.tenantHintBlob;
			}
		}

		public int TenantHintBlobSize
		{
			get
			{
				if (this.tenantHintBlob != null)
				{
					return this.tenantHintBlob.Length;
				}
				return 0;
			}
		}

		public override string ToString()
		{
			if (this.IsEmpty)
			{
				return "<Empty>";
			}
			StringBuilder stringBuilder = new StringBuilder(this.TenantHintBlobSize * 2);
			foreach (byte b in this.TenantHintBlob)
			{
				stringBuilder.Append(b.ToString("X2"));
			}
			return stringBuilder.ToString();
		}

		internal const int MaxTenantHintBlobSize = 128;

		private static readonly byte[] rootOrgBlob = new byte[16];

		private static readonly byte[] emptyBlob = new byte[0];

		private byte[] tenantHintBlob;
	}
}
