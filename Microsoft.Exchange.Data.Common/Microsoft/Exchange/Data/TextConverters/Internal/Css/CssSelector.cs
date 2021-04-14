using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
	internal struct CssSelector
	{
		internal CssSelector(CssToken token)
		{
			this.token = token;
		}

		public int Index
		{
			get
			{
				return this.token.CurrentSelector;
			}
		}

		public bool IsDeleted
		{
			get
			{
				return this.token.SelectorList[this.token.CurrentSelector].IsSelectorDeleted;
			}
		}

		public HtmlNameIndex NameId
		{
			get
			{
				return this.token.SelectorList[this.token.CurrentSelector].NameId;
			}
		}

		public bool HasNameFragment
		{
			get
			{
				return !this.token.SelectorList[this.token.CurrentSelector].Name.IsEmpty;
			}
		}

		public CssToken.SelectorNameTextReader Name
		{
			get
			{
				return new CssToken.SelectorNameTextReader(this.token);
			}
		}

		public bool HasClassFragment
		{
			get
			{
				return !this.token.SelectorList[this.token.CurrentSelector].ClassName.IsEmpty;
			}
		}

		public CssToken.SelectorClassTextReader ClassName
		{
			get
			{
				return new CssToken.SelectorClassTextReader(this.token);
			}
		}

		public CssSelectorClassType ClassType
		{
			get
			{
				return this.token.SelectorList[this.token.CurrentSelector].ClassType;
			}
		}

		public bool IsSimple
		{
			get
			{
				return this.token.SelectorList[this.token.CurrentSelector].Combinator == CssSelectorCombinator.None && (this.token.SelectorTail == this.token.CurrentSelector + 1 || this.token.SelectorList[this.token.CurrentSelector + 1].Combinator == CssSelectorCombinator.None);
			}
		}

		public CssSelectorCombinator Combinator
		{
			get
			{
				return this.token.SelectorList[this.token.CurrentSelector].Combinator;
			}
		}

		[Conditional("DEBUG")]
		private void AssertCurrent()
		{
		}

		private CssToken token;
	}
}
