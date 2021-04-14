using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class AsymmetricSignatureFormatter
	{
		public abstract void SetKey(AsymmetricAlgorithm key);

		public abstract void SetHashAlgorithm(string strName);

		public virtual byte[] CreateSignature(HashAlgorithm hash)
		{
			if (hash == null)
			{
				throw new ArgumentNullException("hash");
			}
			this.SetHashAlgorithm(hash.ToString());
			return this.CreateSignature(hash.Hash);
		}

		public abstract byte[] CreateSignature(byte[] rgbHash);
	}
}
