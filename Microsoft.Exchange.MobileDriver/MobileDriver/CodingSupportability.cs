using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class CodingSupportability
	{
		public CodingSupportability(CodingScheme codingScheme, int radixPerPart, int radixPerSegment)
		{
			if (codingScheme == CodingScheme.Neutral)
			{
				throw new ArgumentOutOfRangeException("codingScheme");
			}
			if (0 >= radixPerPart)
			{
				throw new ArgumentOutOfRangeException("radixPerPart");
			}
			if (0 >= radixPerSegment)
			{
				throw new ArgumentOutOfRangeException("radixPerSegment");
			}
			this.CodingScheme = codingScheme;
			this.RadixPerSegment = radixPerSegment;
			this.RadixPerPart = radixPerPart;
		}

		public CodingScheme CodingScheme { get; private set; }

		public int RadixPerSegment { get; private set; }

		public int RadixPerPart { get; private set; }

		public CodingSchemeInfo CodingSchemeInfo
		{
			get
			{
				if (this.codingSchemeInfo == null)
				{
					this.codingSchemeInfo = CodingSchemeInfo.GetCodingSchemeInfo(this.CodingScheme);
				}
				return this.codingSchemeInfo;
			}
		}

		private CodingSchemeInfo codingSchemeInfo;
	}
}
