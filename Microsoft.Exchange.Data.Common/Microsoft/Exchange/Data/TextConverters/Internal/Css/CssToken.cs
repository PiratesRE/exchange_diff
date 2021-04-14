using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
	internal class CssToken : Token
	{
		public CssToken()
		{
			this.Reset();
		}

		public CssTokenId CssTokenId
		{
			get
			{
				return (CssTokenId)base.TokenId;
			}
		}

		public CssToken.PropertyListPartMajor MajorPart
		{
			get
			{
				return this.PartMajor;
			}
		}

		public CssToken.PropertyListPartMinor MinorPart
		{
			get
			{
				return this.PartMinor;
			}
		}

		public bool IsPropertyListBegin
		{
			get
			{
				return (byte)(this.PartMajor & CssToken.PropertyListPartMajor.Begin) == 3;
			}
		}

		public bool IsPropertyListEnd
		{
			get
			{
				return (byte)(this.PartMajor & CssToken.PropertyListPartMajor.End) == 6;
			}
		}

		public CssToken.PropertyEnumerator Properties
		{
			get
			{
				return new CssToken.PropertyEnumerator(this);
			}
		}

		public CssToken.SelectorEnumerator Selectors
		{
			get
			{
				return new CssToken.SelectorEnumerator(this);
			}
		}

		internal static bool AttemptUnescape(char[] parseBuffer, int parseEnd, ref char ch, ref int parseCurrent)
		{
			if (ch != '\\' || parseCurrent == parseEnd)
			{
				return false;
			}
			ch = parseBuffer[++parseCurrent];
			CharClass charClass = ParseSupport.GetCharClass(ch);
			int num = parseCurrent + 6;
			num = ((num < parseEnd) ? num : parseEnd);
			if (ParseSupport.HexCharacter(charClass))
			{
				int num2 = 0;
				do
				{
					num2 <<= 4;
					num2 |= ParseSupport.CharToHex(ch);
					if (parseCurrent == num)
					{
						goto IL_C3;
					}
					ch = parseBuffer[++parseCurrent];
					charClass = ParseSupport.GetCharClass(ch);
				}
				while (ParseSupport.HexCharacter(charClass));
				if (ch == '\r' && parseCurrent != parseEnd)
				{
					ch = parseBuffer[++parseCurrent];
					if (ch == '\n')
					{
						charClass = ParseSupport.GetCharClass(ch);
					}
					else
					{
						parseCurrent--;
					}
				}
				if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n' && ch != '\f')
				{
					parseCurrent--;
				}
				IL_C3:
				ch = (char)num2;
				return true;
			}
			if (ch >= ' ' && ch != '\u007f')
			{
				return true;
			}
			parseCurrent--;
			ch = '\\';
			return false;
		}

		internal new void Reset()
		{
			this.PartMajor = CssToken.PropertyListPartMajor.None;
			this.PartMinor = CssToken.PropertyListPartMinor.Empty;
			this.PropertyHead = (this.PropertyTail = 0);
			this.CurrentProperty = -1;
			this.SelectorHead = (this.SelectorTail = 0);
			this.CurrentSelector = -1;
		}

		protected internal void WriteEscapedOriginalTo(ref Token.Fragment fragment, ITextSink sink)
		{
			int num = fragment.Head;
			if (num != fragment.Tail)
			{
				int num2 = fragment.HeadOffset;
				do
				{
					Token.RunEntry runEntry = this.RunList[num];
					if (runEntry.Type == (RunType)2147483648U || runEntry.Type == (RunType)3221225472U)
					{
						if (runEntry.Kind == 184549376U && this.Buffer[num2] == '/')
						{
							string text = "/**/";
							for (int i = 0; i < text.Length; i++)
							{
								sink.Write((int)text[i]);
							}
						}
						else
						{
							this.EscapeAndWriteBuffer(this.Buffer, num2, runEntry.Length, sink);
						}
					}
					num2 += runEntry.Length;
				}
				while (++num != fragment.Tail && !sink.IsEnough);
			}
		}

		private void EscapeAndWriteBuffer(char[] buffer, int offset, int length, ITextSink sink)
		{
			int num = offset;
			int i = offset;
			while (i < offset + length)
			{
				char c = buffer[i];
				if (c == '>' || c == '<')
				{
					if (i - num > 0)
					{
						sink.Write(buffer, num, i - num);
					}
					uint num2 = (uint)c;
					char[] array = new char[]
					{
						'\\',
						'\0',
						'\0',
						' '
					};
					for (int j = 2; j > 0; j--)
					{
						uint num3 = num2 & 15U;
						array[j] = (char)((ulong)num3 + (ulong)((num3 < 10U) ? 48L : 55L));
						num2 >>= 4;
					}
					sink.Write(array, 0, 4);
					i = (num = i + 1);
				}
				else
				{
					CssToken.AttemptUnescape(buffer, offset + length, ref c, ref i);
					i++;
				}
			}
			sink.Write(buffer, num, length - (num - offset));
		}

		protected internal CssToken.PropertyListPartMajor PartMajor;

		protected internal CssToken.PropertyListPartMinor PartMinor;

		protected internal CssToken.PropertyEntry[] PropertyList;

		protected internal int PropertyHead;

		protected internal int PropertyTail;

		protected internal int CurrentProperty;

		protected internal Token.FragmentPosition PropertyNamePosition;

		protected internal Token.FragmentPosition PropertyValuePosition;

		protected internal CssToken.SelectorEntry[] SelectorList;

		protected internal int SelectorHead;

		protected internal int SelectorTail;

		protected internal int CurrentSelector;

		protected internal Token.FragmentPosition SelectorNamePosition;

		protected internal Token.FragmentPosition SelectorClassPosition;

		public enum PropertyListPartMajor : byte
		{
			None,
			Begin = 3,
			Continue = 2,
			End = 6,
			Complete
		}

		public enum PropertyListPartMinor : byte
		{
			Empty,
			BeginProperty = 24,
			ContinueProperty = 16,
			EndProperty = 48,
			EndPropertyWithOtherProperties = 176,
			PropertyPartMask = 56,
			Properties = 128
		}

		public enum PropertyPartMajor : byte
		{
			None,
			Begin = 3,
			Continue = 2,
			End = 6,
			Complete,
			ValueQuoted = 64,
			Deleted = 128,
			MaskOffFlags = 7
		}

		public enum PropertyPartMinor : byte
		{
			Empty,
			BeginName = 3,
			ContinueName = 2,
			EndName = 6,
			EndNameWithBeginValue = 30,
			EndNameWithCompleteValue = 62,
			CompleteName = 7,
			CompleteNameWithBeginValue = 31,
			CompleteNameWithCompleteValue = 63,
			BeginValue = 24,
			ContinueValue = 16,
			EndValue = 48,
			CompleteValue = 56
		}

		public struct PropertyEnumerator
		{
			internal PropertyEnumerator(CssToken token)
			{
				this.token = token;
			}

			public int Count
			{
				get
				{
					return this.token.PropertyTail - this.token.PropertyHead;
				}
			}

			public int ValidCount
			{
				get
				{
					int num = 0;
					for (int i = this.token.PropertyHead; i < this.token.PropertyTail; i++)
					{
						if (!this.token.PropertyList[i].IsPropertyDeleted)
						{
							num++;
						}
					}
					return num;
				}
			}

			public CssProperty Current
			{
				get
				{
					return new CssProperty(this.token);
				}
			}

			public int CurrentIndex
			{
				get
				{
					return this.token.CurrentProperty;
				}
			}

			public CssProperty this[int i]
			{
				get
				{
					this.token.CurrentProperty = i;
					this.token.PropertyNamePosition.Rewind(this.token.PropertyList[i].Name);
					this.token.PropertyValuePosition.Rewind(this.token.PropertyList[i].Value);
					return new CssProperty(this.token);
				}
			}

			public bool MoveNext()
			{
				if (this.token.CurrentProperty != this.token.PropertyTail)
				{
					this.token.CurrentProperty++;
					if (this.token.CurrentProperty != this.token.PropertyTail)
					{
						this.token.PropertyNamePosition.Rewind(this.token.PropertyList[this.token.CurrentProperty].Name);
						this.token.PropertyValuePosition.Rewind(this.token.PropertyList[this.token.CurrentProperty].Value);
					}
				}
				return this.token.CurrentProperty != this.token.PropertyTail;
			}

			public void Rewind()
			{
				this.token.CurrentProperty = this.token.PropertyHead - 1;
			}

			public CssToken.PropertyEnumerator GetEnumerator()
			{
				return this;
			}

			public bool Find(CssNameIndex nameId)
			{
				for (int i = this.token.PropertyHead; i < this.token.PropertyTail; i++)
				{
					if (this.token.PropertyList[i].NameId == nameId)
					{
						this.token.CurrentProperty = i;
						this.token.PropertyNamePosition.Rewind(this.token.PropertyList[i].Name);
						this.token.PropertyValuePosition.Rewind(this.token.PropertyList[i].Value);
						return true;
					}
				}
				return false;
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private CssToken token;
		}

		public struct PropertyNameTextReader
		{
			internal PropertyNameTextReader(CssToken token)
			{
				this.token = token;
			}

			public int Length
			{
				get
				{
					return this.token.GetLength(ref this.token.PropertyList[this.token.CurrentProperty].Name);
				}
			}

			public int Read(char[] buffer, int offset, int count)
			{
				return this.token.Read(ref this.token.PropertyList[this.token.CurrentProperty].Name, ref this.token.PropertyNamePosition, buffer, offset, count);
			}

			public void Rewind()
			{
				this.token.PropertyNamePosition.Rewind(this.token.PropertyList[this.token.CurrentProperty].Name);
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(ref this.token.PropertyList[this.token.CurrentProperty].Name, sink);
			}

			public void WriteOriginalTo(ITextSink sink)
			{
				this.token.WriteOriginalTo(ref this.token.PropertyList[this.token.CurrentProperty].Name, sink);
			}

			public string GetString(int maxSize)
			{
				return this.token.GetString(ref this.token.PropertyList[this.token.CurrentProperty].Name, maxSize);
			}

			public void MakeEmpty()
			{
				this.token.PropertyList[this.token.CurrentProperty].Name.Reset();
				this.Rewind();
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private CssToken token;
		}

		public struct PropertyValueTextReader
		{
			internal PropertyValueTextReader(CssToken token)
			{
				this.token = token;
			}

			public int Length
			{
				get
				{
					return this.token.GetLength(ref this.token.PropertyList[this.token.CurrentProperty].Value);
				}
			}

			public bool IsEmpty
			{
				get
				{
					return this.token.IsFragmentEmpty(ref this.token.PropertyList[this.token.CurrentProperty].Value);
				}
			}

			public bool IsContiguous
			{
				get
				{
					return this.token.IsContiguous(ref this.token.PropertyList[this.token.CurrentProperty].Value);
				}
			}

			public BufferString ContiguousBufferString
			{
				get
				{
					return new BufferString(this.token.Buffer, this.token.PropertyList[this.token.CurrentProperty].Value.HeadOffset, this.token.RunList[this.token.PropertyList[this.token.CurrentProperty].Value.Head].Length);
				}
			}

			public int Read(char[] buffer, int offset, int count)
			{
				return this.token.Read(ref this.token.PropertyList[this.token.CurrentProperty].Value, ref this.token.PropertyValuePosition, buffer, offset, count);
			}

			public void Rewind()
			{
				this.token.PropertyValuePosition.Rewind(this.token.PropertyList[this.token.CurrentProperty].Value);
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(ref this.token.PropertyList[this.token.CurrentProperty].Value, sink);
			}

			public void WriteOriginalTo(ITextSink sink)
			{
				this.token.WriteOriginalTo(ref this.token.PropertyList[this.token.CurrentProperty].Value, sink);
			}

			public void WriteEscapedOriginalTo(ITextSink sink)
			{
				this.token.WriteEscapedOriginalTo(ref this.token.PropertyList[this.token.CurrentProperty].Value, sink);
			}

			public string GetString(int maxSize)
			{
				return this.token.GetString(ref this.token.PropertyList[this.token.CurrentProperty].Value, maxSize);
			}

			public bool CaseInsensitiveCompareEqual(string str)
			{
				return this.token.CaseInsensitiveCompareEqual(ref this.token.PropertyList[this.token.CurrentProperty].Value, str);
			}

			public bool CaseInsensitiveContainsSubstring(string str)
			{
				return this.token.CaseInsensitiveContainsSubstring(ref this.token.PropertyList[this.token.CurrentProperty].Value, str);
			}

			public void MakeEmpty()
			{
				this.token.PropertyList[this.token.CurrentProperty].Value.Reset();
				this.Rewind();
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private CssToken token;
		}

		public struct SelectorEnumerator
		{
			internal SelectorEnumerator(CssToken token)
			{
				this.token = token;
			}

			public int Count
			{
				get
				{
					return this.token.SelectorTail - this.token.SelectorHead;
				}
			}

			public int ValidCount
			{
				get
				{
					int num = 0;
					for (int i = this.token.SelectorHead; i < this.token.SelectorTail; i++)
					{
						if (!this.token.SelectorList[i].IsSelectorDeleted)
						{
							num++;
						}
					}
					return num;
				}
			}

			public CssSelector Current
			{
				get
				{
					return new CssSelector(this.token);
				}
			}

			public int CurrentIndex
			{
				get
				{
					return this.token.CurrentSelector;
				}
			}

			public CssSelector this[int i]
			{
				get
				{
					this.token.CurrentSelector = i;
					this.token.SelectorNamePosition.Rewind(this.token.SelectorList[i].Name);
					this.token.SelectorClassPosition.Rewind(this.token.SelectorList[i].ClassName);
					return new CssSelector(this.token);
				}
			}

			public bool MoveNext()
			{
				if (this.token.CurrentSelector != this.token.SelectorTail)
				{
					this.token.CurrentSelector++;
					if (this.token.CurrentSelector != this.token.SelectorTail)
					{
						this.token.SelectorNamePosition.Rewind(this.token.SelectorList[this.token.CurrentSelector].Name);
						this.token.SelectorClassPosition.Rewind(this.token.SelectorList[this.token.CurrentSelector].ClassName);
					}
				}
				return this.token.CurrentSelector != this.token.SelectorTail;
			}

			public void Rewind()
			{
				this.token.CurrentSelector = this.token.SelectorHead - 1;
			}

			public CssToken.SelectorEnumerator GetEnumerator()
			{
				return this;
			}

			public bool Find(HtmlNameIndex nameId)
			{
				for (int i = this.token.SelectorHead; i < this.token.SelectorTail; i++)
				{
					if (this.token.SelectorList[i].NameId == nameId)
					{
						this.token.CurrentSelector = i;
						this.token.SelectorNamePosition.Rewind(this.token.SelectorList[i].Name);
						this.token.SelectorClassPosition.Rewind(this.token.SelectorList[i].ClassName);
						return true;
					}
				}
				return false;
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private CssToken token;
		}

		public struct SelectorNameTextReader
		{
			internal SelectorNameTextReader(CssToken token)
			{
				this.token = token;
			}

			public int Length
			{
				get
				{
					return this.token.GetLength(ref this.token.SelectorList[this.token.CurrentSelector].Name);
				}
			}

			public int Read(char[] buffer, int offset, int count)
			{
				return this.token.Read(ref this.token.SelectorList[this.token.CurrentSelector].Name, ref this.token.SelectorNamePosition, buffer, offset, count);
			}

			public void Rewind()
			{
				this.token.SelectorNamePosition.Rewind(this.token.SelectorList[this.token.CurrentSelector].Name);
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(ref this.token.SelectorList[this.token.CurrentSelector].Name, sink);
			}

			public void WriteOriginalTo(ITextSink sink)
			{
				this.token.WriteOriginalTo(ref this.token.SelectorList[this.token.CurrentSelector].Name, sink);
			}

			public string GetString(int maxSize)
			{
				return this.token.GetString(ref this.token.SelectorList[this.token.CurrentSelector].Name, maxSize);
			}

			public void MakeEmpty()
			{
				this.token.SelectorList[this.token.CurrentSelector].Name.Reset();
				this.Rewind();
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private CssToken token;
		}

		public struct SelectorClassTextReader
		{
			internal SelectorClassTextReader(CssToken token)
			{
				this.token = token;
			}

			public int Length
			{
				get
				{
					return this.token.GetLength(ref this.token.SelectorList[this.token.CurrentSelector].ClassName);
				}
			}

			public int Read(char[] buffer, int offset, int count)
			{
				return this.token.Read(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, ref this.token.SelectorClassPosition, buffer, offset, count);
			}

			public void Rewind()
			{
				this.token.SelectorClassPosition.Rewind(this.token.SelectorList[this.token.CurrentSelector].ClassName);
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, sink);
			}

			public void WriteOriginalTo(ITextSink sink)
			{
				this.token.WriteEscapedOriginalTo(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, sink);
			}

			public string GetString(int maxSize)
			{
				return this.token.GetString(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, maxSize);
			}

			public bool CaseInsensitiveCompareEqual(string str)
			{
				return this.token.CaseInsensitiveCompareEqual(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, str);
			}

			public bool CaseInsensitiveContainsSubstring(string str)
			{
				return this.token.CaseInsensitiveContainsSubstring(ref this.token.SelectorList[this.token.CurrentSelector].ClassName, str);
			}

			public void MakeEmpty()
			{
				this.token.SelectorList[this.token.CurrentSelector].ClassName.Reset();
				this.Rewind();
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private CssToken token;
		}

		protected internal struct PropertyEntry
		{
			public bool IsCompleteProperty
			{
				get
				{
					return this.MajorPart == CssToken.PropertyPartMajor.Complete;
				}
			}

			public bool IsPropertyBegin
			{
				get
				{
					return (byte)(this.PartMajor & CssToken.PropertyPartMajor.Begin) == 3;
				}
			}

			public bool IsPropertyEnd
			{
				get
				{
					return (byte)(this.PartMajor & CssToken.PropertyPartMajor.End) == 6;
				}
			}

			public bool IsPropertyNameEnd
			{
				get
				{
					return (byte)(this.PartMinor & CssToken.PropertyPartMinor.EndName) == 6;
				}
			}

			public bool IsPropertyValueBegin
			{
				get
				{
					return (byte)(this.PartMinor & CssToken.PropertyPartMinor.BeginValue) == 24;
				}
			}

			public CssToken.PropertyPartMajor MajorPart
			{
				get
				{
					return this.PartMajor & CssToken.PropertyPartMajor.Complete;
				}
			}

			public CssToken.PropertyPartMinor MinorPart
			{
				get
				{
					return this.PartMinor;
				}
				set
				{
					this.PartMinor = value;
				}
			}

			public bool IsPropertyValueQuoted
			{
				get
				{
					return (byte)(this.PartMajor & CssToken.PropertyPartMajor.ValueQuoted) == 64;
				}
				set
				{
					this.PartMajor = (value ? (this.PartMajor | CssToken.PropertyPartMajor.ValueQuoted) : (this.PartMajor & (CssToken.PropertyPartMajor)191));
				}
			}

			public bool IsPropertyDeleted
			{
				get
				{
					return (byte)(this.PartMajor & CssToken.PropertyPartMajor.Deleted) == 128;
				}
				set
				{
					this.PartMajor = (value ? (this.PartMajor | CssToken.PropertyPartMajor.Deleted) : (this.PartMajor & (CssToken.PropertyPartMajor)127));
				}
			}

			public CssNameIndex NameId;

			public byte QuoteChar;

			public CssToken.PropertyPartMajor PartMajor;

			public CssToken.PropertyPartMinor PartMinor;

			public Token.Fragment Name;

			public Token.Fragment Value;
		}

		protected internal struct SelectorEntry
		{
			public bool IsSelectorDeleted
			{
				get
				{
					return this.deleted;
				}
				set
				{
					this.deleted = value;
				}
			}

			public HtmlNameIndex NameId;

			private bool deleted;

			public Token.Fragment Name;

			public Token.Fragment ClassName;

			public CssSelectorClassType ClassType;

			public CssSelectorCombinator Combinator;
		}
	}
}
