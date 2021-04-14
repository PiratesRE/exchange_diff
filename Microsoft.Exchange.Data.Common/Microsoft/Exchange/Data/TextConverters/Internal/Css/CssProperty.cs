using System;
using System.Diagnostics;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
	internal struct CssProperty
	{
		internal CssProperty(CssToken token)
		{
			this.token = token;
		}

		public int Index
		{
			get
			{
				return this.token.CurrentProperty;
			}
		}

		public bool IsCompleteProperty
		{
			get
			{
				return this.token.PropertyList[this.token.CurrentProperty].IsCompleteProperty;
			}
		}

		public bool IsPropertyBegin
		{
			get
			{
				return this.token.PropertyList[this.token.CurrentProperty].IsPropertyBegin;
			}
		}

		public bool IsPropertyEnd
		{
			get
			{
				return this.token.PropertyList[this.token.CurrentProperty].IsPropertyEnd;
			}
		}

		public bool IsPropertyNameEnd
		{
			get
			{
				return this.token.PropertyList[this.token.CurrentProperty].IsPropertyNameEnd;
			}
		}

		public bool IsDeleted
		{
			get
			{
				return this.token.PropertyList[this.token.CurrentProperty].IsPropertyDeleted;
			}
		}

		public bool IsPropertyValueQuoted
		{
			get
			{
				return this.token.PropertyList[this.token.CurrentProperty].IsPropertyValueQuoted;
			}
		}

		public CssNameIndex NameId
		{
			get
			{
				return this.token.PropertyList[this.token.CurrentProperty].NameId;
			}
		}

		public char QuoteChar
		{
			get
			{
				return (char)this.token.PropertyList[this.token.CurrentProperty].QuoteChar;
			}
		}

		public bool HasNameFragment
		{
			get
			{
				return !this.token.PropertyList[this.token.CurrentProperty].Name.IsEmpty;
			}
		}

		public CssToken.PropertyNameTextReader Name
		{
			get
			{
				return new CssToken.PropertyNameTextReader(this.token);
			}
		}

		public bool HasValueFragment
		{
			get
			{
				return !this.token.PropertyList[this.token.CurrentProperty].Value.IsEmpty;
			}
		}

		public CssToken.PropertyValueTextReader Value
		{
			get
			{
				return new CssToken.PropertyValueTextReader(this.token);
			}
		}

		public void SetMinorPart(CssToken.PropertyPartMinor newMinorPart)
		{
			this.token.PropertyList[this.token.CurrentProperty].MinorPart = newMinorPart;
		}

		[Conditional("DEBUG")]
		private void AssertCurrent()
		{
		}

		private CssToken token;
	}
}
