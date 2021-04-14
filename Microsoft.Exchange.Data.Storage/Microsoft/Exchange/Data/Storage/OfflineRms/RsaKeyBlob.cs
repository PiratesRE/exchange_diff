using System;
using System.Security.Cryptography;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RsaKeyBlob : IDisposable
	{
		public RsaKeyBlob(byte[] keyBlob)
		{
			this.init(keyBlob);
		}

		public byte[] D
		{
			get
			{
				return this.m_D;
			}
		}

		public void Dispose()
		{
			this.m_bitLength = 0;
			RsaKeyBlob.clearArray(ref this.m_D);
			RsaKeyBlob.clearArray(ref this.m_DP);
			RsaKeyBlob.clearArray(ref this.m_DQ);
			RsaKeyBlob.clearArray(ref this.m_exponent);
			RsaKeyBlob.clearArray(ref this.m_inverseQ);
			this.m_isPrivateKey = false;
			RsaKeyBlob.clearArray(ref this.m_keyBlob);
			RsaKeyBlob.clearArray(ref this.m_modulus);
			RsaKeyBlob.clearArray(ref this.m_P);
			RsaKeyBlob.clearArray(ref this.m_Q);
			GC.SuppressFinalize(this);
		}

		public byte[] DP
		{
			get
			{
				return this.m_DP;
			}
		}

		public byte[] DQ
		{
			get
			{
				return this.m_DQ;
			}
		}

		public byte[] Exponent
		{
			get
			{
				return this.m_exponent;
			}
		}

		public byte[] InverseQ
		{
			get
			{
				return this.m_inverseQ;
			}
		}

		public bool IsPrivateKey
		{
			get
			{
				return this.m_isPrivateKey;
			}
		}

		public int KeySize
		{
			get
			{
				return this.m_bitLength;
			}
		}

		public byte[] Modulus
		{
			get
			{
				return this.m_modulus;
			}
		}

		public byte[] P
		{
			get
			{
				return this.m_P;
			}
		}

		public byte[] Q
		{
			get
			{
				return this.m_Q;
			}
		}

		private static void assignPrivateKeyValue(byte[] keyBlob, ref int offset, out byte[] val, int length)
		{
			val = new byte[length];
			Array.Copy(keyBlob, offset, val, 0, length);
			offset += length;
		}

		private static void checkKeyBlob(byte[] putativeKeyBlob)
		{
			if (!RsaKeyBlob.isValidKeyBlob(putativeKeyBlob))
			{
				throw new CryptographicException("RSA key blob data is corrupted");
			}
		}

		private static void clearArray(ref byte[] array)
		{
			if (array != null)
			{
				Array.Clear(array, 0, array.Length);
				array = null;
			}
		}

		private static int getPrivateKeyLength(int blobLength)
		{
			int num = (blobLength - 20) * 16 / 9;
			if (num < 0 || (blobLength - 20) % 9 != 0)
			{
				throw new CryptographicException("Key size is invalid")
				{
					Data = 
					{
						{
							"Context",
							"RsaKeyBlob.getPrivateKeyLength"
						},
						{
							"bitLength",
							num
						}
					}
				};
			}
			return num;
		}

		private static int getPublicKeyLength(int blobLength)
		{
			int num = (blobLength - 20) * 8;
			if (num < 0)
			{
				throw new CryptographicException("Key size is invalid")
				{
					Data = 
					{
						{
							"Context",
							"RsaKeyBlob.getPublicKeyLength"
						},
						{
							"bitLength",
							num
						}
					}
				};
			}
			return num;
		}

		private void init(byte[] keyBlob)
		{
			if (keyBlob == null)
			{
				throw new ArgumentException("InvalidKeyBlob", "keyBlob");
			}
			if (RsaKeyBlob.isValidKeyBlob(keyBlob))
			{
				this.m_keyBlob = (byte[])keyBlob.Clone();
			}
			else
			{
				if (keyBlob.Length <= 12)
				{
					throw new CryptographicException("Key blob is invalid");
				}
				this.m_keyBlob = new byte[keyBlob.Length - 12];
				Array.Copy(keyBlob, 12, this.m_keyBlob, 0, this.m_keyBlob.Length);
			}
			RsaKeyBlob.checkKeyBlob(this.m_keyBlob);
			if (keyBlob[0] == 7)
			{
				this.m_bitLength = RsaKeyBlob.getPrivateKeyLength(this.m_keyBlob.Length);
				this.m_isPrivateKey = true;
			}
			else
			{
				this.m_bitLength = RsaKeyBlob.getPublicKeyLength(this.m_keyBlob.Length);
				this.m_isPrivateKey = false;
			}
			this.m_modulus = new byte[this.m_bitLength / 8];
			Array.Copy(this.m_keyBlob, 20, this.m_modulus, 0, this.m_modulus.Length);
			this.m_exponent = new byte[4];
			Array.Copy(this.m_keyBlob, 16, this.m_exponent, 0, this.m_exponent.Length);
			if (this.m_isPrivateKey)
			{
				int num = 20 + this.m_modulus.Length;
				RsaKeyBlob.assignPrivateKeyValue(this.m_keyBlob, ref num, out this.m_P, this.m_modulus.Length / 2);
				RsaKeyBlob.assignPrivateKeyValue(this.m_keyBlob, ref num, out this.m_Q, this.m_modulus.Length / 2);
				RsaKeyBlob.assignPrivateKeyValue(this.m_keyBlob, ref num, out this.m_DP, this.m_modulus.Length / 2);
				RsaKeyBlob.assignPrivateKeyValue(this.m_keyBlob, ref num, out this.m_DQ, this.m_modulus.Length / 2);
				RsaKeyBlob.assignPrivateKeyValue(this.m_keyBlob, ref num, out this.m_inverseQ, this.m_modulus.Length / 2);
				RsaKeyBlob.assignPrivateKeyValue(this.m_keyBlob, ref num, out this.m_D, this.m_modulus.Length);
			}
		}

		private static bool isValidKeyBlob(byte[] putativeKeyBlob)
		{
			bool flag = RsaKeyBlob.isValuePresent(putativeKeyBlob, 0, 1, new int[]
			{
				6
			});
			int num = (int)BitConverter.ToUInt32(putativeKeyBlob, 12);
			return RsaKeyBlob.isValuePresent(putativeKeyBlob, 0, 1, new int[]
			{
				flag ? 6 : 7
			}) && RsaKeyBlob.isValuePresent(putativeKeyBlob, 4, 2, new int[]
			{
				41984,
				9216
			}) && RsaKeyBlob.isValuePresent(putativeKeyBlob, 8, 4, new int[]
			{
				flag ? 826364754 : 843141970
			}) && num == (flag ? RsaKeyBlob.getPublicKeyLength(putativeKeyBlob.Length) : RsaKeyBlob.getPrivateKeyLength(putativeKeyBlob.Length));
		}

		private static bool isValuePresent(byte[] array, int offset, int length, params int[] values)
		{
			int num = (int)ByteArrayUtilities.ToUInt32(array, offset, length);
			foreach (int num2 in values)
			{
				if (num2 == num)
				{
					return true;
				}
			}
			return false;
		}

		private const int EXPONENT_OFFSET = 16;

		private const int MODULUS_LENGTH_OFFSET = 12;

		private const int MODULUS_OFFSET = 20;

		private const int MIN_KEY_LENGTH = 384;

		private const int TYPE_OFFSET = 0;

		private const int TYPE_LENGTH = 1;

		private const byte PUBLIC_KEYBLOB_TYPE = 6;

		private const byte PRIVATE_KEYBLOB_TYPE = 7;

		private const int ALGORITHM_OFFSET = 4;

		private const int ALGORITHM_LENGTH = 2;

		private const int SIGNATURE_ALGORITHM = 9216;

		private const int KEY_EXCHANGE_ALGORITHM = 41984;

		private const int MAGIC_OFFSET = 8;

		private const int MAGIC_LENGTH = 4;

		private const int MAGIC_PUBLIC = 826364754;

		private const int MAGIC_PRIVATE = 843141970;

		private const int SN_HEADER_LENGTH = 12;

		private int m_bitLength;

		private byte[] m_D;

		private byte[] m_DP;

		private byte[] m_DQ;

		private byte[] m_exponent;

		private byte[] m_inverseQ;

		private bool m_isPrivateKey;

		private byte[] m_keyBlob;

		private byte[] m_modulus;

		private byte[] m_P;

		private byte[] m_Q;
	}
}
