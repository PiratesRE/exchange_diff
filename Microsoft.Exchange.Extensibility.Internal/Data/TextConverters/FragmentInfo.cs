using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class FragmentInfo
	{
		internal FragmentInfo(ConversationBodyScanner bodyScanner, int startLineIndex, int endLineIndex)
		{
			this.bodyScanner = bodyScanner;
			this.startLineIndex = startLineIndex;
			this.endLineIndex = endLineIndex;
			if (bodyScanner == null)
			{
				this.startWordIndex = (this.endWordIndex = 0);
				return;
			}
			this.startWordIndex = this.GetFirstWordIndex(this.StartLineIndex);
			this.endWordIndex = this.GetFirstWordIndex(this.EndLineIndex);
		}

		internal FragmentInfo(ConversationBodyScanner bodyScanner)
		{
			this.startLineIndex = 0;
			this.startWordIndex = 0;
			this.endLineIndex = bodyScanner.Lines.Count;
			this.endWordIndex = bodyScanner.Words.Count;
			this.bodyScanner = bodyScanner;
		}

		public int StartLineIndex
		{
			get
			{
				return this.startLineIndex;
			}
		}

		public int EndLineIndex
		{
			get
			{
				return this.endLineIndex;
			}
		}

		public int StartWordIndex
		{
			get
			{
				return this.startWordIndex;
			}
		}

		public int EndWordIndex
		{
			get
			{
				return this.endWordIndex;
			}
		}

		public ConversationBodyScanner BodyScanner
		{
			get
			{
				return this.bodyScanner;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.StartLineIndex >= this.EndLineIndex;
			}
		}

		public FragmentInfo FragmentWithoutQuotedText
		{
			get
			{
				this.SeparateQuotedTextFragment();
				return this.fragmentWithoutQuotedText;
			}
		}

		public FragmentInfo QuotedTextFragment
		{
			get
			{
				this.SeparateQuotedTextFragment();
				return this.quotedTextFragment;
			}
		}

		public string GetSummaryText()
		{
			if (this.IsEmpty)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(255);
			int i = this.GetFirstWordIndex(this.StartLineIndex);
			int firstWordIndex = this.GetFirstWordIndex(this.EndLineIndex);
			int num = this.StartLineIndex;
			while (i < firstWordIndex)
			{
				if (i == this.GetFirstWordIndex(num))
				{
					while (i < firstWordIndex && this.BodyScanner.Words[i].WordLength == 1)
					{
						if (this.BodyScanner.Words[i].GetWordChar(0) != '>')
						{
							break;
						}
						i++;
					}
				}
				while (num < this.EndLineIndex - 1 && this.GetFirstWordIndex(num) <= i)
				{
					num++;
				}
				if (i >= this.bodyScanner.Words.Count)
				{
					break;
				}
				stringBuilder.Append(this.bodyScanner.Words[i].ToString());
				if (stringBuilder.Length > 255)
				{
					break;
				}
				stringBuilder.Append(' ');
				i++;
			}
			return stringBuilder.ToString();
		}

		private void SeparateQuotedTextFragment()
		{
			if (this.quotedTextSeparated)
			{
				return;
			}
			this.quotedTextSeparated = true;
			this.fragmentWithoutQuotedText = this;
			this.quotedTextFragment = FragmentInfo.Empty;
			if (this.IsEmpty)
			{
				return;
			}
			if (this.BodyScanner == null)
			{
				return;
			}
			foreach (ConversationBodyScanner.Scanner.FragmentInfo fragmentInfo in this.BodyScanner.Fragments)
			{
				if ((int)fragmentInfo.FirstLine >= this.StartLineIndex)
				{
					if ((int)fragmentInfo.FirstLine > this.EndLineIndex)
					{
						break;
					}
					if (fragmentInfo.Category == ConversationBodyScanner.Scanner.FragmentCategory.MsHeader || fragmentInfo.Category == ConversationBodyScanner.Scanner.FragmentCategory.NonMsHeader)
					{
						this.fragmentWithoutQuotedText = new FragmentInfo(this.bodyScanner, this.StartLineIndex, (int)fragmentInfo.FirstLine);
						this.quotedTextFragment = new FragmentInfo(this.bodyScanner, (int)fragmentInfo.FirstLine, this.EndLineIndex);
						break;
					}
				}
			}
		}

		public void WriteHtml(HtmlWriter streamWriter)
		{
			if (!this.IsEmpty)
			{
				this.bodyScanner.WriteLines(streamWriter, this.startLineIndex, this.endLineIndex - 1);
			}
		}

		public FragmentInfo Trim()
		{
			int num = this.StartLineIndex;
			int num2 = this.EndLineIndex;
			FragmentInfo.TrimBoundary(this.bodyScanner, ref num, ref num2);
			if (num != this.StartLineIndex || num2 != this.EndLineIndex)
			{
				return new FragmentInfo(this.BodyScanner, num, num2);
			}
			return this;
		}

		public bool IsMatchFound(IList<string> words)
		{
			if (words == null || words.Count < 1)
			{
				return false;
			}
			if (this.IsEmpty)
			{
				return false;
			}
			int firstWordIndex = this.GetFirstWordIndex(this.StartLineIndex);
			int firstWordIndex2 = this.GetFirstWordIndex(this.EndLineIndex);
			while (firstWordIndex < firstWordIndex2)
			{
				if ((from c in words
				where string.Compare(this.bodyScanner.Words[firstWordIndex].ToString(), c, StringComparison.CurrentCultureIgnoreCase) == 0
				select c).Any<string>())
				{
					return true;
				}
				firstWordIndex++;
			}
			return false;
		}

		protected int GetFirstWordIndex(int lineIndex)
		{
			if (lineIndex >= this.BodyScanner.Lines.Count)
			{
				return this.BodyScanner.Words.Count;
			}
			return (int)this.BodyScanner.Lines[lineIndex].FirstWordIndex;
		}

		internal static void TrimBoundary(ConversationBodyScanner bodyScanner, ref int startLineIndex, ref int endLineIndex)
		{
			while (endLineIndex > startLineIndex)
			{
				if (!FragmentInfo.IsBlankLine(bodyScanner, startLineIndex))
				{
					break;
				}
				startLineIndex++;
			}
			while (endLineIndex > startLineIndex && FragmentInfo.IsBlankLine(bodyScanner, endLineIndex - 1))
			{
				endLineIndex--;
			}
		}

		internal static bool IsBlankLine(ConversationBodyScanner bodyScanner, int lineIndex)
		{
			if (lineIndex < 0 || lineIndex >= bodyScanner.Lines.Count)
			{
				return false;
			}
			ConversationBodyScanner.Scanner.LineCategory category = bodyScanner.Lines[lineIndex].Category;
			return category == ConversationBodyScanner.Scanner.LineCategory.Blank || category == ConversationBodyScanner.Scanner.LineCategory.HorizontalLineDelimiter || category == ConversationBodyScanner.Scanner.LineCategory.Skipped;
		}

		private readonly ConversationBodyScanner bodyScanner;

		private readonly int startLineIndex;

		private readonly int startWordIndex;

		private readonly int endLineIndex;

		private readonly int endWordIndex;

		private FragmentInfo fragmentWithoutQuotedText;

		private FragmentInfo quotedTextFragment;

		private bool quotedTextSeparated;

		public static readonly FragmentInfo Empty = new FragmentInfo(null, 0, 0);
	}
}
