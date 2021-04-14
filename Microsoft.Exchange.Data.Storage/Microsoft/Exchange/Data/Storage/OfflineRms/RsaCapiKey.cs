using System;
using System.Security.Cryptography;
using Microsoft.Exchange.Diagnostics;
using Microsoft.RightsManagementServices.Core;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RsaCapiKey : IDisposable
	{
		public RsaCapiKey(CspParameters parameters)
		{
			this.m_provider = new RSACryptoServiceProvider(parameters);
		}

		public byte[] Decrypt(byte[] cipherText, bool usePadding)
		{
			int num = this.m_provider.KeySize / 8;
			int maxDataBlockSize = this.getMaxDataBlockSize(num, usePadding);
			int num2 = cipherText.Length / num;
			byte[] array = null;
			byte[] array2 = new byte[num];
			for (int i = num2 - 1; i >= 0; i--)
			{
				Array.Copy(cipherText, i * num, array2, 0, array2.Length);
				byte[] array3 = this.m_provider.Decrypt(array2, usePadding);
				if (array == null)
				{
					array = new byte[num2 * maxDataBlockSize - (maxDataBlockSize - array3.Length)];
				}
				Array.Copy(array3, 0, array, i * maxDataBlockSize, array3.Length);
			}
			return array;
		}

		public void Dispose()
		{
			if (this.m_provider != null)
			{
				this.m_provider.Clear();
				this.m_provider = null;
			}
			GC.SuppressFinalize(this);
		}

		public void Init(byte[] keyBlob)
		{
			RSAParameters parameters = default(RSAParameters);
			RsaKeyBlob rsaKeyBlob = new RsaKeyBlob(keyBlob);
			try
			{
				parameters = RsaCapiKey.CreateRsaParameters(rsaKeyBlob);
				this.m_provider.ImportParameters(parameters);
			}
			finally
			{
				RsaCapiKey.ClearRsaParameters(parameters);
				rsaKeyBlob.Dispose();
			}
		}

		public bool PersistKeyInCryptoServiceProvider
		{
			get
			{
				return this.m_provider.PersistKeyInCsp;
			}
			set
			{
				this.m_provider.PersistKeyInCsp = value;
			}
		}

		public byte[] SignDigestValue(byte[] digest, HashAlgorithmType hashAlgorithm)
		{
			if (this.m_provider == null)
			{
				throw new InvalidOperationException("NullArgumentPassed");
			}
			if (hashAlgorithm == null)
			{
				return this.m_provider.SignHash(digest, CryptoConfig.MapNameToOID("SHA1"));
			}
			return this.m_provider.SignHash(digest, CryptoConfig.MapNameToOID("SHA256"));
		}

		private static void ClearRsaParameters(RSAParameters parameters)
		{
			ByteArrayUtilities.Clear(parameters.Modulus);
			ByteArrayUtilities.Clear(parameters.Exponent);
			ByteArrayUtilities.Clear(parameters.D);
			ByteArrayUtilities.Clear(parameters.DP);
			ByteArrayUtilities.Clear(parameters.DQ);
			ByteArrayUtilities.Clear(parameters.InverseQ);
			ByteArrayUtilities.Clear(parameters.P);
			ByteArrayUtilities.Clear(parameters.Q);
		}

		private static RSAParameters CreateRsaParameters(RsaKeyBlob keyBlob)
		{
			RSAParameters result = default(RSAParameters);
			result.Modulus = ByteArrayUtilities.CreateReversedArray(keyBlob.Modulus);
			result.Exponent = ByteArrayUtilities.CreateReversedArray(keyBlob.Exponent);
			if (keyBlob.IsPrivateKey)
			{
				result.D = ByteArrayUtilities.CreateReversedArray(keyBlob.D);
				result.DP = ByteArrayUtilities.CreateReversedArray(keyBlob.DP);
				result.DQ = ByteArrayUtilities.CreateReversedArray(keyBlob.DQ);
				result.InverseQ = ByteArrayUtilities.CreateReversedArray(keyBlob.InverseQ);
				result.P = ByteArrayUtilities.CreateReversedArray(keyBlob.P);
				result.Q = ByteArrayUtilities.CreateReversedArray(keyBlob.Q);
			}
			return result;
		}

		private int getMaxDataBlockSize(int blockSize, bool fOaep)
		{
			if (blockSize < 11)
			{
				throw new CoreArgumentException("blockSize");
			}
			int result;
			if (!fOaep)
			{
				result = blockSize - 11;
			}
			else
			{
				result = (blockSize - 2) / 3;
			}
			return result;
		}

		private RSACryptoServiceProvider m_provider;
	}
}
