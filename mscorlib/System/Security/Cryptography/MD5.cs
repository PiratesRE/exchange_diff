using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class MD5 : HashAlgorithm
	{
		protected MD5()
		{
			this.HashSizeValue = 128;
		}

		public new static MD5 Create()
		{
			return MD5.Create("System.Security.Cryptography.MD5");
		}

		public new static MD5 Create(string algName)
		{
			return (MD5)CryptoConfig.CreateFromName(algName);
		}
	}
}
