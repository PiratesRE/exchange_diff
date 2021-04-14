using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.TextProcessing
{
	internal class BBitHash : IDisposable
	{
		public void Dispose()
		{
			this.hashAlgorithm.Dispose();
		}

		public void BBitHashShingle(string shingle, ulong[] minimumHashes)
		{
			if (string.IsNullOrEmpty(shingle))
			{
				throw new ArgumentException(Strings.InvalidShingle(shingle));
			}
			ulong termValue;
			ulong termValue2;
			this.ComputeShingleHash(shingle, out termValue, out termValue2);
			int num = minimumHashes.Length / 2;
			for (int i = 0; i < num; i++)
			{
				minimumHashes[i] = Math.Min(this.GenerateTermHash(termValue, i), minimumHashes[i]);
				minimumHashes[i + num] = Math.Min(this.GenerateTermHash(termValue2, i), minimumHashes[i + num]);
			}
		}

		private void ComputeShingleHash(string input, out ulong upperBits, out ulong lowerBits)
		{
			byte[] value = this.hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(input));
			lowerBits = BitConverter.ToUInt64(value, 0);
			upperBits = BitConverter.ToUInt64(value, 8);
		}

		private ulong GenerateTermHash(ulong termValue, int hashSeed)
		{
			ulong num = HashSeeds.PrimeNumbers[hashSeed];
			ulong num2 = HashSeeds.LittlePrimeNumbers[hashSeed];
			num = num * (num2 * (termValue >> 32) >> 5 | num2 * (termValue >> 32) << 59) + (termValue >> 32);
			ulong num3 = termValue & (ulong)-1;
			return num * (num2 * num3 >> 5) | (num2 * num3 << 59) + num3;
		}

		private HashAlgorithm hashAlgorithm = MessageDigestForNonCryptographicPurposes.Create();
	}
}
