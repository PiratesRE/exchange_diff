using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class GsmShortPartSplitter : PureSplitterBase
	{
		public GsmShortPartSplitter() : this(160, 70)
		{
		}

		public GsmShortPartSplitter(int gsmDefaultPerPart, int unicodePerPart) : this(gsmDefaultPerPart, unicodePerPart, 0)
		{
		}

		public GsmShortPartSplitter(int gsmDefaultPerPart, int unicodePerPart, int maximumSegments)
		{
			if (0 >= gsmDefaultPerPart)
			{
				throw new ArgumentOutOfRangeException("gsmDefaultPerPart");
			}
			if (0 >= unicodePerPart)
			{
				throw new ArgumentOutOfRangeException("unicodePerPart");
			}
			if (0 > maximumSegments)
			{
				throw new ArgumentOutOfRangeException("maximumSegments");
			}
			this.GsmDefaultCoding = new CodingSupportability(CodingScheme.GsmDefault, gsmDefaultPerPart, gsmDefaultPerPart);
			this.UnicodeCoding = new CodingSupportability(CodingScheme.Unicode, unicodePerPart, unicodePerPart);
			this.MaximumSegments = maximumSegments;
		}

		public int GsmDefaultPerPart
		{
			get
			{
				return this.GsmDefaultCoding.RadixPerPart;
			}
		}

		public int UnicodePerPart
		{
			get
			{
				return this.UnicodeCoding.RadixPerPart;
			}
		}

		public override bool OnePass
		{
			get
			{
				return true;
			}
		}

		public override PartType PartType
		{
			get
			{
				return PartType.Short;
			}
		}

		protected CodingSupportability GsmDefaultCoding { get; set; }

		protected CodingSupportability UnicodeCoding { get; set; }

		private protected int MaximumSegments { protected get; private set; }

		internal override IList<Bookmark> Split(string text, IEnumerable<Bookmark> existing, int desiredCount, out bool more)
		{
			more = false;
			if (string.IsNullOrEmpty(text))
			{
				return new ReadOnlyCollection<Bookmark>(new Bookmark[]
				{
					Bookmark.Empty
				});
			}
			CodedText codedText = this.GsmDefaultCoding.CodingSchemeInfo.Coder.Code(text);
			CodedText codedText2 = this.UnicodeCoding.CodingSchemeInfo.Coder.Code(text);
			bool flag = 0 > desiredCount;
			List<Bookmark> list;
			if (existing == null)
			{
				list = new List<Bookmark>();
			}
			else
			{
				list = new List<Bookmark>(existing);
				list.Sort((Bookmark a, Bookmark b) => a.BeginLocation.CompareTo(b.BeginLocation));
			}
			if (0 < list.Count && list[list.Count - 1].IncompleteEnd)
			{
				throw new ArgumentOutOfRangeException("existing");
			}
			if (this.MaximumSegments != 0)
			{
				while (list.Count > this.MaximumSegments)
				{
					list.RemoveAt(list.Count - 1);
				}
				if (list.Count == this.MaximumSegments)
				{
					BookmarkHelper.RebuildBookmarksWithTrailingEllipsis(list);
					return list;
				}
			}
			CodedText codedText3 = codedText;
			int num = (0 < list.Count) ? (list[list.Count - 1].EndLocation + 1) : 0;
			int num2 = 0;
			while (codedText3.ToString().Length > num)
			{
				int radixCount = codedText3.GetRadixCount(num);
				if (CodingScheme.GsmDefault == codedText3.CodingScheme && radixCount == 0)
				{
					codedText3 = codedText2;
					radixCount = codedText3.GetRadixCount(num);
					int num3 = (0 < list.Count) ? (num - list[list.Count - 1].BeginLocation) : num;
					if (this.GetRadixPerPart(codedText3.CodingScheme) < num3)
					{
						num2 = 0;
					}
					else if (0 < list.Count)
					{
						list[list.Count - 1] = new Bookmark(list[list.Count - 1].FullText, list[list.Count - 1].PartType, list[list.Count - 1].PartNumber, codedText3.CodingScheme, list[list.Count - 1].BeginLocation, list[list.Count - 1].EndLocation);
					}
				}
				if (num2 == 0)
				{
					if (0 < list.Count)
					{
						list[list.Count - 1] = new Bookmark(list[list.Count - 1].FullText, list[list.Count - 1].PartType, list[list.Count - 1].PartNumber, list[list.Count - 1].CodingScheme, list[list.Count - 1].BeginLocation, num - 1);
						if (this.MaximumSegments != 0 && list.Count == this.MaximumSegments)
						{
							BookmarkHelper.RebuildBookmarksWithTrailingEllipsis(list);
							break;
						}
					}
					if (!flag && 0 > --desiredCount)
					{
						more = true;
						break;
					}
					list.Add(new Bookmark(codedText3.Text, PartType.Short, (0 < list.Count) ? (list[list.Count - 1].PartNumber + 1) : 0, codedText3.CodingScheme, num, -1));
				}
				num2 += radixCount;
				if (this.GetRadixPerPart(codedText3.CodingScheme) < num2)
				{
					if (CodingScheme.Unicode == codedText3.CodingScheme)
					{
						codedText3 = codedText;
					}
					num--;
					num2 = 0;
				}
				num++;
			}
			if (0 < list.Count && -1 == list[list.Count - 1].EndLocation)
			{
				list[list.Count - 1] = new Bookmark(list[list.Count - 1].FullText, list[list.Count - 1].PartType, list[list.Count - 1].PartNumber, list[list.Count - 1].CodingScheme, list[list.Count - 1].BeginLocation, num - 1);
			}
			return list.AsReadOnly();
		}

		private int GetRadixPerPart(CodingScheme codingScheme)
		{
			switch (codingScheme)
			{
			case CodingScheme.GsmDefault:
				return this.GsmDefaultPerPart;
			case CodingScheme.Unicode:
				return this.UnicodePerPart;
			default:
				throw new ArgumentOutOfRangeException("codingScheme");
			}
		}

		public const int MaxGsmDefaultPerPart = 160;

		public const int MaxUnicodePerPart = 70;
	}
}
