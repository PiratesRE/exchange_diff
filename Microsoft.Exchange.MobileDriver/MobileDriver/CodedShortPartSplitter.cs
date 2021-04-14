using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class CodedShortPartSplitter : PureSplitterBase
	{
		public CodedShortPartSplitter(CodingScheme codingScheme, int radixPerPart) : this(codingScheme, radixPerPart, true, '?', 0)
		{
		}

		public CodedShortPartSplitter(CodingScheme codingScheme, int radixPerPart, int maximumSegments) : this(codingScheme, radixPerPart, true, '?', maximumSegments)
		{
		}

		public CodedShortPartSplitter(CodingScheme codingScheme, int radixPerPart, char fallbackCharacter) : this(codingScheme, radixPerPart, false, fallbackCharacter, 0)
		{
		}

		public CodedShortPartSplitter(CodingScheme codingScheme, int radixPerPart, char fallbackCharacter, int maximumSegments) : this(codingScheme, radixPerPart, false, fallbackCharacter, maximumSegments)
		{
		}

		private CodedShortPartSplitter(CodingScheme codingScheme, int radixPerPart, bool throwIfNotCodable, char fallbackCharacter, int maximumSegments)
		{
			this.coding = new CodingSupportability(codingScheme, radixPerPart, radixPerPart);
			this.ThrowIfNotCodable = throwIfNotCodable;
			if (this.coding.CodingSchemeInfo.Coder.GetCodedRadixCount(fallbackCharacter) == 0)
			{
				throw new ArgumentOutOfRangeException("fallbackCharacter");
			}
			if (0 > maximumSegments)
			{
				throw new ArgumentOutOfRangeException("maximumSegments");
			}
			this.FallbackCharacter = fallbackCharacter;
			this.maximumSegments = maximumSegments;
		}

		public CodingScheme CodingScheme
		{
			get
			{
				return this.coding.CodingScheme;
			}
		}

		public int RadixPerPart
		{
			get
			{
				return this.coding.RadixPerPart;
			}
		}

		public bool ThrowIfNotCodable { get; private set; }

		public char FallbackCharacter { get; private set; }

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
			CodedText codedText = this.coding.CodingSchemeInfo.Coder.Code(text);
			if (!codedText.CanBeCodedEntirely)
			{
				if (this.ThrowIfNotCodable)
				{
					throw new MobileDriverCantBeCodedException(Strings.ErrorCantBeCoded(codedText.CodingScheme.ToString(), codedText.ToString()));
				}
				codedText.ReplaceUncodableCharacters(this.FallbackCharacter);
			}
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
			if (this.maximumSegments != 0)
			{
				while (list.Count > this.maximumSegments)
				{
					list.RemoveAt(list.Count - 1);
				}
				if (list.Count == this.maximumSegments)
				{
					BookmarkHelper.RebuildBookmarksWithTrailingEllipsis(list);
					return list;
				}
			}
			int num = (0 < list.Count) ? (list[list.Count - 1].EndLocation + 1) : 0;
			int num2 = 0;
			while (codedText.Text.Length > num)
			{
				int num3 = codedText.GetRadixCount(num);
				if (num3 == 0)
				{
					num3 = this.coding.CodingSchemeInfo.Coder.GetCodedRadixCount(this.FallbackCharacter);
				}
				if (num2 == 0)
				{
					if (0 < list.Count)
					{
						list[list.Count - 1] = new Bookmark(list[list.Count - 1].FullText, list[list.Count - 1].PartType, list[list.Count - 1].PartNumber, list[list.Count - 1].CodingScheme, list[list.Count - 1].BeginLocation, num - 1);
						if (this.maximumSegments != 0 && list.Count == this.maximumSegments)
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
					list.Add(new Bookmark(codedText.Text, PartType.Short, (0 < list.Count) ? (list[list.Count - 1].PartNumber + 1) : 0, codedText.CodingScheme, num, -1));
				}
				num2 += num3;
				if (this.coding.RadixPerPart < num2)
				{
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

		private CodingSupportability coding;

		private int maximumSegments;
	}
}
