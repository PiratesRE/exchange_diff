using System;
using System.Text;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct TextRun
	{
		internal TextRun(FormatStore.TextStore text, uint position)
		{
			this.isImmutable = false;
			this.text = text;
			this.position = position;
		}

		public uint Position
		{
			get
			{
				return this.position;
			}
		}

		public TextRunType Type
		{
			get
			{
				return FormatStore.TextStore.TypeFromRunHeader(this.text.Pick(this.position));
			}
		}

		public int EffectiveLength
		{
			get
			{
				return FormatStore.TextStore.LengthFromRunHeader(this.text.Pick(this.position));
			}
		}

		public int Length
		{
			get
			{
				char c = this.text.Pick(this.position);
				if (c < '\u3000')
				{
					return FormatStore.TextStore.LengthFromRunHeader(c) + 1;
				}
				return 1;
			}
		}

		public int WordLength
		{
			get
			{
				int num = 0;
				TextRun textRun = this;
				while (!textRun.IsEnd() && textRun.Type == TextRunType.NonSpace && num < 1024)
				{
					num += textRun.EffectiveLength;
					textRun = textRun.GetNext();
				}
				return num;
			}
		}

		private bool IsLong
		{
			get
			{
				return this.Type < TextRunType.FirstShort;
			}
		}

		public char this[int index]
		{
			get
			{
				return this.text.Plane(this.position)[this.text.Index(this.position) + 1 + index];
			}
		}

		public char GetWordChar(int index)
		{
			int effectiveLength = this.EffectiveLength;
			if (index < effectiveLength)
			{
				return this[index];
			}
			TextRun next = this.GetNext();
			index -= effectiveLength;
			while (!next.IsEnd())
			{
				effectiveLength = next.EffectiveLength;
				if (index < effectiveLength)
				{
					return next[index];
				}
				if (next.Type != TextRunType.NonSpace)
				{
					break;
				}
				index -= effectiveLength;
				next = next.GetNext();
			}
			throw new ArgumentOutOfRangeException("index");
		}

		public void MoveNext()
		{
			if (this.isImmutable)
			{
				throw new InvalidOperationException("This run is immutable");
			}
			this.position += (uint)this.Length;
		}

		public void SkipInvalid()
		{
			if (this.isImmutable)
			{
				throw new InvalidOperationException("This run is immutable");
			}
			while (!this.IsEnd() && this.Type == TextRunType.Invalid)
			{
				this.MoveNext();
			}
		}

		public bool IsEnd()
		{
			return this.position >= this.text.CurrentPosition;
		}

		public TextRun GetNext()
		{
			return new TextRun(this.text, this.position + (uint)this.Length);
		}

		public void GetChunk(int start, out char[] buffer, out int offset, out int count)
		{
			buffer = this.text.Plane(this.position);
			offset = this.text.Index(this.position) + 1 + start;
			count = this.EffectiveLength - start;
		}

		public int AppendFragment(int start, ref ScratchBuffer scratchBuffer, int maxLength)
		{
			int offset = this.text.Index(this.position) + 1 + start;
			int num = Math.Min(this.EffectiveLength - start, maxLength);
			if (num != 0)
			{
				scratchBuffer.Append(this.text.Plane(this.position), offset, num);
			}
			return num;
		}

		public void ConvertToInvalid()
		{
			this.text.ConvertToInvalid(this.position);
		}

		public void ConvertToInvalid(int count)
		{
			this.text.ConvertToInvalid(this.position, count);
		}

		public void ConvertShort(TextRunType type, int newEffectiveLength)
		{
			this.text.ConvertShortRun(this.position, type, newEffectiveLength);
		}

		public override string ToString()
		{
			int wordLength = this.WordLength;
			StringBuilder stringBuilder = new StringBuilder(wordLength);
			for (int i = 0; i < wordLength; i++)
			{
				stringBuilder.Append(this.GetWordChar(i));
			}
			return stringBuilder.ToString();
		}

		internal void MakeImmutable()
		{
			this.isImmutable = true;
		}

		public const int MaxEffectiveLength = 4095;

		public static readonly TextRun Invalid = default(TextRun);

		private FormatStore.TextStore text;

		private uint position;

		private bool isImmutable;
	}
}
