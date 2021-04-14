using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlToken : Token
	{
		public HtmlToken()
		{
			this.Reset();
		}

		public HtmlTokenId HtmlTokenId
		{
			get
			{
				return (HtmlTokenId)base.TokenId;
			}
			set
			{
				base.TokenId = (TokenId)value;
			}
		}

		public HtmlToken.TagFlags Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public bool IsEndTag
		{
			get
			{
				return 0 != (byte)(this.flags & HtmlToken.TagFlags.EndTag);
			}
		}

		public bool IsEmptyScope
		{
			get
			{
				return 0 != (byte)(this.flags & HtmlToken.TagFlags.EmptyScope);
			}
		}

		public HtmlToken.TagPartMajor MajorPart
		{
			get
			{
				return this.PartMajor;
			}
		}

		public HtmlToken.TagPartMinor MinorPart
		{
			get
			{
				return this.PartMinor;
			}
		}

		public bool IsTagComplete
		{
			get
			{
				return this.PartMajor == HtmlToken.TagPartMajor.Complete;
			}
		}

		public bool IsTagBegin
		{
			get
			{
				return (byte)(this.PartMajor & HtmlToken.TagPartMajor.Begin) == 3;
			}
		}

		public bool IsTagEnd
		{
			get
			{
				return (byte)(this.PartMajor & HtmlToken.TagPartMajor.End) == 6;
			}
		}

		public bool IsTagNameEmpty
		{
			get
			{
				return 0 != (byte)(this.flags & HtmlToken.TagFlags.EmptyTagName);
			}
		}

		public bool IsTagNameBegin
		{
			get
			{
				return (byte)(this.PartMinor & HtmlToken.TagPartMinor.BeginName) == 3;
			}
		}

		public bool IsTagNameEnd
		{
			get
			{
				return (byte)(this.PartMinor & HtmlToken.TagPartMinor.EndName) == 6;
			}
		}

		public bool HasNameFragment
		{
			get
			{
				return !base.IsFragmentEmpty(this.NameInternal);
			}
		}

		public HtmlToken.TagNameTextReader Name
		{
			get
			{
				return new HtmlToken.TagNameTextReader(this);
			}
		}

		public HtmlToken.TagUnstructuredContentTextReader UnstructuredContent
		{
			get
			{
				return new HtmlToken.TagUnstructuredContentTextReader(this);
			}
		}

		public HtmlTagIndex OriginalTagId
		{
			get
			{
				return this.OriginalTagIndex;
			}
		}

		public bool IsAllowWspLeft
		{
			get
			{
				return (byte)(this.flags & HtmlToken.TagFlags.AllowWspLeft) == 64;
			}
		}

		public bool IsAllowWspRight
		{
			get
			{
				return (byte)(this.flags & HtmlToken.TagFlags.AllowWspRight) == 128;
			}
		}

		public HtmlToken.AttributeEnumerator Attributes
		{
			get
			{
				return new HtmlToken.AttributeEnumerator(this);
			}
		}

		internal new void Reset()
		{
			this.TagIndex = (this.OriginalTagIndex = HtmlTagIndex._NULL);
			this.NameIndex = HtmlNameIndex._NOTANAME;
			this.flags = HtmlToken.TagFlags.None;
			this.PartMajor = HtmlToken.TagPartMajor.None;
			this.PartMinor = HtmlToken.TagPartMinor.Empty;
			this.NameInternal.Reset();
			this.Unstructured.Reset();
			this.NamePosition.Reset();
			this.UnstructuredPosition.Reset();
			this.AttributeTail = 0;
			this.CurrentAttribute = -1;
			this.AttrNamePosition.Reset();
			this.AttrValuePosition.Reset();
		}

		private HtmlToken.TagFlags flags;

		protected internal HtmlTagIndex TagIndex;

		protected internal HtmlTagIndex OriginalTagIndex;

		protected internal HtmlNameIndex NameIndex;

		protected internal HtmlToken.TagPartMajor PartMajor;

		protected internal HtmlToken.TagPartMinor PartMinor;

		protected internal Token.LexicalUnit Unstructured;

		protected internal Token.FragmentPosition UnstructuredPosition;

		protected internal Token.LexicalUnit NameInternal;

		protected internal Token.LexicalUnit LocalName;

		protected internal Token.FragmentPosition NamePosition;

		protected internal HtmlToken.AttributeEntry[] AttributeList;

		protected internal int AttributeTail;

		protected internal int CurrentAttribute;

		protected internal Token.FragmentPosition AttrNamePosition;

		protected internal Token.FragmentPosition AttrValuePosition;

		[Flags]
		public enum TagFlags : byte
		{
			None = 0,
			EmptyTagName = 8,
			EndTag = 16,
			EmptyScope = 32,
			AllowWspLeft = 64,
			AllowWspRight = 128
		}

		public enum TagPartMajor : byte
		{
			None,
			Begin = 3,
			Continue = 2,
			End = 6,
			Complete
		}

		public enum TagPartMinor : byte
		{
			Empty,
			BeginName = 3,
			ContinueName = 2,
			EndName = 6,
			EndNameWithAttributes = 134,
			CompleteName = 7,
			CompleteNameWithAttributes = 135,
			BeginAttribute = 24,
			ContinueAttribute = 16,
			EndAttribute = 48,
			EndAttributeWithOtherAttributes = 176,
			AttributePartMask = 56,
			Attributes = 128
		}

		public enum AttrPartMajor : byte
		{
			None,
			Begin = 24,
			Continue = 16,
			End = 48,
			Complete = 56,
			EmptyName = 1,
			ValueQuoted = 64,
			Deleted = 128,
			MaskOffFlags = 56
		}

		public enum AttrPartMinor : byte
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

		public struct AttributeEnumerator
		{
			internal AttributeEnumerator(HtmlToken token)
			{
				this.token = token;
			}

			public int Count
			{
				get
				{
					return this.token.AttributeTail;
				}
			}

			public HtmlAttribute Current
			{
				get
				{
					return new HtmlAttribute(this.token);
				}
			}

			public int CurrentIndex
			{
				get
				{
					return this.token.CurrentAttribute;
				}
			}

			public HtmlAttribute this[int i]
			{
				get
				{
					if (i != this.token.CurrentAttribute)
					{
						this.token.AttrNamePosition.Rewind(this.token.AttributeList[i].Name);
						this.token.AttrValuePosition.Rewind(this.token.AttributeList[i].Value);
					}
					this.token.CurrentAttribute = i;
					return new HtmlAttribute(this.token);
				}
			}

			public bool MoveNext()
			{
				if (this.token.CurrentAttribute != this.token.AttributeTail)
				{
					this.token.CurrentAttribute++;
					if (this.token.CurrentAttribute != this.token.AttributeTail)
					{
						this.token.AttrNamePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Name);
						this.token.AttrValuePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Value);
					}
				}
				return this.token.CurrentAttribute != this.token.AttributeTail;
			}

			public void Rewind()
			{
				this.token.CurrentAttribute = -1;
			}

			public HtmlToken.AttributeEnumerator GetEnumerator()
			{
				return this;
			}

			public bool Find(HtmlNameIndex nameIndex)
			{
				for (int i = 0; i < this.token.AttributeTail; i++)
				{
					if (this.token.AttributeList[i].NameIndex == nameIndex)
					{
						this.token.CurrentAttribute = i;
						this.token.AttrNamePosition.Rewind(this.token.AttributeList[i].Name);
						this.token.AttrValuePosition.Rewind(this.token.AttributeList[i].Value);
						return true;
					}
				}
				return false;
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private HtmlToken token;
		}

		public struct TagUnstructuredContentTextReader
		{
			internal TagUnstructuredContentTextReader(HtmlToken token)
			{
				this.token = token;
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(this.token.Unstructured, sink);
			}

			public string GetString(int maxSize)
			{
				return this.token.GetString(this.token.Unstructured, maxSize);
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private HtmlToken token;
		}

		public struct TagNameTextReader
		{
			internal TagNameTextReader(HtmlToken token)
			{
				this.token = token;
			}

			public int Length
			{
				get
				{
					return this.token.GetLength(this.token.NameInternal);
				}
			}

			public int Read(char[] buffer, int offset, int count)
			{
				return this.token.Read(this.token.NameInternal, ref this.token.NamePosition, buffer, offset, count);
			}

			public void Rewind()
			{
				this.token.NamePosition.Rewind(this.token.NameInternal);
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(this.token.NameInternal, sink);
			}

			public string GetString(int maxSize)
			{
				return this.token.GetString(this.token.NameInternal, maxSize);
			}

			public void MakeEmpty()
			{
				this.token.NameInternal.Reset();
				this.Rewind();
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private HtmlToken token;
		}

		public struct AttributeNameTextReader
		{
			internal AttributeNameTextReader(HtmlToken token)
			{
				this.token = token;
			}

			public int Length
			{
				get
				{
					return this.token.GetLength(this.token.AttributeList[this.token.CurrentAttribute].Name);
				}
			}

			public int Read(char[] buffer, int offset, int count)
			{
				return this.token.Read(this.token.AttributeList[this.token.CurrentAttribute].Name, ref this.token.AttrNamePosition, buffer, offset, count);
			}

			public void Rewind()
			{
				this.token.AttrNamePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Name);
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(this.token.AttributeList[this.token.CurrentAttribute].Name, sink);
			}

			public string GetString(int maxSize)
			{
				return this.token.GetString(this.token.AttributeList[this.token.CurrentAttribute].Name, maxSize);
			}

			public void MakeEmpty()
			{
				this.token.AttributeList[this.token.CurrentAttribute].Name.Reset();
				this.token.AttrNamePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Name);
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private HtmlToken token;
		}

		public struct AttributeValueTextReader
		{
			internal AttributeValueTextReader(HtmlToken token)
			{
				this.token = token;
			}

			public int Length
			{
				get
				{
					return this.token.GetLength(this.token.AttributeList[this.token.CurrentAttribute].Value);
				}
			}

			public bool IsEmpty
			{
				get
				{
					return this.token.IsFragmentEmpty(this.token.AttributeList[this.token.CurrentAttribute].Value);
				}
			}

			public bool IsContiguous
			{
				get
				{
					return this.token.IsContiguous(this.token.AttributeList[this.token.CurrentAttribute].Value);
				}
			}

			public BufferString ContiguousBufferString
			{
				get
				{
					return new BufferString(this.token.Buffer, this.token.AttributeList[this.token.CurrentAttribute].Value.HeadOffset, this.token.RunList[this.token.AttributeList[this.token.CurrentAttribute].Value.Head].Length);
				}
			}

			public int Read(char[] buffer, int offset, int count)
			{
				return this.token.Read(this.token.AttributeList[this.token.CurrentAttribute].Value, ref this.token.AttrValuePosition, buffer, offset, count);
			}

			public void Rewind()
			{
				this.token.AttrValuePosition.Rewind(this.token.AttributeList[this.token.CurrentAttribute].Value);
			}

			public void WriteTo(ITextSink sink)
			{
				this.token.WriteTo(this.token.AttributeList[this.token.CurrentAttribute].Value, sink);
			}

			public string GetString(int maxSize)
			{
				return this.token.GetString(this.token.AttributeList[this.token.CurrentAttribute].Value, maxSize);
			}

			public bool CaseInsensitiveCompareEqual(string str)
			{
				return this.token.CaseInsensitiveCompareEqual(this.token.AttributeList[this.token.CurrentAttribute].Value, str);
			}

			public bool CaseInsensitiveContainsSubstring(string str)
			{
				return this.token.CaseInsensitiveContainsSubstring(this.token.AttributeList[this.token.CurrentAttribute].Value, str);
			}

			public bool SkipLeadingWhitespace()
			{
				return this.token.SkipLeadingWhitespace(this.token.AttributeList[this.token.CurrentAttribute].Value, ref this.token.AttrValuePosition);
			}

			public void MakeEmpty()
			{
				this.token.AttributeList[this.token.CurrentAttribute].Value.Reset();
				this.Rewind();
			}

			[Conditional("DEBUG")]
			private void AssertCurrent()
			{
			}

			private HtmlToken token;
		}

		protected internal struct AttributeEntry
		{
			public bool IsCompleteAttr
			{
				get
				{
					return this.MajorPart == HtmlToken.AttrPartMajor.Complete;
				}
			}

			public bool IsAttrBegin
			{
				get
				{
					return (byte)(this.PartMajor & HtmlToken.AttrPartMajor.Begin) == 24;
				}
			}

			public bool IsAttrEnd
			{
				get
				{
					return (byte)(this.PartMajor & HtmlToken.AttrPartMajor.End) == 48;
				}
			}

			public bool IsAttrEmptyName
			{
				get
				{
					return (byte)(this.PartMajor & HtmlToken.AttrPartMajor.EmptyName) == 1;
				}
			}

			public bool IsAttrNameEnd
			{
				get
				{
					return (byte)(this.PartMinor & HtmlToken.AttrPartMinor.EndName) == 6;
				}
			}

			public bool IsAttrValueBegin
			{
				get
				{
					return (byte)(this.PartMinor & HtmlToken.AttrPartMinor.BeginValue) == 24;
				}
			}

			public HtmlToken.AttrPartMajor MajorPart
			{
				get
				{
					return this.PartMajor & HtmlToken.AttrPartMajor.Complete;
				}
			}

			public HtmlToken.AttrPartMinor MinorPart
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

			public bool IsAttrValueQuoted
			{
				get
				{
					return (byte)(this.PartMajor & HtmlToken.AttrPartMajor.ValueQuoted) == 64;
				}
				set
				{
					this.PartMajor = (value ? (this.PartMajor | HtmlToken.AttrPartMajor.ValueQuoted) : (this.PartMajor & (HtmlToken.AttrPartMajor)191));
				}
			}

			public bool IsAttrDeleted
			{
				get
				{
					return (byte)(this.PartMajor & HtmlToken.AttrPartMajor.Deleted) == 128;
				}
				set
				{
					this.PartMajor = (value ? (this.PartMajor | HtmlToken.AttrPartMajor.Deleted) : (this.PartMajor & (HtmlToken.AttrPartMajor)127));
				}
			}

			public HtmlNameIndex NameIndex;

			public byte QuoteChar;

			public byte DangerousCharacters;

			public HtmlToken.AttrPartMajor PartMajor;

			public HtmlToken.AttrPartMinor PartMinor;

			public Token.LexicalUnit Name;

			public Token.LexicalUnit LocalName;

			public Token.LexicalUnit Value;
		}
	}
}
