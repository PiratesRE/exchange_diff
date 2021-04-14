using System;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.Exchange.Security.Compliance
{
	public abstract class HMACForNonCryptographicPurposes : HMAC
	{
		public HMACForNonCryptographicPurposes(byte[] key, HashAlgorithm hashAlgorithm)
		{
			this.hashAlgorithm = hashAlgorithm;
			byte[] array;
			if (key.Length > 64)
			{
				array = new byte[64];
				using (MessageDigestForNonCryptographicPurposes messageDigestForNonCryptographicPurposes = new MessageDigestForNonCryptographicPurposes())
				{
					byte[] array2 = messageDigestForNonCryptographicPurposes.ComputeHash(key);
					Buffer.BlockCopy(array2, 0, array, 0, array2.Length);
					goto IL_61;
				}
			}
			if (key.Length == 64)
			{
				array = key;
			}
			else
			{
				array = new byte[64];
				Buffer.BlockCopy(key, 0, array, 0, key.Length);
			}
			IL_61:
			this.keyInnerPad = new byte[64];
			this.keyOuterPad = new byte[64];
			for (int i = 0; i < 64; i++)
			{
				this.keyInnerPad[i] = (array[i] ^ 54);
				this.keyOuterPad[i] = (array[i] ^ 92);
			}
		}

		public override byte[] Hash
		{
			get
			{
				if (this.HashValue == null)
				{
					throw new CryptographicUnexpectedOperationException("Hash must be finalized before the hash value is retrieved.");
				}
				return this.HashValue;
			}
		}

		public new byte[] ComputeHash(byte[] message)
		{
			byte[] right = this.hashAlgorithm.ComputeHash(HMACForNonCryptographicPurposes.Concatenate(this.keyInnerPad, message));
			this.HashValue = this.hashAlgorithm.ComputeHash(HMACForNonCryptographicPurposes.Concatenate(this.keyOuterPad, right));
			return this.HashValue;
		}

		public new byte[] ComputeHash(Stream stream)
		{
			long length = stream.Length;
			byte[] array = new byte[length];
			stream.Read(array, 0, (int)length);
			return this.ComputeHash(array);
		}

		private static byte[] Concatenate(byte[] left, byte[] right)
		{
			byte[] array = new byte[left.Length + right.Length];
			Buffer.BlockCopy(left, 0, array, 0, left.Length);
			Buffer.BlockCopy(right, 0, array, left.Length, right.Length);
			return array;
		}

		private readonly HashAlgorithm hashAlgorithm;

		private readonly byte[] keyInnerPad;

		private readonly byte[] keyOuterPad;
	}
}
