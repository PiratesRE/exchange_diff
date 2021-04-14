using System;
using Microsoft.Exchange.Diagnostics.Components.TextProcessing;

namespace Microsoft.Exchange.TextProcessing
{
	internal class FingerprintGenerator
	{
		public Fingerprint GetFingerprint(string text, long id = 0L)
		{
			ulong[] minimumHashStartingArray = this.GetMinimumHashStartingArray();
			string[] array = ShingleGenerator.CollectShingles(text);
			using (BBitHash bbitHash = new BBitHash())
			{
				foreach (string shingle in array)
				{
					bbitHash.BBitHashShingle(shingle, minimumHashStartingArray);
				}
			}
			ExTraceGlobals.FingerprintTracer.TraceDebug<long, int, int>((long)this.GetHashCode(), "Created a fingerprint with id '{0}' and '{1}' shingles using algorithm version {2}", id, array.Length, FingerprintConstants.AlgorithmVersion);
			return new Fingerprint(id, this.GetTruncatedFingerprint(minimumHashStartingArray), Convert.ToUInt64(array.Length), FingerprintConstants.AlgorithmVersion);
		}

		public Fingerprint GetFingerprint(ushort[] fingerprintData, ulong shingleCount, int version, long id = 0L)
		{
			if (fingerprintData.Length != FingerprintConstants.Permutations)
			{
				throw new ArgumentException(Strings.InvalidFingerprintSize(fingerprintData.Length));
			}
			if (version != FingerprintConstants.AlgorithmVersion)
			{
				throw new ArgumentException(Strings.InvalidFingerprintVersion(version, FingerprintConstants.AlgorithmVersion));
			}
			return new Fingerprint(id, fingerprintData, shingleCount, version);
		}

		private ushort[] GetTruncatedFingerprint(ulong[] minimumHashes)
		{
			ushort[] array = new ushort[minimumHashes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (ushort)(minimumHashes[i] & FingerprintConstants.BBitMask);
			}
			return array;
		}

		private ulong[] GetMinimumHashStartingArray()
		{
			ulong[] array = new ulong[FingerprintConstants.Permutations];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ulong.MaxValue;
			}
			return array;
		}
	}
}
