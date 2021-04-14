using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class BodyDiffer
	{
		public BodyDiffer(ConversationBodyScanner parentScanner, ConversationBodyScanner childScanner)
		{
			this.childBodyFragment = new BodyFragmentInfo(childScanner);
			this.parentBodyFragment = new BodyFragmentInfo(parentScanner);
			this.WordByWordDiff();
		}

		public BodyDiffer(BodyFragmentInfo childBodyFragment, BodyFragmentInfo parentBodyFragment)
		{
			this.childBodyFragment = childBodyFragment;
			this.parentBodyFragment = parentBodyFragment;
			this.Diff();
		}

		public FragmentInfo UniqueBodyPart
		{
			get
			{
				return this.uniqueBodyPart;
			}
		}

		public FragmentInfo DisclaimerPart
		{
			get
			{
				return this.disclaimerPart;
			}
		}

		internal ConversationBodyScanner ChildScanner
		{
			get
			{
				return this.childBodyFragment.BodyScanner;
			}
		}

		internal ConversationBodyScanner ParentScanner
		{
			get
			{
				return this.parentBodyFragment.BodyScanner;
			}
		}

		internal int LastUniqueWordIndex
		{
			get
			{
				return this.lastUniqueWord;
			}
		}

		public void WriteUniqueBody(HtmlWriter streamWriter)
		{
			this.UniqueBodyPart.WriteHtml(streamWriter);
		}

		private static bool IsSameWord(TextRun left, TextRun right)
		{
			return BodyDiffer.IsSameWord(left, right, 0, left.WordLength, 0, right.WordLength);
		}

		private static bool IsSameWord(TextRun left, TextRun right, int leftBegin, int leftEnd, int rightBegin, int rightEnd)
		{
			if (leftEnd - leftBegin != rightEnd - rightBegin)
			{
				return false;
			}
			if (leftEnd <= left.WordLength && rightEnd <= right.WordLength)
			{
				while (leftBegin < leftEnd && rightBegin < rightEnd)
				{
					if (!BodyDiffer.IsSameChar(left.GetWordChar(leftBegin), right.GetWordChar(rightBegin)))
					{
						return false;
					}
					leftBegin++;
					rightBegin++;
				}
				return true;
			}
			return false;
		}

		private static bool IsSameChar(char left, char right)
		{
			if (left == right)
			{
				return true;
			}
			char c;
			if (!BodyDiffer.SameCharacters.TryGetValue(left, out c))
			{
				c = left;
			}
			char c2;
			if (!BodyDiffer.SameCharacters.TryGetValue(right, out c2))
			{
				c2 = right;
			}
			return c == c2;
		}

		private static bool GetHrefTagIndex(TextRun word, out int beginIndex, out int endIndex)
		{
			char c = '<';
			beginIndex = (endIndex = -1);
			int i;
			for (i = word.WordLength - 1; i >= 0; i--)
			{
				if (word.GetWordChar(i) == '>')
				{
					c = '<';
					endIndex = i;
					break;
				}
				if (word.GetWordChar(i) == ']')
				{
					c = '[';
					endIndex = i;
					break;
				}
			}
			for (i -= 5; i >= 0; i--)
			{
				if (word.GetWordChar(i) == c)
				{
					beginIndex = i;
					return true;
				}
			}
			endIndex = -1;
			beginIndex = -1;
			return false;
		}

		private static bool IsListNumberOrBullet(TextRun textRun)
		{
			char wordChar = textRun.GetWordChar(0);
			switch (wordChar)
			{
			case '*':
			case '+':
				return textRun.WordLength == 1;
			case ',':
			case '-':
			case '.':
			case '/':
			case '0':
				break;
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				if (textRun.WordLength == 2)
				{
					return textRun.GetWordChar(1) == '.' || textRun.GetWordChar(1) == ')' || textRun.GetWordChar(1) == ':';
				}
				return textRun.WordLength == 3 && char.IsDigit(textRun.GetWordChar(1)) && (textRun.GetWordChar(2) == '.' || textRun.GetWordChar(2) == ')' || textRun.GetWordChar(2) == ':');
			default:
				switch (wordChar)
				{
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
				case 'g':
				case 'h':
					return textRun.WordLength == 2 && (textRun.GetWordChar(1) == '.' || textRun.GetWordChar(1) == ')');
				}
				break;
			}
			return false;
		}

		private static int GetLastNormalFragmentIndex(BodyFragmentInfo bodyFragmentInfo, int beforeWordIndex)
		{
			int num = -1;
			int num2 = 0;
			while (num2 < bodyFragmentInfo.BodyScanner.Fragments.Count && (int)bodyFragmentInfo.BodyScanner.Fragments[num2].FirstLine < bodyFragmentInfo.EndLineIndex && bodyFragmentInfo.BodyScanner.Fragments[num2].FirstWord <= beforeWordIndex)
			{
				if (bodyFragmentInfo.BodyScanner.Fragments[num2].Category == ConversationBodyScanner.Scanner.FragmentCategory.Normal)
				{
					num = num2;
				}
				num2++;
			}
			if (num == -1)
			{
				num = num2;
			}
			return num;
		}

		private static Dictionary<char, char> SameCharactersCollection()
		{
			return new Dictionary<char, char>
			{
				{
					'‘',
					'\''
				},
				{
					'“',
					'"'
				},
				{
					'”',
					'"'
				},
				{
					'’',
					'\''
				},
				{
					'–',
					'-'
				}
			};
		}

		private bool IsHrefBeginText(int iChildWordIndex, bool forwardDirection)
		{
			string text = forwardDirection ? "<<" : ">>";
			int wordLength = this.ChildScanner.Words[iChildWordIndex].WordLength;
			int length = text.Length;
			if (wordLength < length)
			{
				return false;
			}
			int num = forwardDirection ? 0 : (wordLength - length);
			int num2 = 0;
			while (num2 < length && num2 >= 0)
			{
				if (this.ChildScanner.Words[iChildWordIndex].GetWordChar(num) != text[num2])
				{
					return false;
				}
				num2++;
				num++;
			}
			return true;
		}

		private int HrefTextEndIndex(int iChildWordIndex, bool forwardDirection)
		{
			int num = forwardDirection ? 1 : -1;
			if (this.IsHrefBeginText(iChildWordIndex, forwardDirection))
			{
				int num2 = 1;
				while (num2 < 10 && iChildWordIndex > 0 && iChildWordIndex < this.ChildScanner.Words.Count)
				{
					if (this.IsHrefBeginText(iChildWordIndex, !forwardDirection))
					{
						return iChildWordIndex + num;
					}
					num2++;
					iChildWordIndex += num;
				}
			}
			return -1;
		}

		private bool MatchWordWithHref(int iChildWordIndex, int iParentWordIndex)
		{
			TextRun textRun = this.ChildScanner.Words[iChildWordIndex];
			TextRun right = this.ParentScanner.Words[iParentWordIndex];
			int num;
			int num2;
			return BodyDiffer.GetHrefTagIndex(textRun, out num, out num2) && BodyDiffer.IsSameWord(textRun, right, 0, num, 0, num) && BodyDiffer.IsSameWord(textRun, right, num2 + 1, textRun.WordLength, num, right.WordLength);
		}

		private bool IsHorizontalLine(TextRun word)
		{
			for (int i = word.WordLength - 1; i >= 0; i--)
			{
				if (word.GetWordChar(i) != '_' && word.GetWordChar(i) != '_')
				{
					return false;
				}
			}
			return true;
		}

		private bool MatchWord(ref int childWordIndex, ref int parentWordIndex, bool forwardMatch, int recursionLevel)
		{
			int num = forwardMatch ? 1 : -1;
			if (recursionLevel > 5)
			{
				return false;
			}
			if (childWordIndex >= this.childBodyFragment.EndWordIndex || parentWordIndex >= this.parentBodyFragment.EndWordIndex || childWordIndex < 0 || parentWordIndex < 0)
			{
				return false;
			}
			if (BodyDiffer.IsSameWord(this.ParentScanner.Words[parentWordIndex], this.ChildScanner.Words[childWordIndex]))
			{
				return true;
			}
			int num5;
			int num6;
			if (this.IsHrefBeginText(childWordIndex, forwardMatch))
			{
				int num2 = this.HrefTextEndIndex(childWordIndex, forwardMatch);
				if (this.MatchWord(ref num2, ref parentWordIndex, forwardMatch, recursionLevel + 1))
				{
					childWordIndex = num2;
					return true;
				}
			}
			else if (this.IsHorizontalLine(this.ChildScanner.Words[childWordIndex]))
			{
				int num3 = childWordIndex + num;
				if (this.MatchWord(ref num3, ref parentWordIndex, forwardMatch, recursionLevel + 1))
				{
					childWordIndex = num3;
					return true;
				}
			}
			else if (this.IsHorizontalLine(this.ParentScanner.Words[parentWordIndex]))
			{
				int num4 = parentWordIndex + num;
				if (this.MatchWord(ref childWordIndex, ref num4, forwardMatch, recursionLevel + 1))
				{
					parentWordIndex = num4;
					return true;
				}
			}
			else if (BodyDiffer.GetHrefTagIndex(this.ChildScanner.Words[childWordIndex], out num5, out num6))
			{
				if (num5 == 0)
				{
					int num7 = childWordIndex + num;
					if (this.MatchWord(ref num7, ref parentWordIndex, forwardMatch, recursionLevel + 1))
					{
						childWordIndex = num7;
						return true;
					}
				}
				else if (this.MatchWordWithHref(childWordIndex, parentWordIndex))
				{
					return true;
				}
			}
			else if (this.ChildScanner.Words[childWordIndex].WordLength == 1 && this.ChildScanner.Words[childWordIndex].GetWordChar(0) == '>')
			{
				int num8 = childWordIndex + num;
				if (this.MatchWord(ref num8, ref parentWordIndex, forwardMatch, recursionLevel + 1))
				{
					childWordIndex = num8;
					return true;
				}
			}
			else if (BodyDiffer.IsListNumberOrBullet(this.ChildScanner.Words[childWordIndex]))
			{
				int num9 = childWordIndex + num;
				if (this.MatchWord(ref num9, ref parentWordIndex, forwardMatch, recursionLevel + 1))
				{
					childWordIndex = num9;
					return true;
				}
			}
			else if (BodyDiffer.IsListNumberOrBullet(this.ParentScanner.Words[parentWordIndex]))
			{
				int num10 = parentWordIndex + num;
				if (this.MatchWord(ref childWordIndex, ref num10, forwardMatch, recursionLevel + 1))
				{
					parentWordIndex = num10;
					return true;
				}
			}
			return false;
		}

		private void Diff()
		{
			BodyFragmentInfo bodyFragmentInfo;
			this.childBodyFragment.ExtractNestedBodyParts(this.parentBodyFragment.BodyTag, out bodyFragmentInfo, out this.uniqueBodyPart, out this.disclaimerPart);
			if (bodyFragmentInfo != null)
			{
				return;
			}
			this.uniqueBodyPart = FragmentInfo.Empty;
			this.disclaimerPart = FragmentInfo.Empty;
			this.WordByWordDiff();
		}

		private void WordByWordDiff()
		{
			if (this.parentBodyFragment.StartWordIndex == this.parentBodyFragment.EndWordIndex)
			{
				this.uniqueBodyPart = this.childBodyFragment.Trim();
				return;
			}
			if (this.childBodyFragment.IsEmpty)
			{
				return;
			}
			int lastNormalFragmentIndex = BodyDiffer.GetLastNormalFragmentIndex(this.parentBodyFragment, this.parentBodyFragment.EndWordIndex);
			int lastNormalFragmentIndex2 = BodyDiffer.GetLastNormalFragmentIndex(this.childBodyFragment, this.childBodyFragment.EndWordIndex);
			int num = this.childBodyFragment.StartWordIndex;
			int num2 = this.parentBodyFragment.StartWordIndex;
			int disclaimerWordStart = this.childBodyFragment.EndWordIndex;
			if (lastNormalFragmentIndex < this.ParentScanner.Fragments.Count && lastNormalFragmentIndex2 < this.ChildScanner.Fragments.Count && this.ParentScanner.Fragments[lastNormalFragmentIndex].FirstWord < this.parentBodyFragment.EndWordIndex && this.ChildScanner.Fragments[lastNormalFragmentIndex2].FirstWord < this.childBodyFragment.EndWordIndex)
			{
				num = this.ChildScanner.Fragments[lastNormalFragmentIndex2].FirstWord;
				num2 = this.ParentScanner.Fragments[lastNormalFragmentIndex].FirstWord;
				while (num < this.childBodyFragment.EndWordIndex && num2 < this.parentBodyFragment.EndWordIndex && this.MatchWord(ref num, ref num2, true, 0))
				{
					num++;
					num2++;
				}
			}
			if (num2 != this.parentBodyFragment.EndWordIndex)
			{
				num = this.childBodyFragment.EndWordIndex - 1;
				num2 = this.parentBodyFragment.EndWordIndex - 1;
			}
			else
			{
				disclaimerWordStart = num;
				num = this.ChildScanner.Fragments[lastNormalFragmentIndex2].FirstWord - 1;
				num2 = this.ParentScanner.Fragments[lastNormalFragmentIndex].FirstWord - 1;
			}
			while (num >= this.childBodyFragment.StartWordIndex && num2 >= this.parentBodyFragment.StartWordIndex && this.MatchWord(ref num, ref num2, false, 0))
			{
				num--;
				num2--;
			}
			this.InitializeUniquePart(num, num2 < 0);
			this.InitializeDisclaimerPart(disclaimerWordStart);
			if (this.uniqueBodyPart.IsEmpty && !this.disclaimerPart.IsEmpty)
			{
				this.uniqueBodyPart = this.disclaimerPart;
				this.disclaimerPart = FragmentInfo.Empty;
			}
		}

		private void InitializeUniquePart(int lastUniqueWordIndex, bool fullParentMatched)
		{
			this.lastUniqueWord = ((lastUniqueWordIndex < this.childBodyFragment.EndWordIndex) ? lastUniqueWordIndex : (this.childBodyFragment.EndWordIndex - 1));
			int lastNormalFragmentIndex = BodyDiffer.GetLastNormalFragmentIndex(this.childBodyFragment, lastUniqueWordIndex);
			int num = (this.ChildScanner.Fragments.Count > lastNormalFragmentIndex + 1) ? ((int)this.ChildScanner.Fragments[lastNormalFragmentIndex + 1].FirstLine) : this.childBodyFragment.EndLineIndex;
			num = ((num >= this.childBodyFragment.EndLineIndex) ? this.childBodyFragment.EndLineIndex : num);
			int startLineIndex = this.childBodyFragment.StartLineIndex;
			FragmentInfo.TrimBoundary(this.ChildScanner, ref startLineIndex, ref num);
			if (fullParentMatched)
			{
				for (int i = this.childBodyFragment.StartLineIndex; i < num; i++)
				{
					if ((ulong)this.ChildScanner.Lines[i].FirstWordIndex > (ulong)((long)this.lastUniqueWord))
					{
						num = i;
						FragmentInfo.TrimBoundary(this.ChildScanner, ref startLineIndex, ref num);
						if (num > 0 && this.ChildScanner.Lines[num - 1].Category == ConversationBodyScanner.Scanner.LineCategory.PotentialNonMsHeader)
						{
							num--;
						}
					}
				}
			}
			this.uniqueBodyPart = new FragmentInfo(this.ChildScanner, startLineIndex, num);
		}

		private void InitializeDisclaimerPart(int disclaimerWordStart)
		{
			int num = this.childBodyFragment.EndLineIndex;
			while (num > this.UniqueBodyPart.EndLineIndex && (long)disclaimerWordStart <= (long)((ulong)this.ChildScanner.Lines[num - 1].FirstWordIndex))
			{
				num--;
			}
			int endLineIndex = this.childBodyFragment.EndLineIndex;
			FragmentInfo.TrimBoundary(this.ChildScanner, ref num, ref endLineIndex);
			this.disclaimerPart = new FragmentInfo(this.ChildScanner, num, endLineIndex);
		}

		private const int MaxMatchWordRecursionLevel = 5;

		private const string BeginHrefTag = "<<";

		private const string EndHrefTag = ">>";

		private const char QuoteChar = '>';

		private static readonly Dictionary<char, char> SameCharacters = BodyDiffer.SameCharactersCollection();

		private BodyFragmentInfo childBodyFragment;

		private BodyFragmentInfo parentBodyFragment;

		private FragmentInfo uniqueBodyPart = FragmentInfo.Empty;

		private FragmentInfo disclaimerPart = FragmentInfo.Empty;

		private int lastUniqueWord;
	}
}
