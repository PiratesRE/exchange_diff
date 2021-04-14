using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class GsmConcatenatedPartSplitter : GsmShortPartSplitter
	{
		public GsmConcatenatedPartSplitter() : this(160, 70, 153, 67, 255, false)
		{
		}

		public GsmConcatenatedPartSplitter(int gsmDefaultPerPart, int unicodePerPart, int gsmDefaultPerSegment, int unicodePerSegment, int segmentsPerConcatenatedPart, bool onePassSplitting) : this(gsmDefaultPerPart, unicodePerPart, gsmDefaultPerSegment, unicodePerSegment, segmentsPerConcatenatedPart, onePassSplitting, 0)
		{
		}

		public GsmConcatenatedPartSplitter(int gsmDefaultPerPart, int unicodePerPart, int gsmDefaultPerSegment, int unicodePerSegment, int segmentsPerConcatenatedPart, bool onePassSplitting, int maximumSegments) : base(gsmDefaultPerPart, unicodePerPart, maximumSegments)
		{
			if (0 >= gsmDefaultPerPart)
			{
				throw new ArgumentOutOfRangeException("gsmDefaultPerPart");
			}
			if (0 >= unicodePerPart)
			{
				throw new ArgumentOutOfRangeException("unicodePerPart");
			}
			if (0 >= gsmDefaultPerSegment)
			{
				throw new ArgumentOutOfRangeException("gsmDefaultPerSegment");
			}
			if (0 >= unicodePerSegment)
			{
				throw new ArgumentOutOfRangeException("unicodePerSegment");
			}
			if (0 >= segmentsPerConcatenatedPart)
			{
				throw new ArgumentOutOfRangeException("segmentsPerConcatenatedPart");
			}
			base.GsmDefaultCoding = new CodingSupportability(CodingScheme.GsmDefault, gsmDefaultPerPart, gsmDefaultPerSegment);
			base.UnicodeCoding = new CodingSupportability(CodingScheme.Unicode, unicodePerPart, unicodePerSegment);
			this.SegmentsPerConcatenatedPart = segmentsPerConcatenatedPart;
			this.onePass = onePassSplitting;
		}

		public int GsmDefaultPerSegment
		{
			get
			{
				return base.GsmDefaultCoding.RadixPerSegment;
			}
		}

		public int UnicodePerSegment
		{
			get
			{
				return base.UnicodeCoding.RadixPerSegment;
			}
		}

		public int SegmentsPerConcatenatedPart { get; private set; }

		public override bool OnePass
		{
			get
			{
				return this.onePass;
			}
		}

		public override PartType PartType
		{
			get
			{
				return PartType.Concatenated;
			}
		}

		internal override IList<Bookmark> Split(string text, IEnumerable<Bookmark> existing, int desiredCount, out bool more)
		{
			if (1 == this.SegmentsPerConcatenatedPart)
			{
				return base.Split(text, existing, desiredCount, out more);
			}
			more = false;
			if (string.IsNullOrEmpty(text))
			{
				return new ReadOnlyCollection<Bookmark>(new Bookmark[]
				{
					Bookmark.Empty
				});
			}
			CodedText codedText = base.GsmDefaultCoding.CodingSchemeInfo.Coder.Code(text);
			CodedText codedText2 = base.UnicodeCoding.CodingSchemeInfo.Coder.Code(text);
			bool flag = 0 > desiredCount;
			int num = 0;
			List<Bookmark> list;
			if (existing == null)
			{
				list = new List<Bookmark>();
			}
			else
			{
				list = new List<Bookmark>(existing);
				list.Sort((Bookmark a, Bookmark b) => a.BeginLocation.CompareTo(b.BeginLocation));
				num = list.Count;
			}
			if (base.MaximumSegments != 0)
			{
				while (list.Count > base.MaximumSegments)
				{
					list.RemoveAt(list.Count - 1);
				}
				if (!this.onePass && 0 < list.Count && 1 == this.GetLastGroupSegmentCount(list))
				{
					list.RemoveAt(list.Count - 1);
					desiredCount++;
				}
				if (list.Count == base.MaximumSegments)
				{
					BookmarkHelper.RebuildBookmarksWithTrailingEllipsis(list);
					return list;
				}
			}
			CodedText codedText3 = codedText;
			int num2 = -1;
			bool flag2 = false;
			int num3 = 0;
			int num4 = 0;
			if (0 < list.Count)
			{
				Bookmark bookmark = list[list.Count - 1];
				num2 = bookmark.PartNumber;
				flag2 = bookmark.IncompleteEnd;
				num3 = bookmark.EndLocation + (flag2 ? 0 : 1);
				num4 = this.GetLastGroupSegmentCount(list) % this.SegmentsPerConcatenatedPart;
				if (num4 != 0)
				{
					codedText3 = ((CodingScheme.GsmDefault == bookmark.CodingScheme) ? codedText : codedText2);
				}
			}
			int num5 = 0;
			while (codedText3.ToString().Length > num3)
			{
				int radixCount = codedText3.GetRadixCount(num3);
				if (CodingScheme.GsmDefault == codedText3.CodingScheme && radixCount == 0)
				{
					codedText3 = codedText2;
					radixCount = codedText3.GetRadixCount(num3);
					if (0 < list.Count)
					{
						int lastGroupSegmentCount = this.GetLastGroupSegmentCount(list);
						int beginLocation = list[list.Count - lastGroupSegmentCount].BeginLocation;
					}
					if (0 < list.Count)
					{
						if (!this.OnePass)
						{
							desiredCount += list.Count;
							list.Clear();
							num3 = 0;
							num2 = -1;
						}
						else if (1 == list.Count - num)
						{
							list.RemoveAt(list.Count - 1);
							desiredCount++;
							num3 = ((0 < list.Count) ? (list[list.Count - 1].EndLocation + 1) : 0);
						}
					}
					num4 = 0;
					flag2 = false;
					num5 = 0;
				}
				if (num5 == 0)
				{
					if (num4 == 0)
					{
						flag2 = false;
						num2++;
					}
					if (0 < list.Count)
					{
						list[list.Count - 1] = new Bookmark(list[list.Count - 1].FullText, list[list.Count - 1].PartType, list[list.Count - 1].PartNumber, list[list.Count - 1].CodingScheme, list[list.Count - 1].BeginLocation, flag2 ? num3 : (num3 - 1), list[list.Count - 1].IncompleteBegin, flag2);
						if (base.MaximumSegments != 0 && list.Count == base.MaximumSegments)
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
					if (num4 == 0)
					{
						bool flag3 = false;
						IList<Bookmark> result = base.Split(text, list, 1, out flag3);
						if (!flag3)
						{
							return result;
						}
					}
					list.Add(new Bookmark(codedText3.Text, PartType.Concatenated, num2, codedText3.CodingScheme, num3, -1, flag2, false));
					num4 = (num4 + 1) % this.SegmentsPerConcatenatedPart;
				}
				if (flag2)
				{
					num5--;
				}
				num5 += radixCount;
				if (this.GetRadixPerSegment(codedText3.CodingScheme) < num5)
				{
					num5 %= this.GetRadixPerSegment(codedText3.CodingScheme);
					flag2 = (1 == num5 && 2 == radixCount);
					if (num4 == 0 && CodingScheme.Unicode == codedText3.CodingScheme)
					{
						codedText3 = codedText;
					}
					num3--;
					num5 = 0;
				}
				else
				{
					flag2 = false;
				}
				num3++;
			}
			if (0 < list.Count && -1 == list[list.Count - 1].EndLocation)
			{
				list[list.Count - 1] = new Bookmark(list[list.Count - 1].FullText, list[list.Count - 1].PartType, list[list.Count - 1].PartNumber, list[list.Count - 1].CodingScheme, list[list.Count - 1].BeginLocation, num3 - 1, list[list.Count - 1].IncompleteBegin, false);
			}
			return list.AsReadOnly();
		}

		private int GetRadixPerSegment(CodingScheme codingScheme)
		{
			switch (codingScheme)
			{
			case CodingScheme.GsmDefault:
				return this.GsmDefaultPerSegment;
			case CodingScheme.Unicode:
				return this.UnicodePerSegment;
			default:
				throw new ArgumentOutOfRangeException("codingScheme");
			}
		}

		private int GetLastGroupSegmentCount(IList<Bookmark> bookmarks)
		{
			int num = 0;
			int num2 = bookmarks.Count - 1;
			Bookmark bookmark = bookmarks[num2];
			while (0 <= num2 - num && bookmark.PartNumber == bookmarks[num2 - num].PartNumber)
			{
				num++;
			}
			if (this.SegmentsPerConcatenatedPart < num)
			{
				throw new ArgumentOutOfRangeException("bookmarks");
			}
			return num;
		}

		public const int MaxSegmentsPerConcatenatedPart = 255;

		public const int MaxGsmDefaultPer8BitSegment = 153;

		public const int MaxUnicodePer8BitSegment = 67;

		public const int MaxGsmDefaultPer16BitSegment = 151;

		public const int MaxUnicodePer16BitSegment = 66;

		private bool onePass;
	}
}
