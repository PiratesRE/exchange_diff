using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class Rfc2898DeriveBytes : DeriveBytes
	{
		public Rfc2898DeriveBytes(string password, int saltSize) : this(password, saltSize, 1000)
		{
		}

		public Rfc2898DeriveBytes(string password, int saltSize, int iterations) : this(password, saltSize, iterations, HashAlgorithmName.SHA1)
		{
		}

		[SecuritySafeCritical]
		public Rfc2898DeriveBytes(string password, int saltSize, int iterations, HashAlgorithmName hashAlgorithm)
		{
			this.m_cspParams = new CspParameters();
			base..ctor();
			if (saltSize < 0)
			{
				throw new ArgumentOutOfRangeException("saltSize", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(Environment.GetResourceString("Cryptography_HashAlgorithmNameNullOrEmpty"), "hashAlgorithm");
			}
			HMAC hmac = HMAC.Create("HMAC" + hashAlgorithm.Name);
			if (hmac == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_UnknownHashAlgorithm", new object[]
				{
					hashAlgorithm.Name
				}));
			}
			byte[] array = new byte[saltSize];
			Utils.StaticRandomNumberGenerator.GetBytes(array);
			this.Salt = array;
			this.IterationCount = iterations;
			this.m_password = new UTF8Encoding(false).GetBytes(password);
			hmac.Key = this.m_password;
			this.m_hmac = hmac;
			this.m_blockSize = hmac.HashSize >> 3;
			this.Initialize();
		}

		public Rfc2898DeriveBytes(string password, byte[] salt) : this(password, salt, 1000)
		{
		}

		public Rfc2898DeriveBytes(string password, byte[] salt, int iterations) : this(password, salt, iterations, HashAlgorithmName.SHA1)
		{
		}

		public Rfc2898DeriveBytes(string password, byte[] salt, int iterations, HashAlgorithmName hashAlgorithm) : this(new UTF8Encoding(false).GetBytes(password), salt, iterations, hashAlgorithm)
		{
		}

		public Rfc2898DeriveBytes(byte[] password, byte[] salt, int iterations) : this(password, salt, iterations, HashAlgorithmName.SHA1)
		{
		}

		[SecuritySafeCritical]
		public Rfc2898DeriveBytes(byte[] password, byte[] salt, int iterations, HashAlgorithmName hashAlgorithm)
		{
			this.m_cspParams = new CspParameters();
			base..ctor();
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw new ArgumentException(Environment.GetResourceString("Cryptography_HashAlgorithmNameNullOrEmpty"), "hashAlgorithm");
			}
			HMAC hmac = HMAC.Create("HMAC" + hashAlgorithm.Name);
			if (hmac == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_UnknownHashAlgorithm", new object[]
				{
					hashAlgorithm.Name
				}));
			}
			this.Salt = salt;
			this.IterationCount = iterations;
			this.m_password = password;
			hmac.Key = password;
			this.m_hmac = hmac;
			this.m_blockSize = hmac.HashSize >> 3;
			this.Initialize();
		}

		public int IterationCount
		{
			get
			{
				return (int)this.m_iterations;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
				}
				this.m_iterations = (uint)value;
				this.Initialize();
			}
		}

		public byte[] Salt
		{
			get
			{
				return (byte[])this.m_salt.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length < 8)
				{
					throw new ArgumentException(Environment.GetResourceString("Cryptography_PasswordDerivedBytes_FewBytesSalt"));
				}
				this.m_salt = (byte[])value.Clone();
				this.Initialize();
			}
		}

		public override byte[] GetBytes(int cb)
		{
			if (cb <= 0)
			{
				throw new ArgumentOutOfRangeException("cb", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
			}
			byte[] array = new byte[cb];
			int i = 0;
			int num = this.m_endIndex - this.m_startIndex;
			if (num > 0)
			{
				if (cb < num)
				{
					Buffer.InternalBlockCopy(this.m_buffer, this.m_startIndex, array, 0, cb);
					this.m_startIndex += cb;
					return array;
				}
				Buffer.InternalBlockCopy(this.m_buffer, this.m_startIndex, array, 0, num);
				this.m_startIndex = (this.m_endIndex = 0);
				i += num;
			}
			while (i < cb)
			{
				byte[] src = this.Func();
				int num2 = cb - i;
				if (num2 <= this.m_blockSize)
				{
					Buffer.InternalBlockCopy(src, 0, array, i, num2);
					i += num2;
					Buffer.InternalBlockCopy(src, num2, this.m_buffer, this.m_startIndex, this.m_blockSize - num2);
					this.m_endIndex += this.m_blockSize - num2;
					return array;
				}
				Buffer.InternalBlockCopy(src, 0, array, i, this.m_blockSize);
				i += this.m_blockSize;
			}
			return array;
		}

		public override void Reset()
		{
			this.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (this.m_hmac != null)
				{
					((IDisposable)this.m_hmac).Dispose();
				}
				if (this.m_buffer != null)
				{
					Array.Clear(this.m_buffer, 0, this.m_buffer.Length);
				}
				if (this.m_salt != null)
				{
					Array.Clear(this.m_salt, 0, this.m_salt.Length);
				}
			}
		}

		private void Initialize()
		{
			if (this.m_buffer != null)
			{
				Array.Clear(this.m_buffer, 0, this.m_buffer.Length);
			}
			this.m_buffer = new byte[this.m_blockSize];
			this.m_block = 1U;
			this.m_startIndex = (this.m_endIndex = 0);
		}

		private byte[] Func()
		{
			byte[] array = Utils.Int(this.m_block);
			this.m_hmac.TransformBlock(this.m_salt, 0, this.m_salt.Length, null, 0);
			this.m_hmac.TransformBlock(array, 0, array.Length, null, 0);
			this.m_hmac.TransformFinalBlock(EmptyArray<byte>.Value, 0, 0);
			byte[] hashValue = this.m_hmac.HashValue;
			this.m_hmac.Initialize();
			byte[] array2 = hashValue;
			int num = 2;
			while ((long)num <= (long)((ulong)this.m_iterations))
			{
				this.m_hmac.TransformBlock(hashValue, 0, hashValue.Length, null, 0);
				this.m_hmac.TransformFinalBlock(EmptyArray<byte>.Value, 0, 0);
				hashValue = this.m_hmac.HashValue;
				for (int i = 0; i < this.m_blockSize; i++)
				{
					byte[] array3 = array2;
					int num2 = i;
					array3[num2] ^= hashValue[i];
				}
				this.m_hmac.Initialize();
				num++;
			}
			this.m_block += 1U;
			return array2;
		}

		[SecuritySafeCritical]
		public byte[] CryptDeriveKey(string algname, string alghashname, int keySize, byte[] rgbIV)
		{
			if (keySize < 0)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidKeySize"));
			}
			int num = X509Utils.NameOrOidToAlgId(alghashname, OidGroup.HashAlgorithm);
			if (num == 0)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_PasswordDerivedBytes_InvalidAlgorithm"));
			}
			int num2 = X509Utils.NameOrOidToAlgId(algname, OidGroup.AllGroups);
			if (num2 == 0)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_PasswordDerivedBytes_InvalidAlgorithm"));
			}
			if (rgbIV == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Cryptography_PasswordDerivedBytes_InvalidIV"));
			}
			byte[] result = null;
			Rfc2898DeriveBytes.DeriveKey(this.ProvHandle, num2, num, this.m_password, this.m_password.Length, keySize << 16, rgbIV, rgbIV.Length, JitHelpers.GetObjectHandleOnStack<byte[]>(ref result));
			return result;
		}

		private SafeProvHandle ProvHandle
		{
			[SecurityCritical]
			get
			{
				if (this._safeProvHandle == null)
				{
					lock (this)
					{
						if (this._safeProvHandle == null)
						{
							SafeProvHandle safeProvHandle = Utils.AcquireProvHandle(this.m_cspParams);
							Thread.MemoryBarrier();
							this._safeProvHandle = safeProvHandle;
						}
					}
				}
				return this._safeProvHandle;
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void DeriveKey(SafeProvHandle hProv, int algid, int algidHash, byte[] password, int cbPassword, int dwFlags, byte[] IV, int cbIV, ObjectHandleOnStack retKey);

		private byte[] m_buffer;

		private byte[] m_salt;

		private HMAC m_hmac;

		private byte[] m_password;

		private CspParameters m_cspParams;

		private uint m_iterations;

		private uint m_block;

		private int m_startIndex;

		private int m_endIndex;

		private int m_blockSize;

		[SecurityCritical]
		private SafeProvHandle _safeProvHandle;
	}
}
