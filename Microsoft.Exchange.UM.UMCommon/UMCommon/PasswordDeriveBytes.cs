using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class PasswordDeriveBytes : DeriveBytes
	{
		internal PasswordDeriveBytes(string strPassword, byte[] rgbSalt) : this(strPassword, rgbSalt, "SHA1", 100)
		{
		}

		internal PasswordDeriveBytes(byte[] password, byte[] salt) : this(password, salt, "SHA1", 100)
		{
		}

		internal PasswordDeriveBytes(string strPassword, byte[] rgbSalt, string strHashName, int iterations) : this(new UTF8Encoding(false).GetBytes(strPassword), rgbSalt, strHashName, iterations)
		{
		}

		internal PasswordDeriveBytes(byte[] password, byte[] salt, string hashName, int iterations)
		{
			this.IterationCount = iterations;
			this.Salt = salt;
			this.HashName = hashName;
			this.password = password;
		}

		internal string HashName
		{
			get
			{
				return this.hashName;
			}
			set
			{
				if (this.baseValue != null)
				{
					throw new CryptographicException(Strings.PasswordDerivedBytesValuesFixed("HashName"));
				}
				this.hashName = value;
				this.hash = PasswordDeriveBytes.CreateFromName(this.hashName);
			}
		}

		internal int IterationCount
		{
			get
			{
				return this.iterations;
			}
			set
			{
				if (this.baseValue != null)
				{
					throw new CryptographicException(Strings.PasswordDerivedBytesValuesFixed("IterationCount"));
				}
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value", Strings.PasswordDerivedBytesNeedNonNegNum);
				}
				this.iterations = value;
			}
		}

		internal byte[] Salt
		{
			get
			{
				if (this.salt == null)
				{
					return null;
				}
				return (byte[])this.salt.Clone();
			}
			set
			{
				if (this.baseValue != null)
				{
					throw new CryptographicException(Strings.PasswordDerivedBytesValuesFixed("Salt"));
				}
				if (value == null)
				{
					this.salt = null;
					return;
				}
				this.salt = (byte[])value.Clone();
			}
		}

		public override byte[] GetBytes(int cb)
		{
			int num = 0;
			byte[] array = new byte[cb];
			if (this.baseValue == null)
			{
				this.ComputeBaseValue();
			}
			else if (this.extra != null)
			{
				num = this.extra.Length - this.extraCount;
				if (num >= cb)
				{
					Buffer.BlockCopy(this.extra, this.extraCount, array, 0, cb);
					if (num > cb)
					{
						this.extraCount += cb;
					}
					else
					{
						this.extra = null;
					}
					return array;
				}
				Buffer.BlockCopy(this.extra, num, array, 0, num);
				this.extra = null;
			}
			byte[] array2 = this.ComputeBytes(cb - num);
			Buffer.BlockCopy(array2, 0, array, num, cb - num);
			if (array2.Length + num > cb)
			{
				this.extra = array2;
				this.extraCount = cb - num;
			}
			return array;
		}

		public override void Reset()
		{
			this.prefix = 0;
			this.extra = null;
			this.baseValue = null;
		}

		private static HashAlgorithm CreateFromName(string hashName)
		{
			if (string.Equals(hashName, "SHA256CryptoServiceProvider", StringComparison.InvariantCultureIgnoreCase))
			{
				return new SHA256CryptoServiceProvider();
			}
			if (string.Equals(hashName, "SHA256", StringComparison.InvariantCultureIgnoreCase))
			{
				return new SHA256CryptoServiceProvider();
			}
			if (string.Equals(hashName, "SHA512", StringComparison.InvariantCultureIgnoreCase))
			{
				return new SHA512CryptoServiceProvider();
			}
			return (HashAlgorithm)CryptoConfig.CreateFromName(hashName);
		}

		private byte[] ComputeBaseValue()
		{
			this.hash.Initialize();
			this.hash.TransformBlock(this.password, 0, this.password.Length, this.password, 0);
			if (this.salt != null)
			{
				this.hash.TransformBlock(this.salt, 0, this.salt.Length, this.salt, 0);
			}
			this.hash.TransformFinalBlock(new byte[0], 0, 0);
			this.baseValue = this.hash.Hash;
			this.hash.Initialize();
			for (int i = 1; i < this.iterations - 1; i++)
			{
				this.hash.ComputeHash(this.baseValue);
				this.baseValue = this.hash.Hash;
			}
			return this.baseValue;
		}

		private byte[] ComputeBytes(int cb)
		{
			int num = 0;
			this.hash.Initialize();
			int num2 = this.hash.HashSize / 8;
			byte[] array = new byte[(cb + num2 - 1) / num2 * num2];
			CryptoStream cryptoStream = new CryptoStream(Stream.Null, this.hash, CryptoStreamMode.Write);
			this.HashPrefix(cryptoStream);
			cryptoStream.Write(this.baseValue, 0, this.baseValue.Length);
			cryptoStream.Close();
			Buffer.BlockCopy(this.hash.Hash, 0, array, num, num2);
			num += num2;
			while (cb > num)
			{
				this.hash.Initialize();
				cryptoStream = new CryptoStream(Stream.Null, this.hash, CryptoStreamMode.Write);
				this.HashPrefix(cryptoStream);
				cryptoStream.Write(this.baseValue, 0, this.baseValue.Length);
				cryptoStream.Close();
				Buffer.BlockCopy(this.hash.Hash, 0, array, num, num2);
				num += num2;
			}
			return array;
		}

		private void HashPrefix(CryptoStream cs)
		{
			int num = 0;
			byte[] array = new byte[]
			{
				48,
				48,
				48
			};
			if (this.prefix > 999)
			{
				throw new CryptographicException(Strings.PasswordDerivedBytesTooManyBytes);
			}
			if (this.prefix >= 100)
			{
				byte[] array2 = array;
				int num2 = 0;
				array2[num2] += (byte)(this.prefix / 100);
				num++;
			}
			if (this.prefix >= 10)
			{
				byte[] array3 = array;
				int num3 = num;
				array3[num3] += (byte)(this.prefix % 100 / 10);
				num++;
			}
			if (this.prefix > 0)
			{
				byte[] array4 = array;
				int num4 = num;
				array4[num4] += (byte)(this.prefix % 10);
				num++;
				cs.Write(array, 0, num);
			}
			this.prefix++;
		}

		private int extraCount;

		private int prefix;

		private int iterations;

		private byte[] baseValue;

		private byte[] extra;

		private byte[] salt;

		private string hashName;

		private byte[] password;

		private HashAlgorithm hash;
	}
}
