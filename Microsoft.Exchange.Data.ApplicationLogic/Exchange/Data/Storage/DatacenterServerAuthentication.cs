using System;
using System.Security.Cryptography;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatacenterServerAuthentication
	{
		public DatacenterServerAuthentication()
		{
			this.randomNumberGenerator = RandomNumberGenerator.Create();
		}

		public byte[] CurrentSecretKey
		{
			get
			{
				return this.currentSecretKey;
			}
			set
			{
				this.currentSecretKey = value;
			}
		}

		internal byte[] CurrentIVKey
		{
			get
			{
				return this.currentIVKey;
			}
			set
			{
				this.currentIVKey = value;
			}
		}

		public byte[] PreviousSecretKey
		{
			get
			{
				return this.previousSecretKey;
			}
			set
			{
				this.previousSecretKey = value;
			}
		}

		internal byte[] PreviousIVKey
		{
			get
			{
				return this.previousIVKey;
			}
			set
			{
				this.previousIVKey = value;
			}
		}

		public bool TrySetCurrentAndPreviousSecretKeys(string currentKeyBase64, string previousKeyBase64)
		{
			return this.TrySetCurrentAndPreviousSecretKeys(currentKeyBase64, previousKeyBase64, null, null);
		}

		public bool TrySetCurrentAndPreviousSecretKeys(string currentKeyBase64, string previousKeyBase64, string currentIVBase64, string previousIVBase64)
		{
			if (string.IsNullOrEmpty(currentKeyBase64))
			{
				return false;
			}
			if (previousKeyBase64 != null && previousKeyBase64.Length == 0)
			{
				return false;
			}
			bool result;
			try
			{
				this.currentSecretKey = Convert.FromBase64String(currentKeyBase64);
				if (!string.IsNullOrEmpty(currentIVBase64))
				{
					this.currentIVKey = Convert.FromBase64String(currentIVBase64);
				}
				if (string.IsNullOrEmpty(previousKeyBase64))
				{
					this.previousSecretKey = null;
					this.previousIVKey = null;
				}
				else
				{
					this.previousSecretKey = Convert.FromBase64String(previousKeyBase64);
					if (!string.IsNullOrEmpty(previousIVBase64))
					{
						this.previousIVKey = Convert.FromBase64String(previousIVBase64);
					}
				}
				result = true;
			}
			catch (FormatException)
			{
				result = false;
			}
			return result;
		}

		public string GetAuthenticationString()
		{
			if (this.currentSecretKey == null)
			{
				throw new InvalidOperationException("Current secret key was not set");
			}
			byte[] randomBytes = this.GetRandomBytes(128);
			long utcTicks = ExDateTime.Now.UtcTicks;
			return this.GenerateCustomAuthenticationString(randomBytes, utcTicks);
		}

		public string GenerateCustomAuthenticationString(byte[] nonce, long timestamp)
		{
			if (nonce == null)
			{
				throw new ArgumentNullException("nonce");
			}
			if (this.currentSecretKey == null)
			{
				throw new InvalidOperationException("Current secret key was not set");
			}
			byte[] hashedValue = this.GetHashedValue(this.currentSecretKey, nonce, 0, nonce.Length, timestamp);
			byte[] array = new byte[1 + nonce.Length + hashedValue.Length + 8];
			array[0] = 0;
			Array.Copy(nonce, 0, array, 1, nonce.Length);
			Array.Copy(hashedValue, 0, array, 1 + nonce.Length, hashedValue.Length);
			DatacenterServerAuthentication.SetBlobTailToLong(array, timestamp);
			return Convert.ToBase64String(array);
		}

		public bool ValidateAuthenticationString(string authenticationString)
		{
			if (this.currentSecretKey == null)
			{
				throw new InvalidOperationException("Current secret key was not set");
			}
			if (string.IsNullOrEmpty(authenticationString))
			{
				return false;
			}
			byte[] array = null;
			try
			{
				array = Convert.FromBase64String(authenticationString);
			}
			catch (FormatException)
			{
				return false;
			}
			if (array.Length < 9)
			{
				return false;
			}
			if (array[0] != 0)
			{
				return false;
			}
			long longFromBlobTail = DatacenterServerAuthentication.GetLongFromBlobTail(array);
			if (ExDateTime.Now.UtcTicks - (long)((ulong)-1294967296) > longFromBlobTail)
			{
				return false;
			}
			bool result = false;
			byte[] hashedValue = this.GetHashedValue(this.currentSecretKey, array, 1, 16, longFromBlobTail);
			if (array.Length != 17 + hashedValue.Length + 8)
			{
				return false;
			}
			if (DatacenterServerAuthentication.HashesMatch(hashedValue, array, 17))
			{
				result = true;
			}
			else if (this.previousSecretKey != null)
			{
				hashedValue = this.GetHashedValue(this.previousSecretKey, array, 1, 16, longFromBlobTail);
				if (array.Length == 17 + hashedValue.Length + 8)
				{
					result = DatacenterServerAuthentication.HashesMatch(hashedValue, array, 17);
				}
			}
			return result;
		}

		private static void SetBlobTailToLong(byte[] blob, long value)
		{
			int num = blob.Length;
			blob[num - 8] = (byte)(value & 255L);
			blob[num - 7] = (byte)(value >> 8 & 255L);
			blob[num - 6] = (byte)(value >> 16 & 255L);
			blob[num - 5] = (byte)(value >> 24 & 255L);
			blob[num - 4] = (byte)(value >> 32 & 255L);
			blob[num - 3] = (byte)(value >> 40 & 255L);
			blob[num - 2] = (byte)(value >> 48 & 255L);
			blob[num - 1] = (byte)(value >> 56 & 255L);
		}

		private static long GetLongFromBlobTail(byte[] blob)
		{
			int num = blob.Length;
			long num2 = 0L;
			num2 |= (long)((ulong)blob[num - 8]);
			num2 |= (long)((long)((ulong)blob[num - 7]) << 8);
			num2 |= (long)((long)((ulong)blob[num - 6]) << 16);
			num2 |= (long)((long)((ulong)blob[num - 5]) << 24);
			num2 |= (long)((long)((ulong)blob[num - 4]) << 32);
			num2 |= (long)((long)((ulong)blob[num - 3]) << 40);
			num2 |= (long)((long)((ulong)blob[num - 2]) << 48);
			return num2 | (long)((long)((ulong)blob[num - 1]) << 56);
		}

		private static bool HashesMatch(byte[] internalHash, byte[] arrayContainingExternalHash, int externalHashOffset)
		{
			for (int i = 0; i < internalHash.Length; i++)
			{
				if (internalHash[i] != arrayContainingExternalHash[externalHashOffset + i])
				{
					return false;
				}
			}
			return true;
		}

		private byte[] GetRandomBytes(int bits)
		{
			byte[] array = new byte[bits / 8];
			this.randomNumberGenerator.GetBytes(array);
			return array;
		}

		private byte[] GetHashedValue(byte[] secretKey, byte[] arrayContainingNonce, int nonceStartOffset, int nonceByteLength, long timestamp)
		{
			byte[] array = new byte[secretKey.Length + nonceByteLength + 8];
			Array.Copy(secretKey, array, secretKey.Length);
			Array.Copy(arrayContainingNonce, nonceStartOffset, array, secretKey.Length, nonceByteLength);
			DatacenterServerAuthentication.SetBlobTailToLong(array, timestamp);
			byte[] result;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				result = sha256Cng.ComputeHash(array);
			}
			return result;
		}

		public const int NonceLength = 128;

		public const long MaxAuthStringLifetimeTicks = 3000000000L;

		private const int AuthenticationBlobVersion = 0;

		private readonly RandomNumberGenerator randomNumberGenerator;

		private byte[] currentSecretKey;

		private byte[] currentIVKey;

		private byte[] previousIVKey;

		private byte[] previousSecretKey;
	}
}
