using System;
using Microsoft.Exchange.Diagnostics.Components.TextProcessing;

namespace Microsoft.Exchange.TextProcessing
{
	internal class Fingerprint
	{
		public Fingerprint(long id, ushort[] fingerprintData, ulong shingleCount, int version)
		{
			this.Identifier = id;
			this.ShingleCount = shingleCount;
			this.FingerprintData = fingerprintData;
			this.Version = version;
		}

		public long Identifier { get; private set; }

		public int Version { get; private set; }

		public ulong ShingleCount { get; private set; }

		public ushort[] FingerprintData { get; private set; }

		public static double ComputeSimilarity(Fingerprint first, Fingerprint second)
		{
			if (first.Version != second.Version)
			{
				throw new ArgumentException(Strings.MismatchedFingerprintVersions(first.Version, second.Version));
			}
			if (first.FingerprintData.Length != second.FingerprintData.Length || first.FingerprintData.Length == 0)
			{
				throw new ArgumentException(Strings.MismatchedFingerprintSize(first.FingerprintData.Length, second.FingerprintData.Length));
			}
			ulong num = 0UL;
			for (int i = 0; i < first.FingerprintData.Length; i++)
			{
				if (first.FingerprintData[i] == second.FingerprintData[i])
				{
					num += 1UL;
				}
			}
			double num2 = num / (double)first.FingerprintData.Length;
			ExTraceGlobals.FingerprintTracer.TraceDebug<double, long, long>(0L, "Computed fingerprint similarity of '{0}', for fingerprints with id '{1}', '{2}'.", num2, first.Identifier, second.Identifier);
			return num2;
		}

		public static double ComputeContainment(Fingerprint template, Fingerprint document)
		{
			if (template.Version != document.Version)
			{
				throw new ArgumentException(Strings.MismatchedFingerprintVersions(template.Version, document.Version));
			}
			if (0UL == template.ShingleCount)
			{
				throw new ArgumentException(Strings.InvalidShingleCountForTemplate(template.ShingleCount));
			}
			double num = Fingerprint.ComputeSimilarity(template, document);
			double num2 = num / (1.0 + num) * ((template.ShingleCount + document.ShingleCount) / template.ShingleCount);
			ExTraceGlobals.FingerprintTracer.TraceDebug<double, long, long>(0L, "Computed fingerprint contaiment coefficient of '{0}' between template fingerprint with id '{1}' and document fingerprinting with id '{2}'.", num2, template.Identifier, document.Identifier);
			return num2;
		}
	}
}
