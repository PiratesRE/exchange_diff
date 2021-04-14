using System;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Compliance
{
	internal class HMACSHA256Cng : KeyedHashAlgorithm
	{
		public HMACSHA256Cng() : this(HMACSHA256Cng.GenerateRandomKey())
		{
		}

		public HMACSHA256Cng(byte[] key)
		{
			this.hashAlgorithmInner = new SHA256Cng();
			this.hashAlgorithmOuter = new SHA256Cng();
			this.HashSizeValue = 256;
			this.InitializeKey(key);
		}

		public override void Initialize()
		{
			this.hashAlgorithmInner.Initialize();
			this.hashAlgorithmOuter.Initialize();
			this.hashing = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.hashAlgorithmInner != null)
				{
					this.hashAlgorithmInner.Clear();
					this.hashAlgorithmInner = null;
				}
				if (this.hashAlgorithmOuter != null)
				{
					this.hashAlgorithmOuter.Clear();
					this.hashAlgorithmOuter = null;
				}
			}
			base.Dispose(disposing);
		}

		protected override void HashCore(byte[] rgb, int ib, int cb)
		{
			if (!this.hashing)
			{
				this.hashAlgorithmInner.TransformBlock(this.inner, 0, this.inner.Length, this.inner, 0);
				this.hashing = true;
			}
			this.hashAlgorithmInner.TransformBlock(rgb, ib, cb, rgb, ib);
		}

		protected override byte[] HashFinal()
		{
			if (!this.hashing)
			{
				this.hashAlgorithmInner.TransformBlock(this.inner, 0, this.inner.Length, this.inner, 0);
				this.hashing = true;
			}
			this.hashAlgorithmInner.TransformFinalBlock(new byte[0], 0, 0);
			byte[] hash = this.hashAlgorithmInner.Hash;
			this.hashAlgorithmOuter.TransformBlock(this.outer, 0, this.outer.Length, this.outer, 0);
			this.hashAlgorithmOuter.TransformBlock(hash, 0, hash.Length, hash, 0);
			this.hashing = false;
			this.hashAlgorithmOuter.TransformFinalBlock(new byte[0], 0, 0);
			return this.hashAlgorithmOuter.Hash;
		}

		private static byte[] GenerateRandomKey()
		{
			byte[] array = new byte[64];
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			randomNumberGenerator.GetBytes(array);
			return array;
		}

		private void InitializeKey(byte[] key)
		{
			this.inner = null;
			this.outer = null;
			if (key.Length > 64)
			{
				this.KeyValue = this.hashAlgorithmInner.ComputeHash(key);
			}
			else
			{
				this.KeyValue = (byte[])key.Clone();
			}
			if (this.inner == null)
			{
				this.inner = new byte[64];
			}
			if (this.outer == null)
			{
				this.outer = new byte[64];
			}
			for (int i = 0; i < 64; i++)
			{
				this.inner[i] = 54;
				this.outer[i] = 92;
			}
			for (int j = 0; j < this.KeyValue.Length; j++)
			{
				this.inner[j] = (this.inner[j] ^ this.KeyValue[j]);
				this.outer[j] = (this.outer[j] ^ this.KeyValue[j]);
			}
		}

		private const int BlockSizeValue = 64;

		private HashAlgorithm hashAlgorithmInner;

		private HashAlgorithm hashAlgorithmOuter;

		private bool hashing;

		private byte[] inner;

		private byte[] outer;
	}
}
