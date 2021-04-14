using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class TextFilter : ContentFilter
	{
		public TextFilter(PropertyDefinition property, string text, MatchOptions matchOptions, MatchFlags matchFlags) : base(property, matchOptions, matchFlags)
		{
			this.text = text;
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public override SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property)
		{
			base.CheckClonable(property);
			return new TextFilter(property, this.text, base.MatchOptions, base.MatchFlags);
		}

		public override bool Equals(object obj)
		{
			TextFilter textFilter = obj as TextFilter;
			return textFilter != null && base.MatchFlags == textFilter.MatchFlags && base.MatchOptions == textFilter.MatchOptions && this.text == textFilter.text && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.text.GetHashCode();
		}

		public override string PropertyName
		{
			get
			{
				if (base.Property != null)
				{
					return QueryFilter.ConvertPropertyName(base.Property.Name);
				}
				return base.PropertyName;
			}
		}

		public override IEnumerable<string> Keywords()
		{
			return new string[]
			{
				this.StringValue
			};
		}

		protected override string StringValue
		{
			get
			{
				if (this.text != null)
				{
					return this.text;
				}
				return "<null>";
			}
		}

		private readonly string text;
	}
}
