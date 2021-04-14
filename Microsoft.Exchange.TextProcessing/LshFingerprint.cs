using System;
using System.Globalization;

namespace Microsoft.Exchange.TextProcessing
{
	internal class LshFingerprint
	{
		private LshFingerprint(uint[] fingerprintData, string id = "")
		{
			this.Identifier = id;
			this.FingerprintData = fingerprintData;
		}

		public string Identifier { get; set; }

		public short Version
		{
			get
			{
				if (this.FingerprintData != null)
				{
					return (short)(this.FingerprintData[4] >> 16);
				}
				return 0;
			}
		}

		public short ShingleCount
		{
			get
			{
				if (this.FingerprintData != null)
				{
					return (short)(this.FingerprintData[4] & 65535U);
				}
				return 0;
			}
		}

		public uint[] FingerprintData { get; private set; }

		internal string EncodedFingerprintData
		{
			set
			{
				this.encodedFingerprintData = value;
			}
		}

		public static bool ShingleCountClose(LshFingerprint fingerprint, LshFingerprint otherFingerprint)
		{
			return fingerprint != null && otherFingerprint != null && fingerprint.ShingleCount != 0 && (double)Math.Abs((int)(fingerprint.ShingleCount - otherFingerprint.ShingleCount)) < 0.2 * (double)fingerprint.ShingleCount;
		}

		public static void ComputeSimilarity(LshFingerprint fingerprint, LshFingerprint otherFingerprint, out int similarityNumerator, out int similarityDenorminator, bool oneBit = false, bool forContainment = false)
		{
			similarityNumerator = 0;
			similarityDenorminator = (oneBit ? 32 : 48);
			if (LshFingerprint.NotQualifiedFingerprint(fingerprint) || LshFingerprint.NotQualifiedFingerprint(otherFingerprint))
			{
				return;
			}
			if (!forContainment && !LshFingerprint.ShingleCountClose(fingerprint, otherFingerprint))
			{
				similarityNumerator = 0;
				return;
			}
			uint[] fingerprintData = fingerprint.FingerprintData;
			uint[] fingerprintData2 = otherFingerprint.FingerprintData;
			uint num = oneBit ? 1U : 3U;
			for (int i = 0; i < 4; i++)
			{
				uint num2 = fingerprintData[i];
				uint num3 = fingerprintData2[i];
				for (int j = 0; j < 16; j++)
				{
					uint num4 = num2 & num;
					uint num5 = num3 & num;
					if (num4 == num5)
					{
						similarityNumerator++;
					}
					num2 >>= 2;
					num3 >>= 2;
				}
			}
			similarityNumerator = Math.Max(similarityNumerator - (oneBit ? 32 : 16), 0);
		}

		public static bool AcceptSimilar(LshFingerprint fingerprint, LshFingerprint otherFingerprint, bool oneBit = false)
		{
			if (LshFingerprint.NotQualifiedFingerprint(fingerprint) || LshFingerprint.NotQualifiedFingerprint(otherFingerprint))
			{
				return false;
			}
			if (!LshFingerprint.ShingleCountClose(fingerprint, otherFingerprint))
			{
				return false;
			}
			uint[] fingerprintData = fingerprint.FingerprintData;
			uint[] fingerprintData2 = otherFingerprint.FingerprintData;
			for (int i = 0; i < 4; i++)
			{
				if (oneBit)
				{
					if ((fingerprintData[i] & 1431655765U) == (fingerprintData2[i] & 1431655765U))
					{
						return true;
					}
				}
				else if (fingerprintData[i] == fingerprintData2[i])
				{
					return true;
				}
			}
			return false;
		}

		public static void ComputeContainment(LshFingerprint template, LshFingerprint document, out int containmentNumerator, out int containmentDenorminator)
		{
			int num;
			int num2;
			LshFingerprint.ComputeSimilarity(template, document, out num, out num2, false, true);
			if (num == 0)
			{
				containmentNumerator = 0;
				containmentDenorminator = 1;
				return;
			}
			containmentNumerator = num * (int)(template.ShingleCount + document.ShingleCount);
			containmentDenorminator = (num + num2) * (int)template.ShingleCount;
			if (containmentNumerator > containmentDenorminator)
			{
				containmentNumerator = containmentDenorminator;
			}
		}

		public static bool TryDecode(string encodedFingerprint, out LshFingerprint decodedFingerprint)
		{
			if (!string.IsNullOrEmpty(encodedFingerprint))
			{
				string[] array = encodedFingerprint.Split(LshFingerprintConstants.DotDelimit);
				int num = 5;
				if (array.Length == num)
				{
					uint[] array2 = new uint[num];
					for (int i = 0; i < array2.Length; i++)
					{
						if (!uint.TryParse(array[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out array2[i]))
						{
							decodedFingerprint = LshFingerprint.EmptyFingerprint;
							return false;
						}
					}
					short num2 = (short)(array2[4] >> 16);
					if (num2 == 2)
					{
						decodedFingerprint = new LshFingerprint(array2, "");
						return true;
					}
				}
			}
			decodedFingerprint = LshFingerprint.EmptyFingerprint;
			return false;
		}

		public static bool TryCreateFingerprint(uint[] fingerprintData, out LshFingerprint createdFingerprint, string id = "")
		{
			if (fingerprintData != null)
			{
				if (fingerprintData.Length != 5)
				{
					createdFingerprint = LshFingerprint.EmptyFingerprint;
					return false;
				}
				short num = (short)(fingerprintData[4] >> 16);
				if (num != 2)
				{
					createdFingerprint = LshFingerprint.EmptyFingerprint;
					return false;
				}
			}
			createdFingerprint = new LshFingerprint(fingerprintData, id);
			return true;
		}

		public static LshFingerprint GetEmptyFingerprint()
		{
			return LshFingerprint.EmptyFingerprint;
		}

		public string Encode()
		{
			if (this.encodedFingerprintData == null)
			{
				if (this.FingerprintData != null)
				{
					this.encodedFingerprintData = string.Format("{0}.{1}.{2}.{3}.{4}", new object[]
					{
						this.FingerprintData[0].ToString("X"),
						this.FingerprintData[1].ToString("X"),
						this.FingerprintData[2].ToString("X"),
						this.FingerprintData[3].ToString("X"),
						this.FingerprintData[4].ToString("X")
					});
				}
				else
				{
					this.encodedFingerprintData = string.Empty;
				}
			}
			return this.encodedFingerprintData;
		}

		private static bool NotQualifiedFingerprint(LshFingerprint fingerprint)
		{
			return fingerprint == null || fingerprint.Version != 2 || fingerprint.FingerprintData == null || fingerprint.FingerprintData.Length != 5 || fingerprint.ShingleCount <= 10;
		}

		private const double ShingleCountThreshold = 0.2;

		private static readonly LshFingerprint EmptyFingerprint = new LshFingerprint(null, "");

		private string encodedFingerprintData;
	}
}
