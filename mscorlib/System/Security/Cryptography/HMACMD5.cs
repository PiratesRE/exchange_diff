using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class HMACMD5 : HMAC
	{
		public HMACMD5() : this(Utils.GenerateRandom(64))
		{
		}

		public HMACMD5(byte[] key)
		{
			this.m_hashName = "MD5";
			this.m_hash1 = new MD5CryptoServiceProvider();
			this.m_hash2 = new MD5CryptoServiceProvider();
			this.HashSizeValue = 128;
			base.InitializeKey(key);
		}
	}
}
