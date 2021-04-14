using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class CodingSchemeInfo
	{
		static CodingSchemeInfo()
		{
			CodingSchemeInfo.known.Add(CodingScheme.GsmDefault, new CodingSchemeInfo(CodingScheme.GsmDefault, 7, new ReadOnlyCollection<int>(new int[]
			{
				1,
				2
			}), new GsmDefaultCoder()));
			CodingSchemeInfo.known.Add(CodingScheme.Unicode, new CodingSchemeInfo(CodingScheme.Unicode, 16, new ReadOnlyCollection<int>(new int[]
			{
				1
			}), new UnicodeCoder()));
			CodingSchemeInfo.known.Add(CodingScheme.UsAscii, new CodingSchemeInfo(CodingScheme.UsAscii, 7, new ReadOnlyCollection<int>(new int[]
			{
				1
			}), new UsAsciiCoder()));
			CodingSchemeInfo.known.Add(CodingScheme.Ia5, new CodingSchemeInfo(CodingScheme.Ia5, 7, new ReadOnlyCollection<int>(new int[]
			{
				1
			}), new Ia5Coder()));
			CodingSchemeInfo.known.Add(CodingScheme.Iso_8859_1, new CodingSchemeInfo(CodingScheme.Iso_8859_1, 8, new ReadOnlyCollection<int>(new int[]
			{
				1
			}), new Iso_8859_1Coder()));
			CodingSchemeInfo.known.Add(CodingScheme.Iso_8859_8, new CodingSchemeInfo(CodingScheme.Iso_8859_8, 8, new ReadOnlyCollection<int>(new int[]
			{
				1
			}), new Iso_8859_8Coder()));
			CodingSchemeInfo.known.Add(CodingScheme.ShiftJis, new CodingSchemeInfo(CodingScheme.ShiftJis, 8, new ReadOnlyCollection<int>(new int[]
			{
				1,
				2
			}), new ShiftJisCoder()));
			CodingSchemeInfo.known.Add(CodingScheme.EucKr, new CodingSchemeInfo(CodingScheme.EucKr, 8, new ReadOnlyCollection<int>(new int[]
			{
				1,
				2
			}), new EucKrCoder()));
		}

		private CodingSchemeInfo(CodingScheme codingScheme, int codingBitsRadix, IList<int> codingRadixesAllowance, ICoder coder)
		{
			if (codingScheme == CodingScheme.Neutral)
			{
				throw new ArgumentOutOfRangeException("codingScheme");
			}
			if (0 >= codingBitsRadix)
			{
				throw new ArgumentOutOfRangeException("codingBitsRadix");
			}
			if (codingRadixesAllowance == null)
			{
				throw new ArgumentNullException("codingRadixesAllowance");
			}
			if (coder == null)
			{
				throw new ArgumentNullException("coder");
			}
			foreach (int num in codingRadixesAllowance)
			{
				if (0 >= num)
				{
					throw new ArgumentOutOfRangeException("codingRadixesAllowance");
				}
			}
			this.CodingScheme = codingScheme;
			this.CodingBitsRadix = codingBitsRadix;
			List<int> list = new List<int>(codingRadixesAllowance);
			list.Sort();
			this.CodingRadixesAllowance = list.AsReadOnly();
			this.Coder = coder;
		}

		public CodingScheme CodingScheme { get; private set; }

		public int CodingBitsRadix { get; private set; }

		public IList<int> CodingRadixesAllowance { get; private set; }

		public ICoder Coder { get; private set; }

		public CodingCategory CodingCategory
		{
			get
			{
				if (1 == this.CodingRadixesAllowance.Count)
				{
					return CodingCategory.Fixed;
				}
				return CodingCategory.Variant;
			}
		}

		public static CodingSchemeInfo GetCodingSchemeInfo(CodingScheme codingScheme)
		{
			if (!CodingSchemeInfo.known.ContainsKey(codingScheme))
			{
				throw new ArgumentOutOfRangeException("codingScheme");
			}
			return CodingSchemeInfo.known[codingScheme];
		}

		private static IDictionary<CodingScheme, CodingSchemeInfo> known = new Dictionary<CodingScheme, CodingSchemeInfo>();
	}
}
