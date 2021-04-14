using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class EllipsisTrailer
	{
		public EllipsisTrailer(CodingScheme codingScheme)
		{
			this.Coder = CodingSchemeInfo.GetCodingSchemeInfo(codingScheme).Coder;
		}

		private ICoder Coder { get; set; }

		public string Trail(string original)
		{
			return this.Trail(original, -1);
		}

		public string Trail(string original, int endLocation)
		{
			if (string.IsNullOrEmpty(original))
			{
				return original;
			}
			if (original.Length <= endLocation)
			{
				throw new ArgumentOutOfRangeException("endLocation");
			}
			if (0 > endLocation)
			{
				endLocation = original.Length - 1;
			}
			foreach (string text in EllipsisTrailer.EllipsisCandidates)
			{
				string str = text;
				int num = 0;
				foreach (char ch in text)
				{
					num += this.Coder.GetCodedRadixCount(ch);
				}
				if (0 < num)
				{
					int num2 = 0;
					int num3 = endLocation;
					while (0 <= num3)
					{
						num2 += this.Coder.GetCodedRadixCount(original[num3]);
						if (num <= num2)
						{
							return original.Substring(0, num3) + str;
						}
						num3--;
					}
				}
			}
			return original;
		}

		public const char Ellipsis = '…';

		public const char Dot = '.';

		public static readonly string EllipsisString = '…'.ToString();

		public static readonly string DotDotDotString = string.Format("{0}{1}{2}", '.', '.', '.');

		public static readonly IList<string> EllipsisCandidates = new ReadOnlyCollection<string>(new string[]
		{
			EllipsisTrailer.EllipsisString,
			EllipsisTrailer.DotDotDotString
		});
	}
}
