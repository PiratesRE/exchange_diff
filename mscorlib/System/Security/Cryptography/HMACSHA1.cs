using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACSHA1 : HMAC
	{
		public HMACSHA1() : this(Utils.GenerateRandom(64))
		{
		}

		public HMACSHA1(byte[] key) : this(key, false)
		{
		}

		public HMACSHA1(byte[] key, bool useManagedSha1)
		{
			this.m_hashName = "SHA1";
			if (useManagedSha1)
			{
				this.m_hash1 = new SHA1Managed();
				this.m_hash2 = new SHA1Managed();
			}
			else
			{
				this.m_hash1 = new SHA1CryptoServiceProvider();
				this.m_hash2 = new SHA1CryptoServiceProvider();
			}
			this.HashSizeValue = 160;
			base.InitializeKey(key);
		}
	}
}
