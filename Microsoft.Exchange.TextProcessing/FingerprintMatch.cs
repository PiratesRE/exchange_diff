using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class FingerprintMatch : IMatch
	{
		internal FingerprintMatch(LshFingerprint fingerprint, double coefficient, bool detectContainment)
		{
			this.fingerprint = fingerprint;
			this.coefficient = coefficient;
			this.detectContainment = detectContainment;
		}

		public bool IsMatch(TextScanContext data)
		{
			LshFingerprint lshFingerprint = data.Fingerprint;
			int num;
			int num2;
			if (this.detectContainment)
			{
				LshFingerprint.ComputeContainment(this.fingerprint, lshFingerprint, out num, out num2);
			}
			else
			{
				LshFingerprint.ComputeSimilarity(this.fingerprint, lshFingerprint, out num, out num2, false, false);
			}
			return (double)num >= this.coefficient * (double)num2;
		}

		private readonly double coefficient;

		private readonly bool detectContainment;

		private LshFingerprint fingerprint;
	}
}
