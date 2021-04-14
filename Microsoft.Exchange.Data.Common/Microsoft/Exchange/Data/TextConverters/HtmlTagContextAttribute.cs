using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters
{
	public struct HtmlTagContextAttribute
	{
		internal HtmlTagContextAttribute(HtmlTagContext tagContext, int attributeIndexAndCookie)
		{
			this.tagContext = tagContext;
			this.attributeIndexAndCookie = attributeIndexAndCookie;
		}

		public bool IsNull
		{
			get
			{
				return this.tagContext == null;
			}
		}

		public HtmlAttributeId Id
		{
			get
			{
				this.AssertValid();
				return this.tagContext.GetAttributeNameIdImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie));
			}
		}

		public string Name
		{
			get
			{
				this.AssertValid();
				return this.tagContext.GetAttributeNameImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie));
			}
		}

		public string Value
		{
			get
			{
				this.AssertValid();
				return this.tagContext.GetAttributeValueImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie));
			}
		}

		internal HtmlAttributeParts Parts
		{
			get
			{
				this.AssertValid();
				return this.tagContext.GetAttributePartsImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie));
			}
		}

		public int ReadValue(char[] buffer, int offset, int count)
		{
			this.AssertValid();
			return this.tagContext.ReadAttributeValueImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), buffer, offset, count);
		}

		public void Write()
		{
			this.AssertValid();
			this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), true, true);
		}

		public void WriteName()
		{
			this.AssertValid();
			this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), true, false);
		}

		public void WriteValue()
		{
			this.AssertValid();
			this.tagContext.WriteAttributeImpl(HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie), false, true);
		}

		public override string ToString()
		{
			if (this.tagContext != null)
			{
				return HtmlTagContext.ExtractIndex(this.attributeIndexAndCookie).ToString();
			}
			return "null";
		}

		private void AssertValid()
		{
			if (this.tagContext == null)
			{
				throw new InvalidOperationException(TextConvertersStrings.AttributeNotInitialized);
			}
			this.tagContext.AssertAttributeValid(this.attributeIndexAndCookie);
		}

		public static readonly HtmlTagContextAttribute Null = default(HtmlTagContextAttribute);

		private HtmlTagContext tagContext;

		private int attributeIndexAndCookie;
	}
}
